using Application.Repositories;
using MediatR;

namespace DAL.Commands.Municipality;

public record UpdateMunicipalityCommand(int Id, string Name, int Population) : IRequest;

public class UpdateMunicipalityCommandHandler(IMunicipalityServiceRepository repo) : IRequestHandler<UpdateMunicipalityCommand>
{
    public async Task Handle(UpdateMunicipalityCommand request, CancellationToken cancellationToken)
        => await repo.UpdateAsync(request.Id, request.Name, request.Population);
}
