# GroupDocs.Total for .NET

[![NuGet](https://img.shields.io/nuget/v/GroupDocs.Total)](https://www.nuget.org/packages/GroupDocs.Total/)
![Downloads](https://img.shields.io/nuget/dt/GroupDocs.Total?label=nuget%20downloads)

Runnable code for [GroupDocs.Total for .NET](https://products.groupdocs.com/total/net/) — one package
containing every GroupDocs document API: viewing, conversion, comparison, signing, annotation,
editing, metadata, search, watermarking, merging, parsing, redaction and assembly.

The repository holds two things, for two different moments:

| | What it is | Start here when you want to |
|---|---|---|
| **[`Examples/`](Examples)** | 44 small console examples, one per feature, across all 13 products | Copy a working snippet for one specific task |
| **[`Demos/`](Demos)** | Two complete ASP.NET web applications | See the APIs wired into a real app end to end |

## Examples

Each example is a self-contained class that opens a sample file, does one thing, and writes an
output — the same code published on the
[GroupDocs.Total documentation](https://docs.groupdocs.com/total/net/developer-guide/). Sample files
sit next to the code that uses them, so an example runs with no setup.

```bash
# Run them all
dotnet run --project Examples/GroupDocs.Total.Examples.CSharp.Net/GroupDocs.Total.Examples.CSharp.Net.csproj

# List every example, then run one on its own
dotnet run --project Examples/GroupDocs.Total.Examples.CSharp.Net/GroupDocs.Total.Examples.CSharp.Net.csproj -- --list
dotnet run --project Examples/GroupDocs.Total.Examples.CSharp.Net/GroupDocs.Total.Examples.CSharp.Net.csproj -- --example ViewerRenderToHtml
```

Two projects share one set of example code, so every example is proven on both runtimes:

| Project | Target | Package |
|---|---|---|
| `GroupDocs.Total.Examples.CSharp.Net` | `net8.0` | `GroupDocs.Total` |
| `GroupDocs.Total.Examples.CSharp.Framework` | `net462` | `GroupDocs.Total.NETFramework` |

See [`Examples/README.md`](Examples/README.md) for the full walkthrough.

> **`Examples/` is generated, not hand-written.** Every example is extracted from the documentation
> by the [`docs-to-code-examples`](https://docs.groupdocs.com/total/net/developer-guide/) pipeline,
> compiled, executed, and its real output published back into the docs page it came from. That is
> what keeps the two in step. **Edit the documentation, not the generated code** — a change made here
> is overwritten on the next run.

## Demos

Complete ASP.NET applications showing document viewing, annotation, conversion, comparison,
signatures, editing, metadata and search working together behind a UI.

| Demo | Framework | Path |
|---|---|---|
| [MVC](Demos/MVC) | ASP.NET MVC 5 | `Demos/MVC` |
| [WebForms](Demos/WebForms) | ASP.NET WebForms | `Demos/WebForms` |

Open the solution for the one you want in Visual Studio 2022, build, run, and browse to
<http://localhost:8080/>. Each demo's README covers its configuration and project layout.

## Requirements

| For | You need |
|---|---|
| `Examples/` | [.NET 8 SDK](https://dotnet.microsoft.com/download) — the `net462` project also needs .NET Framework 4.6.2 targeting packs |
| `Demos/` | .NET Framework 4.8 and Visual Studio 2022 or later |

## Licensing

Everything runs in evaluation mode without a license, which adds trial watermarks to output. You can
[request a temporary license](https://purchase.groupdocs.com/temporary-license) to remove them.

- **Examples** read the `GROUPDOCS_LIC_PATH` environment variable, or pick up any `*.lic` file next
  to the executable.
- **Demos** load from their `Licenses/` folder, or from `application:licensePath` in `Web.config`.

## Resources

- **Product page:** [GroupDocs.Total for .NET](https://products.groupdocs.com/total/net/)
- **Documentation:** [docs.groupdocs.com/total/net](https://docs.groupdocs.com/total/net/)
- **Developer guide:** [every product, with runnable examples](https://docs.groupdocs.com/total/net/developer-guide/)
- **API reference:** [reference.groupdocs.com](https://reference.groupdocs.com/)
- **Releases:** [NuGet](https://www.nuget.org/packages/GroupDocs.Total/) · [direct download](https://releases.groupdocs.com/total/net/)
- **Free support:** [Forum](https://forum.groupdocs.com/c/total)
- **Paid support:** [Helpdesk](https://helpdesk.groupdocs.com)
- **Blog:** [GroupDocs.Total](https://blog.groupdocs.com/category/groupdocs-total-product-family)

## License

This project is licensed under the MIT License — see the [LICENSE](LICENSE) file for details.
