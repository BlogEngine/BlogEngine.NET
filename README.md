This repository provides latest source code for BlogEngine.NET project. 

<a href="https://blogengine.io/themes/" target="_blank">
<img src="https://blogengine.io/files/images/themes/themes.jpg" alt="Download BlogEngine Themes">
</a> &nbsp;

<br>
<br>

<a href="https://blogengine.io/" target="_blank">
<img src="https://blogengine.io/files/images/github/btn01.png" alt="BlogEngine Website">
</a> &nbsp;
<a href="https://blogengine.io/features/" target="_blank">
<img src="https://blogengine.io/files/images/github/btn02.png" alt="BlogEgnien Features">
</a> &nbsp;
<a href="https://blogengine.io/themes/" target="_blank">
<img src="https://blogengine.io/files/images/github/btn03.png" alt="BlogEngine Themes">
</a> &nbsp;
<a href="https://blogengine.io/docs/" target="_blank">
<img src="https://blogengine.io/files/images/github/btn04.png" alt="BlogEngine Docs">
</a> &nbsp;
<a href="https://blogengine.io/donate/" target="_blank">
<img src="https://blogengine.io/files/images/github/btn05.png" alt="BlogEngine Donate">
</a>

<br>
<br>





# Installation

There are two download options for BlogEngine.NET:

### 1. Web Project
This is an ideal option that you just need to download and copy BlogEngine files on your website and then everything is ready:

Requirements:
  * ASP.NET 4.5 +

Steps:
1. **[Download](https://github.com/rxtur/BlogEngine.NET/releases/download/v3.3.6.0/3360.zip)** and extract zip file on root of your website.
2. Add write permissions to the `App_Data` and `Custom` folder.
3. Installation is done.
4. You can navigate to administration by adding `/admin/` to your website's URL, for example: `http://yourblog.com/admin/`
5. Username: `admin` Password `admin`


### 2. Source Code
This is the developer option. If you are interested is seeing how things work or want to add to the functionality, this is your option.

Environment:
  * Visual Studio 2015 +
  * ASP.NET 4.5 +

Steps:
  1. Clone repository
  2. Open solution in Visual Studio 2015 +
  3. Build and run solution to load website in the browser
  4. You can navigate to administration on: `http://localhost:64079/admin/`
  5. Username: `admin` Password `admin`

### 3. Security Update
After install, update `machineKey` in `Web.config` with values generated with tool [like this](https://www.allkeysgenerator.com/Random/ASP-Net-MachineKey-Generator.aspx). This will prevent known exploit (reported Sep 2019). This only effects if you use default `admin` account.

# Screenshot
More screenshots on the [website](https://blogengine.io).

![dashboard-3](https://cloud.githubusercontent.com/assets/1932785/11760070/0012f9d8-a052-11e5-84a8-e9097cb85f23.png)
