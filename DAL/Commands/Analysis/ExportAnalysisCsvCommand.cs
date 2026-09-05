using Application.DTOs;
using Application.Repositories;
using MediatR;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace DAL.Commands.Analysis;

public record ExportAnalysisCsvCommand(AnalysisRequest Request) : IRequest<string>;

internal sealed class ExportAnalysisCsvCommandHandler(IAnalysisRepository repository)
    : IRequestHandler<ExportAnalysisCsvCommand, string>
{
    private static readonly PropertyInfo[] _props = typeof(PartiesExportRow).GetProperties();

    public async Task<string> Handle(ExportAnalysisCsvCommand request, CancellationToken cancellationToken)
    {
        var parties = await repository.GetPartiesAsync(request.Request, cancellationToken);

        var csv = new StringBuilder();

        csv.AppendLine(string.Join(",",
            _props.Select(p => FormatValue(p.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? p.Name))));

        foreach (var party in parties)
        {
            var row = new PartiesExportRow(party);
            csv.AppendLine(string.Join(",", _props.Select(p => FormatValue(p.GetValue(row)))));
        }

        return csv.ToString();
    }

    private static string FormatValue(object? value)
    {
        var raw = value switch
        {
            null => string.Empty,
            decimal d => d.ToString("F4", CultureInfo.InvariantCulture),
            IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
            _ => value.ToString() ?? string.Empty
        };

        return $"\"{raw.Replace("\"", "\"\"")}\"";
    }
}
