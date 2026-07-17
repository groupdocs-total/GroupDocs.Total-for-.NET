using System;
using System.IO;
using System.Threading.Tasks;
using GroupDocs.Total;
using GroupDocs.Total.Examples.CSharp.DeveloperGuide.Annotation;
using GroupDocs.Total.Examples.CSharp.DeveloperGuide.Assembly;
using GroupDocs.Total.Examples.CSharp.DeveloperGuide.Comparison;
using GroupDocs.Total.Examples.CSharp.DeveloperGuide.Conversion;
using GroupDocs.Total.Examples.CSharp.DeveloperGuide.Editor;
using GroupDocs.Total.Examples.CSharp.DeveloperGuide.Merger;
using GroupDocs.Total.Examples.CSharp.DeveloperGuide.Metadata;
using GroupDocs.Total.Examples.CSharp.DeveloperGuide.Parser;
using GroupDocs.Total.Examples.CSharp.DeveloperGuide.Redaction;
using GroupDocs.Total.Examples.CSharp.DeveloperGuide.Search;
using GroupDocs.Total.Examples.CSharp.DeveloperGuide.Signature;
using GroupDocs.Total.Examples.CSharp.DeveloperGuide.Viewer;
using GroupDocs.Total.Examples.CSharp.DeveloperGuide.Watermark;
using GroupDocs.Total.Examples.CSharp.GettingStarted.LicensingAndSubscription;

// Set working directory to the output folder where sample files are copied
Directory.SetCurrentDirectory(AppContext.BaseDirectory);

// =================================================================
//  GroupDocs.Total for .NET Examples
// =================================================================

var examples = new (string Name, Action Run)[]
{
    ("ConversionPdfToWord", ConversionPdfToWord.Run),
    ("ConversionWordToPdf", ConversionWordToPdf.Run),
    ("ConversionSpecificPages", ConversionSpecificPages.Run),
    ("ConversionDocumentInfo", ConversionDocumentInfo.Run),
    ("ViewerRenderToHtml", ViewerRenderToHtml.Run),
    ("ViewerRenderToPdf", ViewerRenderToPdf.Run),
    ("ViewerRenderToPng", ViewerRenderToPng.Run),
    ("ViewerDocumentInfo", ViewerDocumentInfo.Run),
    ("ComparisonCompareDocuments", ComparisonCompareDocuments.Run),
    ("ComparisonListChanges", ComparisonListChanges.Run),
    ("ComparisonAcceptReject", ComparisonAcceptReject.Run),
    ("WatermarkAddText", WatermarkAddText.Run),
    ("WatermarkAddImage", WatermarkAddImage.Run),
    ("WatermarkSpreadsheet", WatermarkSpreadsheet.Run),
    ("MetadataReadInfo", MetadataReadInfo.Run),
    ("MetadataFindProperties", MetadataFindProperties.Run),
    ("MetadataSanitize", MetadataSanitize.Run),
    ("ParserExtractText", ParserExtractText.Run),
    ("ParserExtractMarkdown", ParserExtractMarkdown.Run),
    ("ParserDocumentInfo", ParserDocumentInfo.Run),
    ("MergerJoinDocuments", MergerJoinDocuments.Run),
    ("MergerExtractPages", MergerExtractPages.Run),
    ("MergerDocumentInfo", MergerDocumentInfo.Run),
    ("AssemblyFromObjects", AssemblyFromObjects.Run),
    ("AssemblyToPdf", AssemblyToPdf.Run),
    ("RedactionExactPhrase", RedactionExactPhrase.Run),
    ("RedactionRegex", RedactionRegex.Run),
    ("RedactionMetadata", RedactionMetadata.Run),
    ("SignatureSignWithText", SignatureSignWithText.Run),
    ("SignatureSignWithQrCode", SignatureSignWithQrCode.Run),
    ("SignatureSearch", SignatureSearch.Run),
    ("SearchBuildIndex", SearchBuildIndex.Run),
    ("SearchFuzzy", SearchFuzzy.Run),
    ("SearchBoolean", SearchBoolean.Run),
    ("EditorDocumentToHtml", EditorDocumentToHtml.Run),
    ("EditorHtmlToDocument", EditorHtmlToDocument.Run),
    ("EditorSaveAsRtf", EditorSaveAsRtf.Run),
    ("AnnotationAddArea", AnnotationAddArea.Run),
    ("AnnotationHighlight", AnnotationHighlight.Run),
    ("AnnotationExtract", AnnotationExtract.Run),
    ("LicenseFromFile", LicenseFromFile.Run),
    ("LicenseFromStream", LicenseFromStream.Run),
    ("LicenseEmbedded", LicenseEmbedded.Run),
    ("LicenseMetered", LicenseMetered.Run)
};

