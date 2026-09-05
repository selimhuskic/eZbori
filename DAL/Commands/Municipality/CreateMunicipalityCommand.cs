using Application.Repositories;
using MediatR;

namespace DAL.Commands.Municipality;

public record CreateMunicipalityCommand(Application.Models.Municipality Municipality) : IRequest<Application.Models.Municipality>;

public class CreateMunicipalityCommandHandler(IMunicipalityServiceRepository repo)
    : IRequestHandler<CreateMunicipalityCommand, Application.Models.Municipality>
{
    public async Task<Application.Models.Municipality> Handle(CreateMunicipalityCommand request, CancellationToken cancellationToken)
        => await repo.CreateAsync(request.Municipality);
}
