namespace GroupDocs.Total.Examples.CSharp.DeveloperGuide.Assembly;

using System.Collections.Generic;
using System;
using GroupDocs.Assembly.Data;
using GroupDocs.Assembly;

public class InvoiceItem
{
    public string Description { get; set; } = "";
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

public class Invoice
{
    public string Number { get; set; } = "";
    public string CustomerName { get; set; } = "";
    public DateTime Date { get; set; }
    public List<InvoiceItem> Items { get; set; } = new();
}

public static class AssemblyFromObjects
{
    public static void Run()
    {
        var invoice = new Invoice
        {
            Number = "INV-2026-0042",
            CustomerName = "Northwind Trading Ltd",
            Date = new DateTime(2026, 7, 16),
            Items = new List<InvoiceItem>
            {
                new InvoiceItem { Description = "Document processing licence", Quantity = 3, Price = 1200m },
                new InvoiceItem { Description = "Priority support",            Quantity = 1, Price = 800m },
                new InvoiceItem { Description = "Onboarding workshop",         Quantity = 2, Price = 450m }
            }
        };

        DocumentAssembler assembler = new DocumentAssembler();

        // "invoice" must match the name the template's tags use
        assembler.AssembleDocument(
            "invoice-template.docx",
            "assembly-from-objects.docx",
            new DataSourceInfo(invoice, "invoice"));

        Console.WriteLine("Generated assembly-from-objects.docx");
    }
}
