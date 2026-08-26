using Application.Enum;
using DAL.Exceptions;
using MediatR;

namespace DAL.Queries;

public class ElectionYearHandlerValidation(IMediator mediator)
{
    private readonly IMediator _mediator = mediator;

    public virtual async Task Validate(int electionYear, ElectionType electionType, CancellationToken cancellationToken)
    {
        var electionYears = await _mediator.Send(new GetElectionYearsQuery((int)electionType), cancellationToken);

        if (!electionYears.Contains(electionYear))
            throw new UserException("Election year not valid!");
    }
}
