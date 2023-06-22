using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace dnSpy.Theme.Installer;

static class Program
{
    private const string Version = "1.0";

    private static int Main(string[] args)
    {
        if (!(Directory.Exists(ThemeInstaller.BuiltinThemesDirectory)
              && Directory.Exists(ThemeHotReloadPluginInstaller.PluginDirectory))
           )
        {
            Console.Error.WriteLine("Error: Built-in themes and plugins not found - has the exe been moved" +
                                    " out of its folder?");
            return 1;
        }

        if (args.Length == 0)
        {
            Help();
            return 0;
        }

        var arguments = new List<string>();
        var flags = new Dictionary<string, string?>();
        foreach (var arg in args)
        {
            if (arg.StartsWith('-') && arg.Length > 1 && (arg.Length == 2 || char.IsWhiteSpace(arg, 2)))
            {
                if (flags.ContainsKey(arg[0..2]))
                {
                    Console.Error.WriteLine($"Error: Duplicate flag '{arg[0..2]}'");
                    return 1;
                }
                flags.Add(arg[0..2], arg.Length > 2 ? arg[2..] : null);
            }
            else
            {
                arguments.Add(arg);
            }
        }
        
        if (flags.Count == 0 && arguments.Count == 1)
        {
            Help();
            return 0;
        }
        
        string? dnSpyDirectory = null;

        if (arguments.Count == 0)
        {
            // Check if path to dnSpy installation was not supplied but is needed
            
            foreach (var flag in flags)
            {
                if (new List<string> { "b", "-i", "-e", "-f", "-p" }.Contains(flag.Key))
                {
                    Console.Error.WriteLine("Error: Provide the path to your dnSpy installation as the" +
                                            " first argument.");
                    return 1;
                }
            }
        }
        else if (arguments.Count > 1)
        {
            Console.Error.WriteLine("Error: Too many arguments. Provide the path to your" +
                                    " dnSpy installation as the first and only argument.");
            return 1;
        }
        else
        {
            dnSpyDirectory = arguments[0];
        }

        foreach (var flag in flags)
        {
            if (!(new List<string> { "-h", "-l", "-b", "-i", "-e", "-f", "-p" }.Contains(flag.Key)))
            {
                Console.Error.WriteLine($"Error: Unexpected flag: '{flag.Value}'. To see all valid flags, run" +
                                        " dnSpyThemeInstaller without any arguments or with the -h flag");
            }
            
            if (new List<string> { "-i", "-e", "-f" }.Contains(flag.Key)
                && flag.Value == null || flag.Value.All(char.IsWhiteSpace))
            {
                switch (flag.Key)
                {
                    case "-i":
                        Console.Error.WriteLine("Error: Expected value for -i flag - list of built-in theme(s) to" +
                                                " install, separated by space (' ')");
                        return 1;
                    case "-e":
                        Console.Error.WriteLine("Error: Expected value for -e flag - list of built-in theme(s)" +
                                                " not to install, separated by space (' ')");
                        return 1;
                    case "-f":
                        Console.Error.WriteLine("Error: Expected value for -f flag - list of path(s) to" +
                                                " theme files / folders containing theme files to install");
                        return 1;
                }
            }
        }

        if (flags.ContainsKey("-i") && flags.ContainsKey("-e"))
        {
            Console.Error.WriteLine("Error: -i flag (list of built-in theme(s) to install" +
                                    " and -e flag (list of built-in theme(s) not to install" +
                                    " are mutually exclusive. For more information, run" +
                                    " dnSpyThemeInstaller without any arguments or with the -h flag.");
            return 1;
        }

        // Help
        
        if (flags.ContainsKey("-h"))
        {
            Help();
            return 0;
        }
        
        // List All Built-in Themes

        if (flags.ContainsKey("-l"))
        {
            // Replace underscores with spaces
            List<string> themes = ThemeInstaller.BuiltinThemes.Select(
                theme => theme.Replace("_", " ")).ToList();
            Console.WriteLine($"""Built-in Themes:{"\n-"}{string.Join("\n-", themes)}{"\n"}""");
        }

        if (flags.ContainsKey("-b") || flags.ContainsKey("-i") || flags.ContainsKey("-e") ||
            flags.ContainsKey("-f") || flags.ContainsKey("-p"))
        { 
            var themeInstaller = new ThemeInstaller(dnSpyDirectory);
            var themeHotReloadPluginInstaller = new ThemeHotReloadPluginInstaller(dnSpyDirectory);

            // Install Built-in Themes

            if (flags.ContainsKey("-b") || flags.ContainsKey("-i") || flags.ContainsKey("-e"))
            {
                // Remember: includeThemes and excludeThemes are mutually exclusive, so only one of them
                // can exist

                List<string>? includeThemes = null;
                if (flags.ContainsKey("-i"))
                {
                    includeThemes = new List<string>(flags["-i"].Split(" "));
                }

                List<string>? excludeThemes = null;
                if (flags.ContainsKey("-e"))
                {
                    excludeThemes = new List<string>(flags["-e"].Split(" "));
                }

                // Remember: includeThemes and excludeThemes are mutually exclusive, so only one of them
                // can exist

                if (includeThemes != null)
                {
                    Console.WriteLine($"Installing built-in theme(s) {ListItems(includeThemes)}...");
                    themeInstaller.InstallBuiltinThemes(includeThemes);
                }
                else if (excludeThemes != null)
                {
                    Console.WriteLine($"Installing all built-in themes except {ListItems(includeThemes)}...");
                    themeInstaller.InstallBuiltinThemesExcluding(excludeThemes);
                }
                else
                {
                    Console.WriteLine("Installing all built-in themes...");
                    themeInstaller.InstallBuiltinThemes();
                }
            }

            // Install Your Own Themes

            if (flags.ContainsKey("-f"))
            {
                // This regex matches spaces, except when inside single or double quotes.
                var themePaths = new List<string>(Regex.Split(flags["-f"],
                    """\s+(?=(?:[^'"]|'[^']*'|"[^"]*")*$)"""));
                Console.WriteLine($"Installing theme(s) from {ListItems(themePaths)}...");
                themeInstaller.InstallThemes(themePaths);
            }

            // Install ThemeHotReload plugin

            if (flags.ContainsKey("-p"))
            {
                Console.WriteLine("Installing ThemeHotReload plugin...");
                themeHotReloadPluginInstaller.InstallPlugin();
            }
            
            Console.WriteLine("Done! Restart for the changes to show.");    
        }
        
        return 0;
    }

