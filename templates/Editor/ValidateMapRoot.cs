// RETIRED FOR STANDALONE MISSIONS.
// This file validates the historical prefab-overlay root.
// Use ValidateStandaloneMapScene.cs for the current real-scene method.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

// Copy this file into Assets/Editor/, then edit the constants below for one map.
public static class ValidateMapRoot
{
    private const string PrefabAssetPath = "Assets/Maps/YourMap/Prefabs/YourMapRoot.prefab";
    private const string ReportDirectory = "Builds";

    // These are a starter contract, not OPERATOR-required names. Change them
    // together with your prefab hierarchy and runtime plugin expectations.
    private static readonly string[] RequiredChildRoots =
    {
        "GroundCollision",
        "DirectProps",
        "SpawnMarkers",
        "Bounds"
    };

    [MenuItem("OPERATOR Map/Validate Local Map Root")]
    public static void Validate()
    {
        if (AssetDatabase.LoadAssetAtPath<GameObject>(PrefabAssetPath) == null)
            throw new FileNotFoundException("Map prefab was not found: " + PrefabAssetPath);

        var root = PrefabUtility.LoadPrefabContents(PrefabAssetPath);
        try
        {
            var errors = new List<string>();
            var report = new StringBuilder();
            report.AppendLine("Prefab: " + PrefabAssetPath);

            foreach (var name in RequiredChildRoots)
            {
                if (FindDirectChild(root.transform, name) == null)
                    errors.Add("Missing required direct child root: " + name);
            }

            var groundRoot = FindDirectChild(root.transform, "GroundCollision");
            if (groundRoot != null && !groundRoot.GetComponentsInChildren<Collider>(true).Any(c => c.enabled))
                errors.Add("GroundCollision has no enabled Collider.");

            var boundsRoot = FindDirectChild(root.transform, "Bounds");
            if (boundsRoot != null && !boundsRoot.GetComponentsInChildren<Collider>(true).Any(c => c.enabled))
                errors.Add("Bounds has no enabled Collider.");

            var colliders = root.GetComponentsInChildren<Collider>(true);
            report.AppendLine("Collider count: " + colliders.Length);
            if (colliders.Length == 0)
                errors.Add("The map prefab has no colliders.");

            var renderers = root.GetComponentsInChildren<Renderer>(true);
            report.AppendLine("Renderer count: " + renderers.Length);
            foreach (var renderer in renderers)
            {
                var materials = renderer.sharedMaterials;
                for (var index = 0; index < materials.Length; index++)
                {
                    if (materials[index] == null)
                        errors.Add("Null material on " + renderer.name + " slot " + index + ".");
                }
            }

            var terrain = root.GetComponentInChildren<Terrain>(true);
            if (terrain != null)
            {
                report.AppendLine("TerrainData present: " + (terrain.terrainData != null));
                var terrainCollider = terrain.GetComponent<TerrainCollider>();
                report.AppendLine("TerrainCollider present: " + (terrainCollider != null));
                if (terrain.terrainData == null)
                    errors.Add("Terrain component has no TerrainData.");
                if (terrainCollider == null || terrainCollider.terrainData == null)
                    errors.Add("Terrain component has no bound TerrainCollider/TerrainData.");
            }

            var spawnRoot = FindDirectChild(root.transform, "SpawnMarkers");
            report.AppendLine("Spawn marker count: " + (spawnRoot == null ? 0 : spawnRoot.childCount));

            if (errors.Count == 0)
                report.AppendLine("Result: PASS");
            else
            {
                report.AppendLine("Result: FAIL");
                foreach (var error in errors)
                    report.AppendLine("ERROR: " + error);
            }

            Directory.CreateDirectory(ReportDirectory);
            var reportPath = Path.Combine(ReportDirectory, "your_map_bundle.validate.txt");
            File.WriteAllText(reportPath, report.ToString());

            if (errors.Count != 0)
                throw new InvalidOperationException("Map root validation failed. See " + reportPath);

            Debug.Log("Map root validation passed. Report: " + reportPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static Transform FindDirectChild(Transform root, string childName)
    {
        for (var index = 0; index < root.childCount; index++)
        {
            var child = root.GetChild(index);
            if (string.Equals(child.name, childName, StringComparison.Ordinal))
                return child;
        }

        return null;
    }
}
