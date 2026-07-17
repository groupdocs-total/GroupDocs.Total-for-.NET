namespace GroupDocs.Total.Examples.CSharp.DeveloperGuide.Assembly;

using System.Collections.Generic;
using System;
using GroupDocs.Assembly.Data;
using GroupDocs.Assembly;

// Data classes must be public — see the note below
public class LineItem
{
    public string Description { get; set; } = "";
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

public class Order
{
    public string Number { get; set; } = "";
    public string CustomerName { get; set; } = "";
    public DateTime Date { get; set; }
    public List<LineItem> Items { get; set; } = new();
}

public static class AssemblyToPdf
{
    public static void Run()
    {
        var invoice = new Order
        {
            Number = "INV-2026-0043",
            CustomerName = "Contoso Manufacturing",
            Date = new DateTime(2026, 7, 16),
            Items = new List<LineItem>
            {
                new LineItem { Description = "Annual subscription", Quantity = 1, Price = 9600m },
                new LineItem { Description = "Additional seats",    Quantity = 5, Price = 240m }
            }
        };

        DocumentAssembler assembler = new DocumentAssembler();

        assembler.AssembleDocument(
            "invoice-template.docx",
            "assembly-to-pdf.pdf",
            new DataSourceInfo(invoice, "invoice"));

        Console.WriteLine("Generated assembly-to-pdf.pdf");
    }
}
