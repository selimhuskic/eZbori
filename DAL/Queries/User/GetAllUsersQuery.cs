using MediatR;

namespace DAL.Queries.User;

public record GetAllUsersQuery(int Page, int PageSize) : IRequest<(IEnumerable<Application.Models.User> Items, int Total)>;

internal sealed class GetAllUsersQueryHandler(IUserRepository userRepository)
    : IRequestHandler<GetAllUsersQuery, (IEnumerable<Application.Models.User> Items, int Total)>
{
    private readonly IUserRepository _userRepository = userRepository;

    public Task<(IEnumerable<Application.Models.User> Items, int Total)> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        => _userRepository.GetPagedAsync(request.Page, request.PageSize, cancellationToken);
}
