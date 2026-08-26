using MediatR;

namespace DAL.Queries.User;

public record GetAllUsersQuery : IRequest<IEnumerable<Application.Models.User>>;

internal sealed class GetAllUsersQueryHandler(IUserRepository userRepository)
    : IRequestHandler<GetAllUsersQuery, IEnumerable<Application.Models.User>>
{
    private readonly IUserRepository _userRepository = userRepository;

    public Task<IEnumerable<Application.Models.User>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        => _userRepository.GetAllAsync();
}
