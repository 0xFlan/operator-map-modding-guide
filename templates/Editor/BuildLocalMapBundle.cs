// RETIRED FOR STANDALONE MISSIONS.
// This file builds the historical prefab-overlay bundle.
// Use BuildStandaloneMapBundles.cs for the current real-scene method.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

// Copy this file into Assets/Editor/, then edit the constants below for one map.
public static class BuildLocalMapBundle
{
    private const string PrefabAssetPath = "Assets/Maps/YourMap/Prefabs/YourMapRoot.prefab";
    private const string BundleName = "your_map_bundle";
    private const string OutputDirectory = "Builds";

    // Add only paths that Unity does not discover through the prefab dependency graph.
    // Examples: a material/texture closure used by runtime material repair, or a
    // readable height/surface payload reconstructed by the runtime plugin.
    private static readonly string[] ExplicitClosureAssetPaths = new string[0];

    [MenuItem("OPERATOR Map/Build Local Map Bundle")]
    public static void Build()
    {
        RequireAsset(PrefabAssetPath, typeof(GameObject));

        var assetPaths = new List<string> { PrefabAssetPath };
        foreach (var path in ExplicitClosureAssetPaths)
        {
            RequireAsset(path, typeof(UnityEngine.Object));
            if (!assetPaths.Contains(path, StringComparer.Ordinal))
                assetPaths.Add(path);
        }

        var outputPath = Path.GetFullPath(OutputDirectory);
        Directory.CreateDirectory(outputPath);

        var build = new AssetBundleBuild
        {
            assetBundleName = BundleName,
            assetNames = assetPaths.ToArray()
        };

        var manifest = BuildPipeline.BuildAssetBundles(
            outputPath,
            new[] { build },
            BuildAssetBundleOptions.StrictMode,
            BuildTarget.StandaloneWindows64);

        if (manifest == null)
            throw new InvalidOperationException("Unity did not return an AssetBundle manifest.");

        var bundlePath = Path.Combine(outputPath, BundleName);
        VerifyBundle(bundlePath);
        WriteBuildNote(bundlePath, assetPaths);
        Debug.Log("Built and verified local map bundle: " + bundlePath);
    }

    private static void RequireAsset(string assetPath, Type requiredType)
    {
        if (string.IsNullOrWhiteSpace(assetPath))
            throw new InvalidOperationException("An AssetBundle input path is empty.");

        var asset = AssetDatabase.LoadAssetAtPath(assetPath, requiredType);
        if (asset == null)
            throw new FileNotFoundException("Required Unity asset was not found: " + assetPath);
    }

    private static void VerifyBundle(string bundlePath)
    {
        if (!File.Exists(bundlePath))
            throw new FileNotFoundException("Unity did not emit the expected bundle: " + bundlePath);

        var bundle = AssetBundle.LoadFromFile(bundlePath);
        if (bundle == null)
            throw new InvalidOperationException("Unity emitted a bundle that cannot be opened: " + bundlePath);

        try
        {
            var expectedPath = PrefabAssetPath.ToLowerInvariant();
            var names = bundle.GetAllAssetNames();
            if (!names.Contains(expectedPath, StringComparer.Ordinal))
                throw new InvalidOperationException(
                    "Bundle is missing the configured prefab asset path: " + expectedPath);

            var root = bundle.LoadAsset<GameObject>(expectedPath);
            if (root == null)
                throw new InvalidOperationException(
                    "Bundle contains the prefab path but it does not load as a GameObject: " + expectedPath);
        }
        finally
        {
            bundle.Unload(false);
        }
    }

    private static void WriteBuildNote(string bundlePath, IList<string> assetPaths)
    {
        var notePath = bundlePath + ".build.txt";
        File.WriteAllLines(notePath, new[]
        {
            "Bundle: " + BundleName,
            "Target: StandaloneWindows64",
            "Prefab asset path: " + PrefabAssetPath.ToLowerInvariant(),
            "Explicit closure count: " + (assetPaths.Count - 1),
            "Built UTC: " + DateTime.UtcNow.ToString("O")
        });
    }
}
