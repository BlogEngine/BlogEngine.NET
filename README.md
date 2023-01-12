<br>
<p>
    <strong>BlogEngine</strong> is an open source blogging platform since 2007. Easily customizable. Many free built-in Themes, Widgets, and Plugins.
</p>
<br>

- **[Website](https://blogengine.io/)**
- **[Docs](https://blogengine.io/support/get-started/)**
- **[Themes](https://blogengine.io/Themes)**
- **[Custom Design Theme](https://blogengine.io/)**
- **[Contact us](https://blogengine.io/)**

<br>
<br>
<br>

## Get Started

1. Requirements
   You need a Windows Hosting that supports ASP.NET 4.5 and above.

2. Download
   Get the latest BlogEngine and extract the zip file on the root of your website.

3. Write Permissions
   Add write permissions to the App_Data and Custom folders on your server.

4. Done
   Navigate to admin panel by adding /admin/ to your website's URL.
   For example: https://yourwebsite.com/admin/<br>
   Username: admin<br>
   Password: admin<br>
   <br><br>

## Development

Environment:

- Visual Studio
- ASP.NET 4.5+

Steps:

- Clone repository
- Open solution in Visual Studio 2015 +
- Build and run solution to load website in the browser
- You can navigate to administration on: http://localhost:64079/admin/
- Username: admin Password admin
  <br><br>

## Security Update

After install, update `machineKey` in `Web.config` with values generated with tool [like this](https://www.allkeysgenerator.com/Random/ASP-Net-MachineKey-Generator.aspx). This will prevent known exploit (reported Sep 2019). This only effects if you use default `admin` account.
<br><br>

## Copyright and License

Code released under the MS-RL License. Docs released under Creative Commons.<br>
Copyright 2007–2023 BlogEngine
