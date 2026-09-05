using Application.Constants;
using Application.Requests;
using DAL.Commands.User;
using DAL.Queries;
using eZbori.Web.Security;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace eZbori.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController(
    IMediator mediator,
    IPasswordHasher<User> passwordHasher,
    TokenProvider tokenProvider) : BaseEZboriController(mediator)
{
    private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;
    private readonly TokenProvider _tokenProvider = tokenProvider;

    [AllowAnonymous]
    [HttpPost("Login")]
    public async Task<ActionResult<string>> Login(LoginRequest request)
    {
        var user = await _mediator.Send(new GetUserWithRolesQuery(request.Username, request.Password));

        if (user is null)
        {
            return Unauthorized();
        }

        if (string.IsNullOrEmpty(user.Password))
        {
            return Ok(new { status = "password_required", email = user.Email });
        }

        var verified = _passwordHasher.VerifyHashedPassword(user, user.Password, request.Password);

        if (verified != PasswordVerificationResult.Success)
        {
            return Unauthorized();
        }

        var accessToken = _tokenProvider.GenerateJwtToken(user);
        var refreshToken = await _tokenProvider.GenerateRefreshToken(user);

        return Ok(new
        {
            status = "ok",
            accessToken,
            refreshToken,
            mustChangePassword = user.MustChangePassword
        });
    }

    [Authorize(Roles = "Administrator")]
    [HttpGet("all")]
    public async Task<IActionResult> GetAllUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var all = (await _mediator.Send(new GetAllUsersQuery(), cancellationToken)).ToList();
        var items = all
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new AdminUserDto(u.Id, u.Email, u.UserName, u.FirstName, u.LastName, u.UserVerified, u.UserRole));
        
        return Ok(new { items, total = all.Count, page, pageSize });
    }

    [HttpPost("Logout")]
    public async Task<IActionResult> Logout()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        if (userIdClaim is null)
            return BadRequest(new { message = "Already logged out!"});

        if (!int.TryParse(userIdClaim.Value, out int userId))
            return BadRequest("Invalid user ID");

        var existingToken = await _tokenProvider.GetToken(userId);

        if (existingToken is null)
            return NotFound("Refresh token not found");

        await _tokenProvider.RemoveToken(existingToken);

        return Ok(new { message = "Logged out successfully" });
    }

    [AllowAnonymous]
    [HttpPost("RefreshToken")]
    public async Task<ActionResult<object>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var user = await _tokenProvider.ValidateRefreshToken(request.RefreshToken);

        if (user == null)
        {
            return Unauthorized();
        }

        var newAccessToken = _tokenProvider.GenerateJwtToken(user);
        var newRefreshToken = await _tokenProvider.GenerateRefreshToken(user);

        return Ok(new
        {
            accessToken = newAccessToken,
            refreshToken = newRefreshToken
        });
    }

    [Authorize(Roles = "Administrator")]
    [HttpPut("{id:int}/role")]
    public async Task<IActionResult> UpdateRole(int id, [FromBody] UpdateRoleRequest request, CancellationToken cancellationToken)
    {
        if (!Enum.IsDefined(typeof(Application.Enum.UserRole), request.RoleId))
        {
            return BadRequest(new { message = "Nepoznata korisnička uloga." });
        }            

        await _mediator.Send(new UpdateUserRoleCommand(id, request.RoleId), cancellationToken);
        return NoContent();
    }

    [Authorize(Roles = "Administrator")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteUser(int id, CancellationToken cancellationToken)
    {
        var user = await _mediator.Send(new GetUserByIdQuery(id), cancellationToken);

        if (user is null) 
        {
            return NotFound();
        }

        if (user.UserRole == (int)Application.Enum.UserRole.Administrator)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new
            {
                message = "Brisanje administratorskog naloga nije dozvoljeno."
            });
        }            

        await _mediator.Send(new DeleteUserCommand(id), cancellationToken);
        return NoContent();
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost("notify")]
    public async Task<IActionResult> SendNotification([FromBody] SendNotificationRequest request, CancellationToken cancellationToken)
    {
        await _mediator.Send(new SendInformationalEmailCommand(request.UserIds, request.Subject, request.Body), cancellationToken);
        return Ok(new { message = "Poruka je poslana." });
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost("invite")]
    public async Task<IActionResult> InviteUser([FromBody] InviteUserRequest request, CancellationToken cancellationToken)
    {
        await _mediator.Send(new InviteUserCommand(
            request.FirstName, request.LastName, request.Email, request.RoleId, request.Message),
            cancellationToken);
        return Ok(new { message = "Korisnik uspješno pozvan." });
    }

    [AllowAnonymous]
    [HttpPost("setpassword")]
    public async Task<IActionResult> SetPassword([FromBody] SetPasswordRequest request, CancellationToken cancellationToken)
    {
        var user = await _mediator.Send(new GetUserQuery(request.Email, request.Email), cancellationToken);
        if (user is null || !string.IsNullOrEmpty(user.Password))
            return BadRequest(new { message = "Zahtjev nije validan." });

        var hashed = _passwordHasher.HashPassword(user, request.NewPassword);
        await _mediator.Send(new SetPasswordCommand(request.Email, hashed), cancellationToken);
        return Ok(new { message = "Lozinka uspješno postavljena." });
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim is null || !int.TryParse(userIdClaim.Value, out int userId))
            return BadRequest("Invalid user ID");
        var dto = await _mediator.Send(new GetUserProfileQuery(userId), cancellationToken);
        return dto is null ? NotFound() : Ok(dto);
    }

    [Authorize]
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile(
        [FromBody] UpdateUserProfileRequest request, CancellationToken cancellationToken)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim is null || !int.TryParse(userIdClaim.Value, out int userId))
            return BadRequest("Invalid user ID");
        await _mediator.Send(new UpdateUserProfileCommand(
            userId, request.Email, request.FirstName, request.LastName,
            request.DateOfBirth, request.MunicipalityId,
            request.ClearMunicipality ?? false, request.ProfileImageBase64),
            cancellationToken);
        return NoContent();
    }

    [AllowAnonymous]
    [HttpPost("Register")]
    public async Task<ActionResult<User>> Register([FromBody] RegisterRequest registerRequest, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var existingUser = await _mediator.Send(
            new GetUserQuery(registerRequest.Email, registerRequest.Username), 
            cancellationToken);

        if (existingUser is not null)
        {
            return Conflict(new
            {
                error = Errors.UserAlreadyExists,
                message = "A user with this email already exists."
            });
        }

        var newUser = new User
        {
            Email = registerRequest.Email,
            DateOfBirth = DateTime.Parse(registerRequest.DateOfBirth),
            FirstName = registerRequest.FirstName,
            LastName = registerRequest.LastName,
            CreatedAt = DateTime.UtcNow, 
            UserName = registerRequest.Username,
            UserRole = (int)Application.Enum.UserRole.User,
            Password = registerRequest.Password,
            UserVerified = false
        };        

        await _mediator.Send(
            new CreateNewUserCommand(
                newUser.WithHashedPassword(_passwordHasher.HashPassword(newUser, newUser.Password))), cancellationToken);

        return Ok(new { success = true });
    }

    [Authorize]
    [HttpPut("password")]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        if (userIdClaim is null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return BadRequest("Invalid user ID");
        }

        var user = await _mediator.Send(new GetUserByIdQuery(userId), cancellationToken);
        if (user is null)
        {
            return NotFound();
        }

        var verified = _passwordHasher.VerifyHashedPassword(user, user.Password, request.CurrentPassword);
        if (verified != PasswordVerificationResult.Success)
        {
            return Unauthorized(new { message = "Pogrešna trenutna lozinka." });
        }

        var hashed = _passwordHasher.HashPassword(user, request.NewPassword);
        await _mediator.Send(new ChangePasswordCommand(userId, hashed), cancellationToken);

        return NoContent();
    }

    [Authorize]
    [HttpDelete("me")]
    public async Task<IActionResult> DeleteOwnAccount(CancellationToken cancellationToken)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim is null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return BadRequest("Invalid user ID");
        }

        var existingToken = await _tokenProvider.GetToken(userId);
        if (existingToken is not null)
        {
            await _tokenProvider.RemoveToken(existingToken);
        }

        await _mediator.Send(new DeleteUserCommand(userId), cancellationToken);

        return NoContent();
    }

    [Authorize]
    [HttpPost("force-change-password")]
    public async Task<IActionResult> ForceChangePassword(
        [FromBody] ForceChangePasswordRequest request, CancellationToken cancellationToken)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim is null || !int.TryParse(userIdClaim.Value, out int userId))
            return BadRequest("Invalid user ID");

        var user = await _mediator.Send(new GetUserByIdQuery(userId), cancellationToken);
        if (user is null) return NotFound();

        if (!user.MustChangePassword)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new
            {
                message = "Promjena lozinke nije obavezna za ovaj nalog. Koristite PUT /api/User/password."
            });
        }           

        var hashed = _passwordHasher.HashPassword(user, request.NewPassword);
        await _mediator.Send(new ForceChangePasswordCommand(userId, hashed), cancellationToken);
        return NoContent();
    }

    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        var message = await _mediator.Send(new ForgotPasswordCommand(request.Email), cancellationToken);
        return Ok(new { message });
    }

    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        var user = await _mediator.Send(new GetUserQuery(request.Email, request.Email), cancellationToken);
        if (user is null) return NotFound();
        var hashed = _passwordHasher.HashPassword(user, request.NewPassword);
        var result = await _mediator.Send(new ResetPasswordCommand(request.Email, request.Token, hashed), cancellationToken);
        return result.Success
            ? Ok(new { message = result.Message })
            : BadRequest(new { message = result.Message });
    }
}
