<img align="right" width="80" height="80" data-rmimg src="https://endev.at/content/projects/LPHP/LPHP_Logo_128.png">

# LPHP-Engine v0.2.0
![GitHub](https://img.shields.io/github/license/TobiHatti/LPHP-Engine)
[![GitHub Release Date](https://img.shields.io/github/release-date-pre/TobiHatti/LPHP-Engine?include_prereleases)](https://github.com/TobiHatti/LPHP-Engine/releases)
[![GitHub release (latest by date including pre-releases)](https://img.shields.io/github/v/release/TobiHatti/LPHP-Engine?include_prereleases)](https://github.com/TobiHatti/LPHP-Engine/releases)
[![GitHub last commit](https://img.shields.io/github/last-commit/TobiHatti/LPHP-Engine)](https://github.com/TobiHatti/LPHP-Engine/commits/master)
[![GitHub issues](https://img.shields.io/github/issues-raw/TobiHatti/LPHP-Engine)](https://github.com/TobiHatti/LPHP-Engine/issues)
[![GitHub language count](https://img.shields.io/github/languages/count/TobiHatti/LPHP-Engine)](https://github.com/TobiHatti/LPHP-Engine)

![image](https://endev.at/content/projects/LPHP/LPHP_Banner_300.png)

The LPHP-Engine (Layout-Based PHP-Engine) provides a minimalistic Layout-Engine for PHP. Based on the Layout-features of the ASP.NET Razor framework. 
This allows you to create PHP-Based Websites with layouts, similar to Razor, without the need of a webserver that supports ASP.Net Websites.

## Features

- Layout features similar to ASP.Net Razor in PHP
- Defining and calling local and global variables
- Defining page layouts with just a few lines of code
- Automatic conversion from .lphp files to php-files with debug-information

## Syntax

### LPHP-Instruction-Block
LPHP-Instructions must be placed inside a LPHP-Instruction-Block at the very beginning of the lphp-file:
```php
$${
  // LPHP-Instruction-Block
}
<!-- Everything after the LPHP-Instruction-Block will be interpreted as default HTML+PHP -->
<html>
<head>
  <?php ...
```

Inside this block you can declare variables, set compiler-flags or set the layout.

### Creating variables

Types of variables:
- Local variables: can be accessed by the declaring file, 
as well as all files that get called or have been called from this file
- Global variables: can be accessed by any file in the project-folder

```php
$${
  set MyVariable1 = "SampleText";     // Create local variable "MyVariable1" with string
  set MyVariable2 = 12345;            // Create local variable "MyVariable2" with decimal
  set MyVariable3 = true;             // Create local variable "MyVariable2" with boolean
  
  glob GlobalVariable1 = "SampleText"; // Create global variable "GlobalVariable1" with string
  glob GlobalVariable2 = 12345;        // Create global variable "GlobalVariable2" with decimal
  glob GlobalVariable3 = true;         // Create global variable "GlobalVariable2" with boolean
}
```

### Calling variables

To call a variable within the HTML/PHP section, use the following syntax:

```php
$${
  set PageTitle = "My Website | Home";
}

<html>
  <head>
    <title>$$PageTitle</title>
  </head>
```

### Set Compiler-Flags
To view all possible compiler-flags, click [here](https://github.com/TobiHatti/LPHP-Engine/tree/master#compiler-flags).

Compiler-flags can be used to manipulate and modify the output and the resulting PHP-file.

Compiler-flags must be set in the LPHP-instruction-block at the beginning of the page.

```php
$${
  NoCompile = true;   // Set the NoCompile-Flag to true > No PHP-file gets created from this file.
}
```

### Functions
Functions can be called within the HTML/PHP-section of the file.

For a list of all possible functions, click [here](https://github.com/TobiHatti/LPHP-Engine/tree/master#functions-1)

```php
<div class="pageContents">
  $$RenderPage("Path/To/File/To/Be/Inserted")
</div>
```

## LPHP Preprocessor
### How to set up the LPHP Preprocessor
Download the latest .exe-file from the release-page.
Then, call the .exe-file via commandline, a batch-file or a shortcut.

__NOTE:__ The program takes 1 parameter: The path to the directory, 
where the LPHP-Files are located. (Usually the website's root-folder)

Program-Call (Example):
`"C:\Path\To\LPHP\Executable\LPHP_Preprocessor.exe" "H:\Projects\MyPersonalWebsite"`

If everything was set up correctly, the preprocessor will constantly watch for changes in the 
directory and compile any lphp-files where a change gets detected.

![image](https://endev.at/content/projects/LPHP/projectImages/PreprocessorSample.png)

## Usage
### Creating a Layout-Page
To fully utilise the layout-features, the following template can be used:

- Step 1: Create a Layout-File (.lphp):
In this file, the basic layout of the page is defined, such as header, footer, menu, etc.:
```php
$${
  // Filename: sampleLayout.lphp
  
  NoCompile = true;
}

<html>
  <head>
    <title>$$PageTitle</title>
  <head>
  <body>
    <header> <!-- ... --> </header>
    <nav>
      $$RenderPage("templates/menuTemplate.php")
    </nav>
    <main>
      $$RenderBody()
    </main>
    <footer> <!-- ... --> </footer>
  </body>
</html>
```

- Step 2: Create a content-page (.lphp) that calls the layout:
```php
$${
  // Filename: index.lphp
  
  Layout = "sampleLayout.lphp";
  
  set PageTitle = "Home";
}

<h1>Welcome to my Website</h1>
<article>
  <!-- ... -->
</article>
```

After these files get compiled, one resulting .php-file will be created with the following contents:
```html
<html>
  <head>
    <title>Home</title>
  <head>
  <body>
    <header> <!-- ... --> </header>
    <nav>
      <!-- Menu Placeholder -->
    </nav>
    <main>
      <h1>Welcome to my Website</h1>
      <article>
        <!-- ... -->
      </article>
    </main>
    <footer> <!-- ... --> </footer>
  </body>
</html>
```
By enhancing this sample, you can create multiple layouts that can quickly be 
changed by simply modifying the "Layout"-section in the calling file.

## Currently supported features
### Functions
- `$$RenderBody()`:
Only used within a layout-file. Pastes the content of the calling file in this section.
- `$$RenderPage("Path/To/File")`:
Pastes any files content in this section.

### Compiler-Flags
- `NoCompile` (default: `false`): 
Determines, if a LPHP file should be converted and saved as a PHP-file. Usually set to `true` in layout-pages.

### Preprocessor-Options
These options can be changed in the LPHP.ini-file (located in the same directory as the executable).
- `REMOVE_HTML_COMMENTS` (default: `True`):
Determines, if HTML-comments should still be included in the `.php` output.
- `MIN_OUTPUT_ENABLED` (default: `True`): Outputs the `.php`-file without line-breaks, tabs or unneccecary white-spaces.
- `XML_OUTPUT_ENABLED` (default: `False`, __WORK IN PROGRESS__) Outputs the .php-file with propper indents and line-breaks.

__Note: When only one output mode (`MIN_OUTPUT_ENABLED`, `XML_OUTPUT_ENABLED`) is set to `True`, the file-extension will always be `.php`. When both output-modes are selected, the XML-output has the file-extension `.php` and the MIN-output `.min.php`__
## FAQ

### Q: When trying to run the programm, Windows-Defender pops up and stops me from running the program.
A: The reason for this is the new trust-system of windows-defender that came with windows 10, it blocks any programms 
that don't have enough "reputation", which a program gains reputation by getting downloaded by many individuals. 
Therefor a new program with no reputation gets marked as "Potentially harmfull." To install it anyway, 
click on "More Info" and then on "Run anyway". Alternatively, you can just download the source-code and compile the program yourself.

## Downloads

Get the current version [here](https://github.com/TobiHatti/LPHP-Engine/releases)

Version: 0.2.0

MD5: 7BA47D840BB06A06CB0A88EF9D4DDA64
