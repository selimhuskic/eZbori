using Application.Services;
using MediatR;

namespace DAL.Commands.User;

public record SendInformationalEmailCommand(IEnumerable<int> UserIds, string Subject, string Body) : IRequest;

internal sealed class SendInformationalEmailCommandHandler(
    IUserRepository userRepository,
    IInformationalEmailQueue emailQueue) : IRequestHandler<SendInformationalEmailCommand>
{
    public async Task Handle(SendInformationalEmailCommand request, CancellationToken cancellationToken)
    {
        foreach (var userId in request.UserIds)
        {
            var user = await userRepository.GetUserByIdAsync(userId);
            if (user is null) continue;

            await emailQueue.PublishAsync(new Application.Models.InformationalEmailMessage(
                user.FirstName, user.LastName, user.Email, request.Subject, request.Body));
        }
    }
}
