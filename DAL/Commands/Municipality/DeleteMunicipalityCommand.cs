using MediatR;

namespace DAL.Commands.Municipality;

public record DeleteMunicipalityCommand(int Id) : IRequest;

public class DeleteMunicipalityCommandHandler(IMunicipalityServiceRepository repo) : IRequestHandler<DeleteMunicipalityCommand>
{
    public async Task Handle(DeleteMunicipalityCommand request, CancellationToken cancellationToken)
        => await repo.DeleteAsync(request.Id);
}
