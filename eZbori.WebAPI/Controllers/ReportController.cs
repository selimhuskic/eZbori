using Application.Repositories;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Unit = QuestPDF.Infrastructure.Unit;

namespace eZbori.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReportController(
    IMediator mediator,
    IUserRepository userRepository,
    IElectionCycleRepository electionCycleRepository) : BaseEZboriController(mediator)
{
    static ReportController() => QuestPDF.Settings.License = LicenseType.Community;

    [Authorize(Roles = "Administrator")]
    [HttpGet("users")]
    public async Task<IActionResult> UsersReport()
    {
        var users = (await userRepository.GetAllAsync()).ToList();

        var pdf = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1, Unit.Centimetre);

                page.Header()
                    .PaddingBottom(10)
                    .Text("Lista korisnika — eZbori")
                    .Bold().FontSize(16).AlignCenter();

                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.RelativeColumn(3);
                        cols.RelativeColumn(4);
                        cols.RelativeColumn(2);
                        cols.RelativeColumn(2);
                    });

                    table.Header(header =>
                    {
                        foreach (var h in new[] { "Ime i prezime", "Email", "Uloga", "Status" })
                            header.Cell().Border(0.5f).Padding(4).Text(h).Bold();
                    });

                    foreach (var u in users)
                    {
                        table.Cell().Border(0.5f).Padding(4).Text($"{u.FirstName} {u.LastName}");
                        table.Cell().Border(0.5f).Padding(4).Text(u.Email ?? "");
                        table.Cell().Border(0.5f).Padding(4).Text(u.UserRole == 2 ? "Administrator" : "Korisnik");
                        table.Cell().Border(0.5f).Padding(4).Text(u.UserVerified ? "Ovjeren" : "Neovjeren");
                    }
                });

                page.Footer().AlignCenter()
                    .Text($"Generirano: {DateTime.UtcNow:dd.MM.yyyy HH:mm}");
            });
        });

        return File(pdf.GeneratePdf(), "application/pdf", "korisnici.pdf");
    }

    [Authorize(Roles = "Administrator")]
    [HttpGet("election-cycles")]
    public async Task<IActionResult> ElectionCyclesReport()
    {
        var cycles = (await electionCycleRepository.GetAllAsync()).ToList();

        var pdf = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1, Unit.Centimetre);

                page.Header()
                    .PaddingBottom(10)
                    .Text("Izborni ciklusi — eZbori")
                    .Bold().FontSize(16).AlignCenter();

                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.ConstantColumn(60);
                        cols.RelativeColumn(2);
                        cols.RelativeColumn(3);
                        cols.RelativeColumn(4);
                    });

                    table.Header(header =>
                    {
                        foreach (var h in new[] { "Godina", "Tip", "Result Key", "API URL" })
                            header.Cell().Border(0.5f).Padding(4).Text(h).Bold();
                    });

                    foreach (var c in cycles)
                    {
                        var typeName = c.ElectionType == 1 ? "Opšti" : "Lokalni";
                        table.Cell().Border(0.5f).Padding(4).Text($"{c.Year}");
                        table.Cell().Border(0.5f).Padding(4).Text(typeName);
                        table.Cell().Border(0.5f).Padding(4).Text(c.ResultKey);
                        table.Cell().Border(0.5f).Padding(4).Text(c.ApiBaseUrl);
                    }
                });

                page.Footer().AlignCenter()
                    .Text($"Generirano: {DateTime.UtcNow:dd.MM.yyyy HH:mm}");
            });
        });

        return File(pdf.GeneratePdf(), "application/pdf", "izborni-ciklusi.pdf");
    }
}
