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
        private const string ChangelogPath = @"E:\Unity\Personal\Saneject\CHANGELOG.md";
        private const string PackageJsonPath = @"E:\Unity\Personal\Saneject\UnityProject\Saneject\Assets\Plugins\Saneject\package.json";
        private const string PackageRootAssetPath = "Assets/Plugins/Saneject";
        private const string SamplesTildePath = @"E:\Unity\Personal\Saneject\UnityProject\Saneject\Assets\Plugins\Saneject\Samples~";
        private const string SamplesPath = @"E:\Unity\Personal\Saneject\UnityProject\Saneject\Assets\Plugins\Saneject\Samples";
        private const string PackageExportPath = @"C:\Users\Alexander\Desktop";
        private const string ReleasePreparationStepKey = "Saneject.ReleasePreparation.Step";
        private const string ReleasePreparationVersionKey = "Saneject.ReleasePreparation.Version";
        private const string ExportAfterSamplesRenameStep = "ExportAfterSamplesRename";

        public static void SetVersionAndExportUnityPackage(string version)
        {
            string releaseVersion = version.Trim();

            if (string.IsNullOrWhiteSpace(releaseVersion))
                return;

            Debug.Log($"Starting release preparation for version {releaseVersion}");
            CheckChangelogVersion(releaseVersion);
            UpdatePackageJsonVersion(releaseVersion);
            PrepareUnityPackage(releaseVersion);
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
            EditorApplication.delayCall += TryFinish;
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

        private static void PrepareUnityPackage(string releaseVersion)
        {
            if (!Directory.Exists(SamplesTildePath))
                throw new DirectoryNotFoundException("Samples folder not found at: " + SamplesTildePath);

            if (Directory.Exists(SamplesPath))
                throw new IOException("Cannot prepare release because Samples already exists at: " + SamplesPath);

            SessionState.SetString(ReleasePreparationStepKey, ExportAfterSamplesRenameStep);
            SessionState.SetString(ReleasePreparationVersionKey, releaseVersion);

            Directory.Move(SamplesTildePath, SamplesPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Samples folder renamed from Samples~ to Samples.");

            TryFinish();
        }

        private static void TryFinish()
        {
            if (SessionState.GetString(ReleasePreparationStepKey, string.Empty) != ExportAfterSamplesRenameStep)
                return;

            string releaseVersion = SessionState.GetString(ReleasePreparationVersionKey, string.Empty);

            if (string.IsNullOrWhiteSpace(releaseVersion))
                throw new InvalidOperationException("Cannot resume release preparation because the version was not stored.");

            try
            {
                string packagePath = Path.Combine(PackageExportPath, $"Saneject-{releaseVersion}.unitypackage");
                AssetDatabase.ExportPackage(PackageRootAssetPath, packagePath, ExportPackageOptions.Recurse);
                Debug.Log($"Saneject-{releaseVersion}.unitypackage created.");
            }
            finally
            {
                if (Directory.Exists(SamplesPath) && !Directory.Exists(SamplesTildePath))
                    Directory.Move(SamplesPath, SamplesTildePath);

                File.Delete(SamplesPath + ".meta");
                Debug.Log("Samples folder renamed back from Samples to Samples~.");

                SessionState.EraseString(ReleasePreparationStepKey);
                SessionState.EraseString(ReleasePreparationVersionKey);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            Debug.Log("Release preparation complete.");
        }
    }
}
