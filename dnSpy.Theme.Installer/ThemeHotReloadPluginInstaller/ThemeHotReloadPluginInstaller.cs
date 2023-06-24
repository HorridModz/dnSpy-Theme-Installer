using System;
using System.IO;
using static dnSpy.Theme.Installer.Utils.FileUtils;

namespace dnSpy.Theme.Installer;

public class ThemeHotReloadPluginInstaller
{
    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once InconsistentNaming
    public string dnSpyPluginsDirectory { get; }
    
    public static string PluginDirectory { get; } =
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ThemeHotReload_Plugin");

    public ThemeHotReloadPluginInstaller(string dnSpyDirectory)
    {
        AssertDirectoryExists(dnSpyDirectory);

        dnSpyPluginsDirectory = Path.Combine(dnSpyDirectory, "bin", "Extensions");
        AssertDirectoryExists(PluginDirectory);
    }

    public void InstallPlugin()
    {
        Directory.CreateDirectory(Path.Combine(dnSpyPluginsDirectory, "dnSpy.Extension.ThemeHotReload"));
        CopyFilesInDirectoryRecursive(PluginDirectory, 
            Path.Combine(dnSpyPluginsDirectory, "dnSpy.Extension.ThemeHotReload"));       
    }
}