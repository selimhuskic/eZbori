using DAL.Exceptions;
using MediatR;

namespace DAL.Queries.GeneralElections.State;

public class GetStateMunicipalPartyQuery : IRequest<TableCandidateReadModel>
{
    public int ElectionYear { get; }
    public int MunicipalityCode { get; }

    public GetStateMunicipalPartyQuery(int electionYear, int municipalityCode)
        => (ElectionYear, MunicipalityCode) = (electionYear, municipalityCode);
}

public class GetStateMunicipalPartyQueryHandler(IStateRepository repository) : IRequestHandler<GetStateMunicipalPartyQuery, TableCandidateReadModel>
{
    private readonly IStateRepository _repository = repository;

    public async Task<TableCandidateReadModel> Handle(GetStateMunicipalPartyQuery request, CancellationToken cancellationToken)
    {
        var stateMunicipalParties = await _repository.GetStateMunicipalPartiesAsync(request.ElectionYear, request.MunicipalityCode).ConfigureAwait(false);

        return stateMunicipalParties ?? throw new UserException("No state municipal party results found!");
    }
}
