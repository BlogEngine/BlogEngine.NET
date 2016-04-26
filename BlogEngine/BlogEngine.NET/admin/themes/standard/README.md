# Work on styles in BlogEngine's admin

Admin of BlogEnigne is implemented by Sass(scss) preprocessor, so if you want to work on CSS styles, you should know how Sass works. so if you know, follow these steps to compile the Sass.

The simple way for Windows users:

1. Download and install the latest version of [RubyInstaller].

2. In your command prompt type and run:

  `gem install sass`

3. Then you have to change directory of command prompt to:

  For you might be in different path.

  `cd D:\BlogEngine\BlogEngine.NET\admin\themes\standard\`

4. Then for compile on saving file, you have to only run this command:

  `sass --watch scss/styles.scss:css/styles.css --style compressed`

  This command will wait until you change the `.scss` files, then will compile automatically.

[rubyinstaller]: http://rubyinstaller.org/downloads/
