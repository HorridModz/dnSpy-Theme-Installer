using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using static dnSpy.Theme.Installer.Utils.FileUtils;

namespace dnSpy.Theme.Installer;

public class ThemeInstaller
{
    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once InconsistentNaming
    public readonly string dnSpyThemesDirectory;

    public ThemeInstaller(string dnSpyDirectory)
    {
        AssertDirectoryExists(dnSpyDirectory);
        
        dnSpyThemesDirectory = Path.Combine(dnSpyDirectory, "bin", "Themes");

        AssertDirectoryExists(dnSpyThemesDirectory);
        AssertDirectoryExists(BuiltinThemesDirectory);
    }

    
    public static string BuiltinThemesDirectory =>
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Themes");
    private static string FormatBuiltinTheme(string theme)
    { 
        // Replace space with underscore to match theme file naming convention
        theme = theme.Replace(" ", "_");
        // Remove file extension if it exists
        theme = Path.GetFileNameWithoutExtension(theme);
        return theme;
    }
    
    private Dictionary<string, string> BuiltinThemesDictionary
    {
        get
        {
            var builtinThemesPaths = Directory.GetFiles(BuiltinThemesDirectory).ToList();
            var builtinThemes = new Dictionary<string, string>();
            foreach (var themePath in builtinThemesPaths)
            {
                builtinThemes.Add(Path.GetFileName(themePath), themePath);
            }

            return builtinThemes;
        }
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public List<string> BuiltinThemes => BuiltinThemesDictionary.Keys.ToList();

    private string GetBuiltinThemePath(string theme)
    {
        // Replace space with underscore to match theme file naming convention
        var oldTheme = theme;
        theme = FormatBuiltinTheme(theme);
        if (!(BuiltinThemesDictionary.ContainsKey(theme)))
        {
            throw new ArgumentException($"{oldTheme} is not a built-in theme");
        }

        return BuiltinThemesDictionary[theme];
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public void InstallTheme(string themePath)
    {
        if (File.Exists(themePath))
        { 
            File.Copy(themePath, dnSpyThemesDirectory + Path.GetFileName(themePath));  
        }
        else if (Directory.Exists(themePath))
        {
            foreach (var file in Directory.GetFiles(themePath, "*.dntheme"))
            {
                InstallTheme(file);
            }
        }
        else
        {
            throw new FileNotFoundException($"File / folder at path `{themePath}` does not exist.");
        }
    }
    
    public void InstallThemes(List<string> themePaths)
    {
        foreach (var themePath in themePaths)
        {
            InstallTheme(themePath);
        }
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public void InstallBuiltinTheme(string theme)
    {
        InstallTheme(GetBuiltinThemePath(theme));
    }
    
    public void InstallBuiltinThemes()
    {
        InstallBuiltinThemes(BuiltinThemes);
    }
    
    public void InstallBuiltinThemes(List<string> themes)
    {
        foreach (var theme in themes)
        {
            InstallBuiltinTheme(theme);   
        }
    }

    public void InstallBuiltinThemesExcluding(List<string> excludeThemes)
    {
        // Format all excluded themes
        excludeThemes = excludeThemes.Select(FormatBuiltinTheme).ToList();
        foreach (var theme in BuiltinThemes)
        {
            if (!(excludeThemes.Contains(FormatBuiltinTheme(theme))))
            {
                InstallBuiltinTheme(theme);
            }
        }
    }
}