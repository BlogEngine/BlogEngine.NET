# Work on styles in BlogEngine's admin

BlogEnigne uses Sass (scss) preprocessor to build CSS styles for admin theme.
Please follow these steps to convert .scss files into CSS styles.

The simplest way for Windows users:

1. Download and install the latest version of [RubyInstaller].

2. In the command prompt, type and run following command:

  `gem install sass`

3. Change directory in command prompt to match path to admin theme in your project

  For example:

  `cd D:\BlogEngine\BlogEngine.NET\admin\themes\standard\`

4. Run this command to start file watcher:

  `sass --watch scss/styles.scss:css/styles.css --style compressed`

  This will monitor any changes in the .scss files and convert them into CSS on the fly.

[rubyinstaller]: http://rubyinstaller.org/downloads/
