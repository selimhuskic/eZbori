using MediatR;

namespace DAL.Queries.User;

public record GetUserQuery(string Email, string Username) : IRequest<Application.Models.User?>;

internal sealed class GetUserQueryHandler(IUserRepository userRepository) : IRequestHandler<GetUserQuery, Application.Models.User?>
{
    private readonly IUserRepository _userRepository = userRepository;

    public Task<Application.Models.User?> Handle(GetUserQuery request, CancellationToken cancellationToken)
        => _userRepository.GetUserAsync(request.Email, request.Username);
}
