using System.IO;

namespace dnSpy.Theme.Installer.Utils;

public static class FileUtils
{
    public static void AssertDirectoryExists(string directory)
    {
        if (!Directory.Exists(directory))
        {
            throw new DirectoryNotFoundException($"Directory '{directory}' does not exist.");
        }
    }

    public static void CopyFilesInDirectoryRecursive(string sourceDir, string targetDir)
    {
        // From https://stackoverflow.com/a/7146097/22081657
        
        Directory.CreateDirectory(targetDir);

        foreach (var file in Directory.GetFiles(sourceDir))
            File.Copy(file, Path.Combine(targetDir, Path.GetFileName(file)));

        foreach (var directory in Directory.GetDirectories(sourceDir))
            CopyFilesInDirectoryRecursive(directory,
                Path.Combine(targetDir, Path.GetFileName(directory)));
    }
}