using DAL.Exceptions;
using MediatR;

namespace DAL.Queries.GeneralElections.Presidency;

public class GetPresidencyMunicipalResultsQuery : IRequest<TableCandidateReadModel>
{
    public int ElectionYear { get; }
    public int MunicipalityCode { get; }


    public GetPresidencyMunicipalResultsQuery(int electionYear, int municipalityCode)
        => (ElectionYear, MunicipalityCode) = (electionYear, municipalityCode);
}

public class GetPresidencyMunicipalResultsQueryHandler(IPresidencyRepository repository) : IRequestHandler<GetPresidencyMunicipalResultsQuery, TableCandidateReadModel>
{
    private readonly IPresidencyRepository _repository = repository;

    public async Task<TableCandidateReadModel> Handle(GetPresidencyMunicipalResultsQuery request, CancellationToken cancellationToken)
    {
        var presidencyMunicipalResults = await _repository.GetPresidencyMunicipalResultsAsync(request.ElectionYear, request.MunicipalityCode).ConfigureAwait(false);

        return presidencyMunicipalResults ?? throw new UserException("No presidency municipal results found!");
    }
}
