using UnityEditor;
using UnityEngine;
using System.IO;

namespace HarfBuzzSharp {
  [InitializeOnLoad]
  public static class LinuxLoadResolver {
    static LinuxLoadResolver() {
      LoadHarfBuzzLibrary();
    }

    private static void LoadHarfBuzzLibrary() {
      // Get the path of the package
      string packagePath = GetPackagePath("com.jawaker.skiaunity");

      if (string.IsNullOrEmpty(packagePath)) {
        Debug.LogError("SkiaForUnity package not found in the Package Manager.");
        return;
      }

      string libraryPath = Path.Combine(packagePath, "Library", "Linux", "linux-x64", "native", "libHarfBuzzSharp.so");

      if (File.Exists(libraryPath)) {
        Debug.Log($"Loading HarfBuzz library from: {libraryPath}");
        var intPtr = LibraryLoader.LoadLibrary(libraryPath);
        Debug.LogError($"Loaded HarfBuzz library from: {libraryPath} for Linux");
      } else {
        Debug.LogError($"Library not found at path: {libraryPath}");
      }
    }

    private static string GetPackagePath(string packageName) {
      string packagePath = null;

      // Query the Package Manager for package info
      var request = UnityEditor.PackageManager.Client.List(true); // Refresh=true to get up-to-date data

      while (!request.IsCompleted) {
        System.Threading.Thread.Sleep(10);
      }

      if (request.Status == UnityEditor.PackageManager.StatusCode.Success) {
        foreach (var package in request.Result) {
          Debug.LogError($"Found package: {package.name}");
          if (package.name == packageName) {
            packagePath = package.resolvedPath;
            break;
          }
        }
      } else if (request.Status >= UnityEditor.PackageManager.StatusCode.Failure) {
        Debug.LogError($"Failed to list packages: {request.Error.message}");
      }

      return packagePath;
    }
  }
}