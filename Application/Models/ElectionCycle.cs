namespace Application.Models;

public class ElectionCycle
{
    public int Id { get; set; }
    public short Year { get; set; }
    public byte ElectionType { get; set; }
    public string ApiBaseUrl { get; set; } = string.Empty;
    public string ResultKey { get; set; } = string.Empty;
    public bool DataImported { get; set; }
}
