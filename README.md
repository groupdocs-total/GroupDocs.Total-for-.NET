# GroupDocs.Total for .NET Demo Projects

[![NuGet](https://img.shields.io/nuget/v/GroupDocs.Total)](https://www.nuget.org/packages/GroupDocs.Total/)
![Downloads](https://img.shields.io/nuget/dt/GroupDocs.Total?label=nuget%20downloads)

This repository contains demo web applications built with [GroupDocs.Total for .NET](https://products.groupdocs.com/total/net/) — a comprehensive suite of document management APIs. The demos showcase document viewing, annotation, conversion, comparison, digital signatures, editing, metadata management, and full-text search.

## Demo Applications

| Demo | Framework | Path |
|---|---|---|
| [MVC](Demos/MVC) | ASP.NET MVC 5 | `Demos/MVC` |
| [WebForms](Demos/WebForms) | ASP.NET WebForms | `Demos/WebForms` |

## System Requirements

- .NET Framework 4.8
- Visual Studio 2022 or later

## Quick Start

```bash
git clone https://github.com/groupdocs-total/GroupDocs.Total-for-.NET
```

1. Open the solution for your preferred framework in Visual Studio
2. Build and run
3. Open http://localhost:8080/

**Note:** Without a license the applications run in trial mode. You can [request a temporary license](https://purchase.groupdocs.com/temporary-license).

## Configuration

Both apps use `appSettings` in `Web.config` for configuration. All settings have sensible defaults — no configuration is required to run.

To override a setting, add a key to `<appSettings>` using the `section:property` format:

```xml
<appSettings>
  <add key="viewer:filesDirectory" value="C:\MyFiles\Viewer" />
  <add key="viewer:htmlMode" value="false" />
</appSettings>
```

License files are loaded from the `Licenses/` folder by default. Override with:

```xml
<add key="application:licensePath" value="C:\path\to\license.lic" />
```

## Resources

- **Product Home:** [GroupDocs.Total for .NET](https://products.groupdocs.com/total/net/)
- **Documentation:** [GroupDocs.Total Documentation](https://docs.groupdocs.com/total/)
- **API References:** [GroupDocs API](https://apireference.groupdocs.com)
- **Download:** [NuGet Package](https://releases.groupdocs.com/total/net/)
- **Free Support:** [Forum](https://forum.groupdocs.com/c/total)
- **Paid Support:** [Helpdesk](https://helpdesk.groupdocs.com)
- **Blog:** [GroupDocs.Total Blog](https://blog.groupdocs.com/category/groupdocs-total-product-family)

## License

This project is licensed under the MIT License — see the [LICENSE](LICENSE) file for details.
