using Bislerium.Data.Services;
using Bislerium.Data;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Colors = QuestPDF.Helpers.Colors;
using QuestPDF.Previewer;
using IContainer = QuestPDF.Infrastructure.IContainer;
using Bislerium.Data.Models;
using Bislerium.Data.Enums;


public static class ReportService
{
   
    // Get total coffee revenue of the month
    public static double CalculateTotalCoffeeRevenue()
    {
        DateTime currentMonthStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        DateTime currentMonthEnd = currentMonthStart.AddMonths(1).AddDays(-1);
        List<Order> orders = OrderService.GetAll();
        List<CoffeeItem> coffees = CoffeeService.GetAll();
        return orders
            .Where(order => order.CreatedAt >= currentMonthStart && order.CreatedAt <= currentMonthEnd && order.OrderType != OrderType.Complimentary && coffees.Any(coffee => coffee.Id == order.Coffee))
            .Sum(order => coffees.First(coffee => coffee.Id == order.Coffee).Price);
    }

    // Get total add-in revenue of the month
    public static double CalculateTotalAddInRevenue()
    {
        DateTime currentMonthStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        DateTime currentMonthEnd = currentMonthStart.AddMonths(1).AddDays(-1);
        List<Order> orders = OrderService.GetAll();
        List<AddInItem> addIns = AddInService.GetAll();
        return orders
            .Where(order => order.CreatedAt >= currentMonthStart && order.CreatedAt <= currentMonthEnd && order.OrderType != OrderType.Complimentary && addIns.Any(addIn => addIn.Id == order.AddIn))
            .Sum(order => addIns.First(addIn => addIn.Id == order.AddIn).Price);
    }

    // Calculate overall nevenue of the month
    public static double CalculateOverallRevenueInCurrentMonth()
    {
        DateTime currentMonthStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        DateTime currentMonthEnd = currentMonthStart.AddMonths(1).AddDays(-1);
        List<Order> orders = OrderService.GetAll();
        return orders
            .Where(order => order.CreatedAt >= currentMonthStart && order.CreatedAt <= currentMonthEnd)
            .Sum(order => order.Total);
    }

    // Get Top Coffees of the month
    public static IEnumerable<(string CoffeeType, int Quantity, double Revenue)> GetTopCoffees()

    {
        List<Order> orders = OrderService.GetAll();
        var currentMonth = DateTime.Now.Month;

         var topCoffees = orders
        .Where(order => order.CreatedAt.Month == currentMonth && order.Coffee != Guid.Empty)
        .GroupBy(order => order.Coffee)
        .Select(group => new
        {
            CoffeeType = CoffeeService.GetById(group.Key)?.CoffeeType,
            Quantity = group.Count(),
            Revenue = (double)group.Sum(order => CoffeeService.GetById(order.Coffee)?.Price ?? 0)
        })
        .Where(result => result.CoffeeType != null) // Filter out null CoffeeType
        .OrderByDescending(result => result.Quantity)
        .Take(5);

        return topCoffees.Select(result => (result.CoffeeType, result.Quantity, result.Revenue));
    }

    // Get Top Add-Ins of the month
    public static IEnumerable<(string AddInName, int Quantity, double Revenue)> GetTopAddIns()
    {
        List<Order> orders = OrderService.GetAll();
        var currentMonth = DateTime.Now.Month;

        var topAddIns = orders
            .Where(order => order.CreatedAt.Month == currentMonth && order.AddIn != Guid.Empty)
            .GroupBy(order => order.AddIn)
            .Select(group => new
            {
                AddInName = AddInService.GetById(group.Key)?.AddInName,
                Quantity = group.Count(),
                Revenue = (double)group.Sum(order => AddInService.GetById(order.AddIn)?.Price ?? 0)
            })
            .OrderByDescending(result => result.Quantity)
            .Take(5);

        return topAddIns.Select(result => (result.AddInName, result.Quantity, result.Revenue));
    }

    // Generate Report
    public static void GenerateReportPDF()
    {
        QuestPDF.Settings.License = LicenseType.Community;

        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(50);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(14));

                page.Header()
                    .Row(row =>
                    {
                        row.RelativeItem().Column(column =>
                        {
                            column.Item().Text("Bislerium Monthly Sales Report").Style(TextStyle.Default.FontSize(20).SemiBold().FontColor(Colors.Green.Lighten1));

                            column.Item().Text(text =>
                            {
                                text.Span("Issue date: ").SemiBold();
                                text.Span($"{DateTime.Now}");
                            });                       
                        });
                    });

                page.Content()
                    .PaddingVertical(40)
                    .Column(column =>
                    {
                        column.Spacing(5);

                        column.Item().Text("Most Frequently Purchased Coffees").Style(TextStyle.Default.FontSize(16).SemiBold());

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(40);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("S.N.");
                                header.Cell().Element(CellStyle).Text("Coffee");
                                header.Cell().Element(CellStyle).AlignRight().Text("Quantity");
                                header.Cell().Element(CellStyle).AlignRight().Text("Total Revenue");

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                                }
                            });

                            // Loop through the data
                            IEnumerable<(string CoffeeType, int Quantity, double Revenue)> topCoffees = GetTopCoffees();
                           
                            int coffeeIndex = 1;

                            foreach (var coffeeItem in topCoffees)
                            {
                                table.Cell().Element(CellStyle).Text(coffeeIndex);
                                table.Cell().Element(CellStyle).Text(coffeeItem.CoffeeType);
                                table.Cell().Element(CellStyle).AlignRight().Text(coffeeItem.Quantity);
                                table.Cell().Element(CellStyle).AlignRight().Text($"${coffeeItem.Revenue}");

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                                }
                                coffeeIndex += 1;
                            }
                        });

                        column.Spacing(20);

                        column.Item().Text("Most Frequently Purchased AddIns").Style(TextStyle.Default.FontSize(16).SemiBold());

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(40);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("S.N.");
                                header.Cell().Element(CellStyle).Text("AddIn Name");
                                header.Cell().Element(CellStyle).AlignRight().Text("Quantity");
                                header.Cell().Element(CellStyle).AlignRight().Text("Total Revenue");

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                                }
                            });



                            // Loop through the data
                            IEnumerable<(string AddInName, int Quantity, double Revenue)> topAddIns = GetTopAddIns();


                            int addInIndex = 1;

                            foreach (var addInItem in topAddIns)
                            {
                                table.Cell().Element(CellStyle).Text(addInIndex);
                                table.Cell().Element(CellStyle).Text(addInItem.AddInName);
                                table.Cell().Element(CellStyle).AlignRight().Text(addInItem.Quantity);
                                table.Cell().Element(CellStyle).AlignRight().Text($"${addInItem.Revenue}");

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                                }
                                addInIndex += 1;
                            }
                        });

                        column.Item().Text(text =>
                        {
                            text.Span("Total Coffee Revenue: ").SemiBold();
                            text.Span($"${CalculateTotalCoffeeRevenue()}");
                        });
                        column.Item().Text(text =>
                        {
                            text.Span("Total Add Ins Revenue: ").SemiBold();
                            text.Span($"${CalculateTotalAddInRevenue()}");
                        });
                        column.Item().Text(text =>
                        {
                            text.Span("Total Revenue: ").SemiBold();
                            text.Span($"${CalculateOverallRevenueInCurrentMonth()}");
                        });
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Page ").SemiBold();
                        x.CurrentPageNumber();
                    });
            });
        }).GeneratePdf(Utils.GetReportsFilePath() + DateTime.Now.ToString("yyyyMMddHHmmss") + "_SalesReport.pdf");
    }
}

