using MediatR;

namespace DAL.Queries.User;

public class GetUserWithRolesQuery : IRequest<Application.Models.User?>
{
    public string UserName { get; }
    public string Password { get; }

    public GetUserWithRolesQuery(string userName, string password)
        => (UserName, Password) = (userName, password);
}

public class UserWithRolesQueryHandler(
    IUserRepository userRepository,
    IUserRoleRepository userRoleRepository) : IRequestHandler<GetUserWithRolesQuery, Application.Models.User?>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUserRoleRepository _userRoleRepository = userRoleRepository;

    public async Task<Application.Models.User?> Handle(GetUserWithRolesQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserAsync(request.UserName);

        if (user != null)
        {
            var userRoles = await _userRoleRepository.GetUserRoleAsync(user.UserRole);

            return user.WithRoles(userRoles);
        }

       return null;
    }
}

