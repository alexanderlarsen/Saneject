using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace DevTools.Editor.VersionTool
{
    public static class VersionToolUtility
    {
        private const string FinalizationPendingKey = "Saneject.VersionTool.FinalizationPending";
        private const string VersionKey = "Saneject.VersionTool.Version";
        private const string PackagePathKey = "Saneject.VersionTool.PackagePath";
        private const string PackageRootAssetPath = "Assets/Plugins/Saneject";

        private static readonly string RepoRootPath = Path.Combine(Application.dataPath, "..", "..", "..");
        private static readonly string ChangelogPath = Path.Combine(RepoRootPath, "CHANGELOG.md");
        private static readonly string PackageJsonPath = Path.Combine(RepoRootPath, @"UnityProject\Saneject\Assets\Plugins\Saneject\package.json");
        private static readonly string SamplesPath = Path.Combine(RepoRootPath, @"UnityProject\Saneject\Assets\Plugins\Saneject\Samples");
        private static readonly string SamplesTildePath = SamplesPath + "~";

        public static void SetVersionAndExportUnityPackage(
            string version,
            string packagePath)
        {
            string releaseVersion = version.Trim();

            if (string.IsNullOrWhiteSpace(releaseVersion))
                return;

            if (string.IsNullOrWhiteSpace(packagePath))
                return;

            Debug.Log($"Starting release preparation for version {releaseVersion}");
            CheckChangelogVersion(releaseVersion);
            UpdatePackageJsonVersion(releaseVersion);
            PrepareUnityPackage(releaseVersion, packagePath);
        }

        public static string GetPackageJsonVersion()
        {
            try
            {
                if (!File.Exists(PackageJsonPath))
                    return string.Empty;

                string packageJson = File.ReadAllText(PackageJsonPath);

                if (string.IsNullOrWhiteSpace(packageJson))
                    return string.Empty;

                JObject packageJsonObj = JObject.Parse(packageJson);
                return packageJsonObj["version"]?.ToString() ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        [InitializeOnLoadMethod]
        private static void ResumeAfterDomainReload()
        {
            EditorApplication.delayCall += TryFinalize;
        }

        private static void CheckChangelogVersion(string releaseVersion)
        {
            if (!File.Exists(ChangelogPath))
                throw new FileNotFoundException("Changelog file not found at: " + ChangelogPath);

            string[] changelog = File.ReadAllLines(ChangelogPath);

            if (changelog.Length == 0)
            {
                EditorUtility.OpenWithDefaultApp(ChangelogPath);
                throw new InvalidOperationException("Changelog file is empty or contains only whitespace.");
            }

            if (!changelog.Any(changelogLine => changelogLine.Contains($"## Version {releaseVersion}")))
            {
                EditorUtility.OpenWithDefaultApp(ChangelogPath);
                throw new InvalidOperationException($"Changelog does not contain version {releaseVersion}.");
            }

            Debug.Log("Changelog version check passed.");
        }

        private static void UpdatePackageJsonVersion(string releaseVersion)
        {
            if (!File.Exists(PackageJsonPath))
                throw new FileNotFoundException("Package.json file not found at: " + PackageJsonPath);

            string packageJson = File.ReadAllText(PackageJsonPath);

            if (string.IsNullOrWhiteSpace(packageJson))
            {
                EditorUtility.OpenWithDefaultApp(PackageJsonPath);
                throw new InvalidOperationException("Package.json file is empty or contains only whitespace.");
            }

            JObject packageJsonObj = JObject.Parse(packageJson);

            if (packageJsonObj["version"] == null)
            {
                EditorUtility.OpenWithDefaultApp(PackageJsonPath);
                throw new InvalidOperationException("Package.json file does not contain a version.");
            }

            if (packageJsonObj["version"].ToString() == releaseVersion)
            {
                Debug.Log("Package.json version is already up-to-date.");
                return;
            }

            packageJsonObj["version"] = releaseVersion;
            File.WriteAllText(PackageJsonPath, packageJsonObj.ToString());
            Debug.Log($"Package.json version updated to {releaseVersion}.");
        }

        private static void PrepareUnityPackage(
            string releaseVersion,
            string packagePath)
        {
            if (!Directory.Exists(SamplesTildePath))
                throw new DirectoryNotFoundException("Samples folder not found at: " + SamplesTildePath);

            if (Directory.Exists(SamplesPath))
                throw new IOException("Cannot prepare release because Samples already exists at: " + SamplesPath);

            SessionState.SetBool(FinalizationPendingKey, true);
            SessionState.SetString(VersionKey, releaseVersion);
            SessionState.SetString(PackagePathKey, packagePath);

            Directory.Move(SamplesTildePath, SamplesPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("Samples folder renamed from Samples~ to Samples.");
        }

        private static void TryFinalize()
        {
            if (!SessionState.GetBool(FinalizationPendingKey, defaultValue: false))
                return;

            string releaseVersion = SessionState.GetString(VersionKey, string.Empty);
            string packagePath = SessionState.GetString(PackagePathKey, string.Empty);

            if (string.IsNullOrWhiteSpace(releaseVersion))
                throw new InvalidOperationException("Cannot resume release preparation because the version was not stored.");

            if (string.IsNullOrWhiteSpace(packagePath))
                throw new InvalidOperationException("Cannot resume release preparation because the package path was not stored.");

            try
            {
                AssetDatabase.ExportPackage(PackageRootAssetPath, packagePath, ExportPackageOptions.Recurse);
                Debug.Log($"Unity package created at: {packagePath}");
            }
            finally
            {
                if (Directory.Exists(SamplesPath) && !Directory.Exists(SamplesTildePath))
                    Directory.Move(SamplesPath, SamplesTildePath);

                File.Delete(SamplesPath + ".meta");
                Debug.Log("Samples folder renamed back from Samples to Samples~.");

                SessionState.EraseBool(FinalizationPendingKey);
                SessionState.EraseString(VersionKey);
                SessionState.EraseString(PackagePathKey);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            Debug.Log("Release preparation complete.");
        }
    }
}