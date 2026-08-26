using DAL.Exceptions;
using MediatR;

namespace DAL.Queries.GeneralElections.Presidency;

public class GetPresidencyResultsQuery : IRequest<TableCandidateReadModel>
{
    public int ElectionYear { get; }

    public Constituency Constituency { get; }

    public GetPresidencyResultsQuery(int electionYear, Constituency constituency)
        => (ElectionYear, Constituency) = (electionYear, constituency);
}

public class GetPresidencyResultsQueryHandler(IPresidencyRepository repository) : IRequestHandler<GetPresidencyResultsQuery, TableCandidateReadModel>
{
    private readonly IPresidencyRepository _repository = repository;

    public async Task<TableCandidateReadModel> Handle(GetPresidencyResultsQuery request, CancellationToken cancellationToken)
    {
        var presidencyResults =
            await _repository.GetPresidencyResultsAsync(request.ElectionYear, request.Constituency).ConfigureAwait(false);

        return presidencyResults ?? throw new UserException("No presidency results found!");
    }
}
