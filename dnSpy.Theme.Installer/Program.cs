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
        
        // Parse arguments into flags and normal arguments
        
        var arguments = new List<string>();
        var flags = new Dictionary<string, string?>();
        string? flagName = null;
        foreach (var arg in args)
        {
            if (arg.StartsWith('-'))
            {
                //Flag
                if (flagName != null)
                {
                    // Last arg was a flag without a value.
                    flags.Add(flagName, null);
                }
                // Set flagName now, but don't add it yet since there may be a value
                flagName = arg;
                if (flags.ContainsKey(flagName))
                {
                    Console.Error.WriteLine($"Error: Duplicate flag '{flagName}'");
                    return 1;
                }
            }
            else if (flagName != null)
                // Value for previous flag
            {
                flags.Add(flagName, arg);
                flagName = null;
            }
            else
            {
                // Normal argument
                arguments.Add(arg);
            }
        }
        if (flagName != null)
        {
            // Last arg was a flag without a value. Add that flag now.
            flags.Add(flagName, null);
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
                && (string.IsNullOrEmpty(flag.Value) || flag.Value.All(char.IsWhiteSpace)))
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
            Console.Error.WriteLine("Error: -i flag (list of built-in theme(s) to install)" +
                                    " and -e flag (list of built-in theme(s) not to install)" +
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
            // Ugh, stupid block scope in try-catch
            ThemeInstaller? themeInstaller = null;
            ThemeHotReloadPluginInstaller? themeHotReloadPluginInstaller = null;
            try
            {
                themeInstaller = new ThemeInstaller(dnSpyDirectory);
                themeHotReloadPluginInstaller = new ThemeHotReloadPluginInstaller(dnSpyDirectory);
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return 1;
            }
            
            // Install Built-in Themes

            if (flags.ContainsKey("-b") || flags.ContainsKey("-i") || flags.ContainsKey("-e"))
            {
                // Remember: includeThemes and excludeThemes are mutually exclusive, so only one of them
                // can exist

                List<string>? includeThemes = null;
                if (flags.ContainsKey("-i"))
                {
                    // This regex matches spaces, except when inside single or double quotes.
                    // Then, if there are outer single / double quotes, we remove them
                    // Finally, we remove the file extension if there is one
                    includeThemes = new List<string>(Regex.Split(flags["-i"].Trim(),
                            """\s+(?=(?:[^'"]|'[^']*'|"[^"]*")*$)"""))
                            .Select(TrimOuterQuotes).Select(Path.GetFileNameWithoutExtension).ToList();
                }

                List<string>? excludeThemes = null;
                if (flags.ContainsKey("-e"))
                {
                    // This regex matches spaces, except when inside single or double quotes.
                    // Then, if there are outer single / double quotes, we remove them
                    // Finally, we remove the file extension if there is one
                    excludeThemes = new List<string>(Regex.Split(flags["-e"].Trim(),
                        """\s+(?=(?:[^'"]|'[^']*'|"[^"]*")*$)"""))
                             .Select(TrimOuterQuotes).Select(Path.GetFileNameWithoutExtension).ToList();
                }

                // Remember: includeThemes and excludeThemes are mutually exclusive, so only one of them
                // can exist

                if (includeThemes != null)
                {
                    Console.WriteLine($"Installing built-in theme(s) {ListItems(includeThemes)}...");
                    try
                    {
                        themeInstaller.InstallBuiltinThemes(includeThemes);
                    }
                    catch (ArgumentException e)
                    {
                        Console.WriteLine($"Error: {e.Message}");
                        return 1;
                    }
                }
                else if (excludeThemes != null)
                {
                    Console.WriteLine($"Installing all built-in themes except {ListItems(excludeThemes)}...");
                    try
                    {
                        themeInstaller.InstallBuiltinThemesExcluding(excludeThemes);
                    }
                    catch (ArgumentException e)
                    {
                        Console.WriteLine($"Error: {e.Message}");
                        return 1;
                    }
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
                // Then, if there are outer single / double quotes, we remove them
                var themePaths = new List<string>(Regex.Split(flags["-f"].Trim(),
                        """\s+(?=(?:[^'"]|'[^']*'|"[^"]*")*$)"""))
                    .Select(TrimOuterQuotes).ToList();
                Console.WriteLine($"Installing theme(s) from {ListItems(themePaths)}...");
                try
                {
                    themeInstaller.InstallThemes(themePaths);
                }
                catch (DirectoryNotFoundException e)
                {
                    Console.WriteLine($"Error: {e.Message}");
                    return 1;
                }
            }

            // Install ThemeHotReload plugin

            if (flags.ContainsKey("-p"))
            {
                Console.WriteLine("Installing ThemeHotReload plugin...");
                themeHotReloadPluginInstaller.InstallPlugin();
            }
            
            Console.WriteLine("Done! Restart dnSpy for the changes to show.");    
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
  -i [themes]       Install some built-in theme(s). Separate with space ("" ""). Mutually exclusive with -e flag.
  -e [themes]       Install all built-in theme(s) except these. Separate with space ("" ""). Mutually exclusive with -i flag.
  -f [paths]        Install your own theme(s) at these path(s). Separate with space ("" "").
  -p                Install ThemeHotReload plugin (https://github.com/HoLLy-HaCKeR/dnSpy.Extension.ThemeHotReload).
                      This plugin is **ONLY FOR** .NET 5.0 version of dnSpy 6.1.8.
   
   The -i and -e flags are mutually exclusive.
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

    private static string TrimOuterQuotes(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        switch (input[0])
        {
            case '"':
                return input.Trim('"');
            case '\'':
                return input.Trim('\'');
            default:
                return input;
        }
    }
}