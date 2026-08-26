using Application.Enum;

namespace Application.ReadModels;

public class MunicipalityReadModel
{
    public int MunicipalityCode { get; }
    public string Name { get; }
    public Entity Entity { get; }
    public StateParliamentElectoralUnit StateParliamentElectoralUnit { get; }
    public EntityParliamentElectoralUnit EntityParliamentElectoralUnit { get; }
    public CantonParliamentElectoralUnit? CantonParliamentElectoralUnit { get; }
    public decimal Latitude { get; }
    public decimal Longitude { get; }
    public int Population { get; }
    public decimal Area { get; }

    public MunicipalityReadModel(int municipalityCode,
        string name,
        Entity entity,
        StateParliamentElectoralUnit stateParliamentElectoralUnit,
        EntityParliamentElectoralUnit entityParliamentElectoralUnit,
        CantonParliamentElectoralUnit? cantonParliamentElectoralUnit,
        decimal latitude,
        decimal longitude,
        int population,
        decimal area)
        => (MunicipalityCode, Name, Entity, 
                StateParliamentElectoralUnit, EntityParliamentElectoralUnit, 
                    CantonParliamentElectoralUnit, Latitude, Longitude, Population, Area) 
            = (municipalityCode, name, entity, 
                    stateParliamentElectoralUnit, entityParliamentElectoralUnit, 
                        cantonParliamentElectoralUnit, latitude, longitude, population, area);
}
