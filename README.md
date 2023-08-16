# dnSpy-Theme-Installer

Automatically adds a curated list of additional themes to dnSpy

## dnSpy-Plugin-Installer

~~dnSpyThemeInstaller has a [sister project](https://github.com/HorridModz/dnSpy-Plugin-Installer) called dnSpyPluginInstaller which is almost identical, except it's for plugins instead of themes.~~
**dnSpyPluginInstaller** is yet to be created. When it is created, this will be updated.

# Using Themes

[dnSpy](https://github.com/dnSpy/dnSpy) is a tool for reverse engineering .NET assemblies. dnSpy supports themes to make it look cooler. To select a theme, use the `View > Theme` menu:

![dnSpy Theme](https://cdn.discordapp.com/attachments/861748086724362260/1020453455975940126/unknown.png)
*Image from https://forum.wearedevs.net/t/28875*

dnSpy comes with a few built-in themes. However, 3 themes isn't enough! Luckily, dnSpy allows you to make custom themes. The goal of this project is to make custom themes more accessible.

# Requirements

- [.NET 7.0 Framework](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)

# Features

- Works automatically and out of the box, with no user input or configuration needed
- Comes with lots of themes built in
- Allows you to supply your own themes
- Easy and simple command line interface
- Can easily and automatically
  install [HoLLy](https://github.com/holly-hacker)'s [ThemeHotReload](https://github.com/HoLLy-HaCKeR/dnSpy.Extension.ThemeHotReload)
  plugin, which allows for
  hot-reloading themes during theme development. This plugin is **ONLY FOR** `.NET 5.0` version of `dnSpy 6.1.8`.
- Cross-platform (I think, hasn't been tested)

# Built-in Themes

> **Developer Note**: I was too lazy to link all the theme names to their corresponding files manually, so I wrote a regex to do it for me. <br> **Search**: `- \[(.*)]\n` <br> **Replace**: `- [$1](Themes/$1.dntheme)\n` 
> <br> <br> Before using this regex search and replace, write the theme names out in this format (`{REMOVETHIS}` is so the regex does not match *this*, leave it out): <br> - {REMOVETHIS}[THEMENAME] <br> - {REMOVETHIS}[THEME2]


- [Atom One Dark](Themes/Atom_One_Dark.dntheme)
- [Dracula](Themes/Dracula.dntheme)
- [Github Dark](Themes/Github_Dark.dntheme)
- [Material Oceanic](Themes/Material_Oceanic.dntheme)
- [Material Darker](Themes/Material_Darker.dntheme)
- [Monokai Pro](Themes/Monokai_Pro.dntheme)
- [Night Owl](Themes/Night_Owl.dntheme)

# Usage

> :exclamation: After running any commands, **restart dnSpy**  for the changes to show.

> **Warning**: If your dnSpy installation folder requires admin rights to modify (this will happen if it is in the `ProgramFiles` or `ProgramFiles (x86)` folder), make sure to run dnSpyThemeInstaller as admin so it can access the folder.

Only **command-line** usage is supported.

> **Note**: If an argument or flag value has multiple words, enclose it in double quotes (""). If you need to use double quotes in the value or the value has a space in it, use single quotes instead (or swap them).

> **Warning**: If there is one argument enclosed in quotes, like this: ` "C:\Users\mr space\uhoh"`, the quotes will be removed automatically by the command line, and it will end up as `C:\Users\mr space\uhoh` - no quotes! To guard against this, nest single quotes inside of double quotes (or vice versa): `"'C:\Users\mr space\itsok'"`

To display help, supply the `-h` flag.

```sh
dnSpyThemeInstaller -h
```

To list all of the built-in themes, use the `l` flag.

```sh
dnSpyThemeInstaller -h
```

To install all of the built-in themes, run `dnSpyThemeInstaller` with the `b` flag. The `b` flag is overridden by the `i` and `e` flags.

```sh
dnSpyThemeInstaller <path_to_dnSpy_folder> -b
```

To only install some of the built-in themes, use the `-i` flag. It does not matter whether you specify the `.dntheme` file extension or not. Separate the themes you want to install with a space (` `). If a theme has a space in its name, enclose it in quotes.
> **Note**: The `i` and `e` flags are mutually exclusive

```sh
dnSpyThemeInstaller <path_to_dnSpy_folder> -i <themes_to_install>
```

To install all of the built-in themes except for a few, use the `-e` flag. It does not matter whether you specify the `.dntheme` file extension or not. Separate the themes you want to exclude with a space (` `). If a theme has a space in its name, enclose it in quotes.
> **Note**: The `i` and `e` flags
are mutually exclusive

```sh
dnSpyThemeInstaller <path_to_dnSpy_folder> -e <themes_to_exclude>
```

To install your own theme(s), provide the location(s) of the file(s) as the `-f` flag. Separate the paths with a space (` `). If a path has a space in its name, enclose it in quotes.

```sh
dnSpyThemeInstaller <path_to_dnSpy_folder> -f <file_or_directory>
```

This argument supports theme files or directories containing theme files. You can also mix and match, like this:

```sh
dnSpyThemeInstaller <path_to_dnSpy_folder> -f "'dir_1' 'file_1' 'dir_2' 'file_2'"
```

To install the [ThemeHotReload](https://github.com/HoLLy-HaCKeR/dnSpy.Extension.ThemeHotReload) plugin, which allows for hot-reloading themes during theme development, run `dnSpyThemeInstaller` with the `-p` flag:

```sh
dnSpyThemeInstallern <path_to_dnSpy_folder> -p
```

> **Warning**: This plugin is **ONLY FOR** `.NET 5.0` version of `dnSpy 6.1.8`. It will not work if these requirements are not matched.

## How to do it manually

If you'd prefer to do it yourself rather than using this tool, then I don't know why you're here. But here's how:

Copy the theme into the `dnSpy\bin\Themes` folder, then restart dnSpy.

# Credits

The built-in themes are from https://forum.wearedevs.net/t/28875.

The theme hot reload plugin is from https://github.com/HoLLy-HaCKeR/dnSpy.Extension.ThemeHotReload.
