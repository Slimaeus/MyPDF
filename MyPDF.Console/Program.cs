using MyPDF.Console.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;
using System.Text;

QuestPDF.Settings.License = LicenseType.Community;


var document = Document.Create(container =>
{
    container.Page(page =>
    {
        page.Size(PageSizes.A4);
        page.Margin(2, Unit.Centimetre);
        page.DefaultTextStyle(x => x.FontSize(20));

        page.Header()
            .Text(Placeholders.Name())
            .SemiBold().FontSize(30).FontColor(Colors.Blue.Medium);

        page.Content()
            ;

        page.Content()
            .PaddingVertical(1, Unit.Centimetre)
            .Column(x =>
            {
                var random = new Random();
                var people = new List<Person>();

                foreach (var i in Enumerable.Range(0, 10))
                {
                    people.Add(new Person { Name = Placeholders.Name().Split(' ')[0], Age = random.Next(1, 99) });
                }

                x.Item().Table(table =>
                {
                    var properties = people.GetType().GetGenericArguments().Single().GetProperties();
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn();
                        foreach (var property in properties)
                            columns.RelativeColumn();
                    });
                    table
                           .Cell()
                           .Row(1)
                           .Column(1)
                           .Element(x => Block(x)).Text(string.Empty);
                    for (int j = 0; j < properties.Count(); j++)
                        table
                            .Cell()
                            .Row(1)
                            .Column((uint)j + 2)
                            .Element(x => Block(x)).Text(properties[j]?.Name.ToString());

                    for (int i = 0; i < people.Count(); i++)
                    {
                        table
                            .Cell()
                            .Row((uint)i + 2)
                            .Column(1)
                            .Element(x => Block(x)).Text((i + 1).ToString());
                        for (int j = 0; j < properties.Count(); j++)
                            table
                                .Cell()
                                .Row((uint)i + 2)
                                .Column((uint)j + 2)
                                .Element(x => Block(x)).Text(properties[j]?.GetValue(people[i])?.ToString());
                    }

                    static IContainer Block(IContainer container)
                        => container
                            .Border(1)
                            .Background(Colors.White)
                            .ShowOnce()
                            .MinWidth(50)
                            .MinHeight(50)
                            .AlignCenter()
                            .AlignMiddle();
                });

                var sb = new StringBuilder();
                foreach (var person in people)
                {
                    sb.Append(person.Name);
                    sb.Append(' ');
                }
                x.Item().Text(sb.ToString());
                x.Spacing(1, Unit.Centimetre);
                x.Item().Image(Placeholders.Image(200, 100));
            });

        page.Footer()
            .AlignCenter()
            .Text(x =>
            {
                x.Span("Page ");
                x.CurrentPageNumber();
            });
    });
});

document.GeneratePdf("example.pdf");

await document.ShowInPreviewerAsync();