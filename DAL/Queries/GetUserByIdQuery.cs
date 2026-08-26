using MediatR;

namespace DAL.Queries;

public record GetUserByIdQuery(int UserId) : IRequest<Application.Models.User?>;

internal sealed class GetUserByIdQueryHandler(IUserRepository userRepository)
    : IRequestHandler<GetUserByIdQuery, Application.Models.User?>
{
    public Task<Application.Models.User?> Handle(GetUserByIdQuery request, CancellationToken ct)
        => userRepository.GetUserByIdAsync(request.UserId);
}
