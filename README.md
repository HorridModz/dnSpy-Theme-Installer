# dnSpy-Theme-Installer

Automatically adds a curated list of additional themes to dnSpy

# dnSpy-Plugin-Installer

~~dnSpyThemeInstaller has a [sister project](link) called dnSpyPluginInstaller which is almost identical, except it's for plugins instead of themes~~
**dnSpyPluginInstaller** is yet to be created. When it is created, this will be updated.

# Requirements

dnSpyThemeInstaller **ONLY** supports **Windows**.

# Features

- Works automatically and out of the box, with no user input or configuration needed
- Comes with lots of themes built in
- Allows you to supply your own themes
- Easy and simple command line interface
- Can easily and automatically
  install [HoLLy](https://github.com/holly-hacker)'s [ThemeHotReload](https://github.com/HoLLy-HaCKeR/dnSpy.Extension.ThemeHotReload)
  plugin, which allows for
  hot-reloading themes during theme development. This plugin is **ONLY FOR** `.NET 5.0` version of `dnSpy 6.1.8`.

# Built-in Themes

> **Developer Note**: I was too lazy to link all the theme names to their corresponding files manually, so I wrote a regex to do it for me. <br> **Search**: `- \[(.*)]\n` <br> **Replace**: `- [$1](Themes/$1.dntheme)\n` 
> <br> <br> Before using this regex search and replace, write the theme names out in this format (`{REMOVETHIS}` is so the regex does not match *this*, leave it out): <br> - {REMOVETHIS}[THEMENAME] <br> - {REMOVETHIS}[THEME2]


- [Atom One Dark](Themes/Atom_One_Dark.dntheme)
- [Dracula](Themes/Dracula.dntheme)(Themes/Dracula.dntheme)
- [Github Dark](Themes/Github_Dark.dntheme)
- [Material Oceanic](Themes/Material_Oceanic.dntheme)
- [Material Darker](Themes/MaterialDarker.dntheme)
- [Monokai Pro](Themes/Monokai_Pro.dntheme)
- [Night Owl](Themes/Night_Owl.dntheme)

# Usage

> :exclamation: After running any commands, **restart dnSpy**  for the changes to show.

Only **command-line** usage is supported.

To display help, supply the `-h` flag.

```sh
dnSpyThemeInstaller -h
```

To install all of the built-in themes, run `dnSpyThemeInstaller` without any arguments.

```sh
dnSpyThemeInstaller
```

You can also do this by simply running (double-clicking) the exe file.


To only install some of the built-in themes, use the `-i` flag. It does not matter whether you specify the `.dntheme` file extension or not. Separate the themes you want to install with a space (` `). If a theme has a space in its name, enclose the theme in quotes.

```sh
dnSpyThemeInstaller -i <themes_to_install>
```

To install all of the built-in themes except for a few, use the `-e` flag. It does not matter whether you specify the `.dntheme` file extension or not. Separate the themes you want to exclude with a space (` `). If a theme has a space in its name, enclose the theme in quotes.

```sh
dnSpyThemeInstaller -i <themes_to_exclude>
```

To install your own theme(s), provide the location(s) of the file(s) as the `-f` flag. Separate the paths with a space (` `). If a path has a space in its name, enclose the path in quotes.

```sh
dnSpyThemeInstaller -f <file_or_directory>
```

This argument supports theme files or directories containing theme files. You can also mix and match, like this:

```sh
dnSpyThemeInstaller -f "dir_1" "file_1" "dir_2" "file_2"
```

To install the [ThemeHotReload](https://github.com/HoLLy-HaCKeR/dnSpy.Extension.ThemeHotReload) plugin, which allows for hot-reloading themes during theme development, run `dnSpyThemeInstaller` with the `-p` flag:

```sh
dnSpyThemeInstallern -p
```

> **Warning**: This plugin is **ONLY FOR** `.NET 5.0` version of `dnSpy 6.1.8`. It will fail if these requirements are not matched.

## How to do it manually

If you'd prefer to do it yourself rather than using this tool, then I don't know why you're here. But here's how:

Copy the

[UNFINISHED]

# Credits

The built-in themes are from https://forum.wearedevs.net/t/28875.

The theme hot reload plugin is from https://github.com/HoLLy-HaCKeR/dnSpy.Extension.ThemeHotReload.