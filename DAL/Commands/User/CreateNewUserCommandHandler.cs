using MediatR;

namespace DAL.Commands.User;

public record CreateNewUserCommand(Application.Models.User User) : IRequest<Application.Models.User>;

internal sealed class CreateNewUserCommandHandler(IUserRepository userRepository) : IRequestHandler<CreateNewUserCommand, Application.Models.User>
{
    private readonly IUserRepository _userRepository = userRepository;    

    public async Task<Application.Models.User> Handle(CreateNewUserCommand request, CancellationToken cancellationToken)    
        => await _userRepository.CreatNewUserAsync(request.User);    
}