    private static void Help()
    {
        Console.WriteLine(
@$"dnSpyThemeInstaller V{Version} by HorridModz ~ https://github.com/HorridModz/dnSpy-Theme-Installer

See README.md for documentation: https://github.com/HorridModz/dnSpy-Theme-Installer/blob/main/README.md

Display Help: dnSpyThemeInstaller -h
Usage: dnSpyThemeInstaller <path_to_dnSpy_folder> [Options]

Options:
  -h                Display help.
  -b                List all built-in themes.
  -b                Install all built-in themes.
  -i [themes]       Install some built-in theme(s). Separate with space ("" "").
  -e [themes]       Install all built-in theme(s) except these. Separate with space ("" "").
  -f [paths]        Install your own theme(s) at these path(s). Separate with space ("" "").
  -p                Install ThemeHotReload plugin (https://github.com/HoLLy-HaCKeR/dnSpy.Extension.ThemeHotReload).
                      This plugin is **ONLY FOR** .NET 5.0 version of dnSpy 6.1.8.
"
                            );
    }
    
    private static string ListItems(List<string> items)
    {
        // Helper function to convert a list of strings to a grammatically correct string
        
        if (items.Count == 1)
        {
            return items[0];
        }
        else if (items.Count == 2)
        {
            return items[0] + " and " + items[1];
        }
        else
        {
            string result = "";
            for (int i = 0; i < items.Count - 1; i++)
            {
                result += items[i] + ", ";
            }
            result += "and " + items[^1];
            return result;
        }
    }
}