var asyncExamples = new (string Name, Func<Task> Run)[]
{

};



// =================================================================
//  CLI dispatch for generate_outputs.py (build once, run many)
// =================================================================

if (args.Length >= 2 && args[0] == "--example")
{
    string exName = args[1];
    string workDir = Environment.GetEnvironmentVariable("EXAMPLE_WORK_DIR") ?? ".";
    Directory.SetCurrentDirectory(workDir);

    PrepareWorkingDir();

    string? licPath = Environment.GetEnvironmentVariable("GROUPDOCS_LIC_PATH");
    if (!string.IsNullOrEmpty(licPath) && File.Exists(licPath))
        License.SetLicense(licPath);

    foreach (var (n, r) in examples)
    {
        if (string.Equals(n, exName, StringComparison.OrdinalIgnoreCase))
        {
            r();
            return;
        }
    }
    foreach (var (n, r) in asyncExamples)
    {
        if (string.Equals(n, exName, StringComparison.OrdinalIgnoreCase))
        {
            await r();
            return;
        }
    }

    Console.Error.WriteLine($"Unknown example: {exName}");
    Environment.Exit(1);
    return;
}

if (args.Length >= 1 && args[0] == "--list")
{
    foreach (var (name, _) in examples) Console.WriteLine(name);
    foreach (var (name, _) in asyncExamples) Console.WriteLine(name);
    return;
}

// =================================================================
//  Default mode: run all examples with interactive output
// =================================================================

PrepareWorkingDir();
PrintIntro();
SetLicense();

foreach (var (name, run) in examples)
    RunExample(name, run);

foreach (var (name, run) in asyncExamples)
    await RunExampleAsync(name, run);

Console.WriteLine();
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine(new string('=', 65));
Console.WriteLine("  All examples completed. Enjoy exploring the GroupDocs API!");
Console.WriteLine(new string('=', 65));
Console.ResetColor();

// =================================================================
//  Helper methods
// =================================================================

static void PrepareWorkingDir()
{
    // nothing to prepare
}

static void PrintIntro()
{
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine();
    Console.WriteLine("=================================================================");
    Console.WriteLine("  Welcome to the GroupDocs.Total for .NET Examples!");
    Console.WriteLine("=================================================================");
    Console.ResetColor();
    Console.WriteLine();
    Console.WriteLine("  Runnable examples for every product bundled in GroupDocs.Total for .NET.");
    Console.WriteLine("  Each example demonstrates different use cases such as:");
    Console.WriteLine();
    Console.WriteLine("  - Viewing and rendering documents.");
    Console.WriteLine("  - Converting between 170+ formats.");
    Console.WriteLine("  - Signing, annotating and comparing documents.");
    Console.WriteLine("  - Extracting text, metadata and search results.");
    Console.WriteLine();
}

static void SetLicense()
{
    // Option 1: Set license from environment variable
    string? licensePath = Environment.GetEnvironmentVariable("GROUPDOCS_LIC_PATH");

    // Option 2: Look for .lic file in the current directory
    if (string.IsNullOrEmpty(licensePath))
    {
        foreach (string file in Directory.GetFiles(".", "*.lic"))
        {
            licensePath = file;
            break;
        }
    }

    if (!string.IsNullOrEmpty(licensePath) && File.Exists(licensePath))
    {
        License.SetLicense(licensePath);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"  License set from: {licensePath}");
        Console.ResetColor();
        Console.WriteLine();
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("  No license file found. Running in evaluation mode.");
        Console.ResetColor();
        Console.WriteLine();
    }
}

static void RunExample(string name, Action run)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine($"Running {name}...");
    Console.ResetColor();
    try
    {
        run();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"  Completed {name}");
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"  Error: {ex.Message}");
    }
    Console.ResetColor();
    Console.WriteLine();
}

static async Task RunExampleAsync(string name, Func<Task> run)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine($"Running {name}...");
    Console.ResetColor();
    try
    {
        await run();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"  Completed {name}");
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"  Error: {ex.Message}");
    }
    Console.ResetColor();
    Console.WriteLine();
}
