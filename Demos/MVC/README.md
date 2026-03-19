# GroupDocs.Total for .NET MVC Demo

[![GitHub license](https://img.shields.io/github/license/groupdocs-total/GroupDocs.Total-for-.NET.svg)](https://github.com/groupdocs-total/GroupDocs.Total-for-.NET/blob/main/LICENSE)

## System Requirements

- .NET Framework 4.8
- Visual Studio 2022 or later

## Overview

ASP.NET MVC 5 demo application showcasing [GroupDocs.Total for .NET](https://products.groupdocs.com/total/net/) document manipulation capabilities:

- Document viewing (GroupDocs.Viewer)
- Annotation (GroupDocs.Annotation)
- Digital signatures (GroupDocs.Signature)
- Document comparison (GroupDocs.Comparison)
- Document conversion (GroupDocs.Conversion)
- Document editing (GroupDocs.Editor)
- Metadata management (GroupDocs.Metadata)
- Full-text search (GroupDocs.Search)

**Note:** Without a license the application runs in trial mode. You can [request a temporary license](https://purchase.groupdocs.com/temporary-license).

## How to Run

```bash
git clone https://github.com/groupdocs-total/GroupDocs.Total-for-.NET
```

1. Open `Demos/MVC/GroupDocs.Total MVC.sln` in Visual Studio
2. Build and run
3. Open http://localhost:8080/

## Project Structure

```
src/
  Client/          - Frontend assets (Angular bundles, CSS, images)
  Controllers/     - MVC controllers
  Files/           - Sample documents for each product
  Products/        - Business logic per product (config, API controllers, services)
  Views/           - Razor views
  Web.config       - Application configuration
```

## Configuration

Settings use `appSettings` in `Web.config` with `section:property` keys. All settings have sensible defaults.

```xml
<appSettings>
  <add key="viewer:filesDirectory" value="C:\MyFiles\Viewer" />
  <add key="application:licensePath" value="C:\path\to\license.lic" />
</appSettings>
```

By default, license files are loaded from the `Licenses/` folder.

## Other Demo Apps

- [GroupDocs.Total for .NET WebForms](https://github.com/groupdocs-total/GroupDocs.Total-for-.NET/tree/main/Demos/WebForms)
- [GroupDocs.Total for Java Spring](https://github.com/groupdocs-total/GroupDocs.Total-for-Java/tree/main/Demos/Spring)
- [GroupDocs.Total for Java Dropwizard](https://github.com/groupdocs-total/GroupDocs.Total-for-Java/tree/main/Demos/Dropwizard)

## Resources

- **Product Home:** [GroupDocs.Total for .NET](https://products.groupdocs.com/total/net/)
- **Documentation:** [GroupDocs.Total Documentation](https://docs.groupdocs.com/total/)
- **API References:** [GroupDocs API](https://apireference.groupdocs.com)
- **Download:** [NuGet Package](https://releases.groupdocs.com/total/net/)
- **Free Support:** [Forum](https://forum.groupdocs.com/c/total)
- **Paid Support:** [Helpdesk](https://helpdesk.groupdocs.com)

## License

The MIT License (MIT). See the [LICENSE](https://github.com/groupdocs-total/GroupDocs.Total-for-.NET/blob/main/LICENSE) for details.
