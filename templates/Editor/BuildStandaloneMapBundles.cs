using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class BuildStandaloneMapBundles
{
    // Set these values for the map. Keep bundle names lowercase.
    private const string OutputDirectory = "Build/StandaloneMap";
    private const string DependencyBundleName = "example_map_assets";
    private const string SceneBundleName = "example_map_scene";
    private const string ScenePath =
        "Assets/Maps/Example/Scenes/Example.unity";

    // Add every direct portable asset or runtime payload that Unity cannot
    // discover safely through the scene dependency closure.
    private static readonly string[] ExplicitDependencyAssets =
    {
        "Assets/Maps/Example/RuntimePayload/terrain-height.bytes",
        "Assets/Maps/Example/RuntimePayload/terrain-weights.bytes",
    };

    [MenuItem("Tools/OPERATOR/Build Standalone Map Bundles")]
    public static void Build()
    {
        RequireWindowsTarget();
        RequireAsset(ScenePath);

        var dependencyAssets = new HashSet<string>(
            AssetDatabase.GetDependencies(ScenePath, true)
                .Where(path => !path.EndsWith(
                    ".cs", StringComparison.OrdinalIgnoreCase))
                .Where(path => !path.EndsWith(
                    ".unity", StringComparison.OrdinalIgnoreCase)),
            StringComparer.Ordinal);

        foreach (var path in ExplicitDependencyAssets)
        {
            RequireAsset(path);
            dependencyAssets.Add(path);
        }

        Directory.CreateDirectory(OutputDirectory);

        var builds = new[]
        {
            new AssetBundleBuild
            {
                assetBundleName = DependencyBundleName,
                assetNames = dependencyAssets
                    .OrderBy(path => path, StringComparer.Ordinal)
                    .ToArray(),
            },
            new AssetBundleBuild
            {
                assetBundleName = SceneBundleName,
                assetNames = new[] { ScenePath },
            },
        };

        var manifest = BuildPipeline.BuildAssetBundles(
            OutputDirectory,
            builds,
            BuildAssetBundleOptions.StrictMode |
            BuildAssetBundleOptions.DeterministicAssetBundle,
            BuildTarget.StandaloneWindows64);

        if (manifest == null)
        {
            throw new InvalidOperationException(
                "AssetBundle build returned no manifest.");
        }

        VerifyBundle(
            Path.Combine(OutputDirectory, DependencyBundleName),
            expectedScenePath: null);
        VerifyBundle(
            Path.Combine(OutputDirectory, SceneBundleName),
            ScenePath);

        Debug.Log(
            $"Built dependency '{DependencyBundleName}' before scene " +
            $"'{SceneBundleName}'. Declared scene: '{ScenePath}'.");
    }

    private static void RequireWindowsTarget()
    {
        if (EditorUserBuildSettings.activeBuildTarget !=
            BuildTarget.StandaloneWindows64)
        {
            throw new InvalidOperationException(
                "Select Windows x86-64 before the bundle build.");
        }
    }

    private static void RequireAsset(string path)
    {
        if (string.IsNullOrWhiteSpace(AssetDatabase.AssetPathToGUID(path)))
        {
            throw new FileNotFoundException(
                $"Required Unity asset does not exist: {path}");
        }
    }

    private static void VerifyBundle(
        string bundlePath,
        string expectedScenePath)
    {
        var fullPath = Path.GetFullPath(bundlePath);
        var bundle = AssetBundle.LoadFromFile(fullPath);
        if (bundle == null)
        {
            throw new InvalidOperationException(
                $"Unity could not load emitted bundle: {fullPath}");
        }

        try
        {
            var scenes = bundle.GetAllScenePaths()
                .OrderBy(path => path, StringComparer.Ordinal)
                .ToArray();

            if (expectedScenePath == null)
            {
                if (scenes.Length != 0)
                {
                    throw new InvalidOperationException(
                        "A dependency bundle contains a scene: " +
                        string.Join(", ", scenes));
                }

                return;
            }

            if (scenes.Length != 1 || !string.Equals(
                    scenes[0], expectedScenePath, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    $"Scene bundle must contain exactly '{expectedScenePath}'. " +
                    "Actual: " + string.Join(", ", scenes));
            }
        }
        finally
        {
            bundle.Unload(true);
        }
    }
}
