using System;
using System.IO;
using static dnSpy.Theme.Installer.Utils.FileUtils;

namespace dnSpy.Theme.Installer;

public class ThemeHotReloadPluginInstaller
{
    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once InconsistentNaming
    public readonly string dnSpyPluginsDirectory;

    public ThemeHotReloadPluginInstaller(string dnSpyDirectory)
    {
        AssertDirectoryExists(dnSpyDirectory);
        
        dnSpyPluginsDirectory = Path.Combine(dnSpyDirectory, "bin", "Extensions");

        AssertDirectoryExists(dnSpyPluginsDirectory);
        AssertDirectoryExists(PluginDirectory);
    }

    public static string PluginDirectory =>
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ThemeHotReload_Plugin");

    public void InstallPlugin()
    {
        CopyFilesInDirectoryRecursive(PluginDirectory, dnSpyPluginsDirectory);       
    }
}