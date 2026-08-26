using Application.ReadModels;
using Application.Repositories;
using MediatR;

namespace DAL.Queries
{
    public class GetMunicipalitiesQuery : IRequest<IEnumerable<MunicipalityReadModel>>
    {
    }

    public class GetMunicipalitiesQueryHandler(IMunicipalityServiceRepository municipalityRepo) : IRequestHandler<GetMunicipalitiesQuery, IEnumerable<MunicipalityReadModel>>
    {
        private readonly IMunicipalityServiceRepository _municipalityRepo = municipalityRepo;

        public async Task<IEnumerable<MunicipalityReadModel>> Handle(GetMunicipalitiesQuery request,
            CancellationToken cancellationToken) => await _municipalityRepo.GetAllMunicipalities();
    }
}
