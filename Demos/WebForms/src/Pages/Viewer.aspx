<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Viewer.aspx.cs" Inherits="GroupDocs.Total.WebForms.Viewer" %>

<%
    GroupDocs.Total.WebForms.Products.Common.Config.GlobalConfiguration config = new GroupDocs.Total.WebForms.Products.Common.Config.GlobalConfiguration();
%>
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, minimum-scale=1" />
    <title>Viewer for .NET WebForms</title>
    <link rel="icon" type="image/x-icon" href="/client/viewer/favicon.ico" />
</head>
<body>
    <client-root></client-root>
    <script src="/client/viewer/polyfills-es2015.js" type="module"></script>
    <script src="/client/viewer/polyfills-es5.js" nomodule></script>
    <script src="/client/viewer/runtime-es2015.js" type="module"></script>
    <script src="/client/viewer/runtime-es5.js" nomodule></script>
    <script src="/client/viewer/styles-es2015.js" type="module"></script>
    <script src="/client/viewer/styles-es5.js" nomodule></script>
    <script src="/client/viewer/vendor-es2015.js" type="module"></script>
    <script src="/client/viewer/vendor-es5.js" nomodule></script>
    <script src="/client/viewer/main-es2015.js" type="module"></script>
    <script src="/client/viewer/main-es5.js" nomodule></script>
    <script src="/client/viewer/styles-es2015.js" type="module"></script>
    <script src="/client/viewer/styles-es5.js" nomodule></script>
</body>
</html>
