using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ValidateStandaloneMapScene
{
    // Set these exact values for the package.
    private const string ExpectedScenePath =
        "Assets/Maps/Example/Scenes/Example.unity";
    private const string ExpectedMapMarker =
        "MAP_ID_author.example-map.example";
    private const string ExpectedSpawnSetMarker =
        "SPAWN_SET_default";
    private const string GameplayBoundsName = "GameplayBounds";
    private const float MarkerWallClearance = 2.0f;
    private const int MinimumPlayerMarkers = 1;
    private const int MinimumEnemyMarkers = 1;

    [MenuItem("Tools/OPERATOR/Validate Standalone Map Scene")]
    public static void Validate()
    {
        var errors = new List<string>();
        var scene = SceneManager.GetActiveScene();

        if (!scene.IsValid() || !scene.isLoaded)
        {
            throw new InvalidOperationException("No valid loaded scene.");
        }

        if (!string.Equals(
                scene.path, ExpectedScenePath, StringComparison.Ordinal))
        {
            errors.Add(
                $"Scene path is '{scene.path}', expected '{ExpectedScenePath}'.");
        }

        if (scene.isDirty)
        {
            errors.Add("Save the scene before validation.");
        }

        var roots = scene.GetRootGameObjects();
        var transforms = roots
            .SelectMany(root => root.GetComponentsInChildren<Transform>(true))
            .ToArray();
        var gameObjects = transforms.Select(item => item.gameObject).ToArray();

        RequireExactName(gameObjects, ExpectedMapMarker, errors);
        RequireExactName(gameObjects, ExpectedSpawnSetMarker, errors);

        var boundsObject = gameObjects.SingleOrDefault(
            item => string.Equals(
                item.name, GameplayBoundsName, StringComparison.Ordinal));
        var gameplayBounds = boundsObject == null
            ? null
            : boundsObject.GetComponent<BoxCollider>();

        if (gameplayBounds == null)
        {
            errors.Add(
                $"'{GameplayBoundsName}' needs one BoxCollider.");
        }

        var terrains = roots
            .SelectMany(root => root.GetComponentsInChildren<Terrain>(true))
            .ToArray();
        var terrainColliders = roots
            .SelectMany(root =>
                root.GetComponentsInChildren<TerrainCollider>(true))
            .ToArray();

        foreach (var terrain in terrains)
        {
            if (terrain.terrainData == null)
            {
                errors.Add($"Terrain '{PathOf(terrain.transform)}' has no data.");
                continue;
            }

            var collider = terrain.GetComponent<TerrainCollider>();
            if (collider != null &&
                collider.terrainData != terrain.terrainData)
            {
                errors.Add(
                    $"Terrain '{PathOf(terrain.transform)}' and its collider " +
                    "do not use the same TerrainData.");
            }
        }

        if (terrains.Length == 0 && terrainColliders.Length == 0)
        {
            errors.Add("Scene has no Terrain or TerrainCollider.");
        }

        var colliders = roots
            .SelectMany(root => root.GetComponentsInChildren<Collider>(true))
            .ToArray();
        if (colliders.Length == 0)
        {
            errors.Add("Scene has no Collider.");
        }

        var renderers = roots
            .SelectMany(root => root.GetComponentsInChildren<Renderer>(true))
            .ToArray();
        foreach (var renderer in renderers)
        {
            if (renderer.sharedMaterials.Any(material => material == null))
            {
                errors.Add(
                    $"Renderer '{PathOf(renderer.transform)}' has a null material.");
            }
        }

        var playerMarkers = transforms
            .Where(item => IsPlayerMarker(item.name))
            .OrderBy(item => item.name, StringComparer.Ordinal)
            .ToArray();
        var aiMarkers = transforms
            .Where(item => IsAiMarker(item.name))
            .OrderBy(item => item.name, StringComparer.Ordinal)
            .ToArray();

        if (playerMarkers.Length < MinimumPlayerMarkers)
        {
            errors.Add(
                $"Scene has {playerMarkers.Length} player marker(s); " +
                $"minimum is {MinimumPlayerMarkers}.");
        }

        if (aiMarkers.Length < MinimumEnemyMarkers)
        {
            errors.Add(
                $"Scene has {aiMarkers.Length} AI marker(s); " +
                $"minimum is {MinimumEnemyMarkers}.");
        }

        if (gameplayBounds != null)
        {
            foreach (var marker in aiMarkers)
            {
                if (!ContainsWithClearance(
                        gameplayBounds,
                        marker.position,
                        MarkerWallClearance))
                {
                    errors.Add(
                        $"AI marker '{PathOf(marker)}' is outside gameplay " +
                        $"bounds or its {MarkerWallClearance:F2}m clearance.");
                }
            }
        }

        var lights = roots
            .SelectMany(root => root.GetComponentsInChildren<Light>(true))
            .Where(light => light.type == LightType.Directional)
            .ToArray();
        if (lights.Length == 0)
        {
            errors.Add("Scene has no fallback Directional Light.");
        }

        if (errors.Count != 0)
        {
            foreach (var error in errors)
            {
                Debug.LogError(error);
            }

            throw new InvalidOperationException(
                $"Standalone scene validation failed with {errors.Count} error(s).");
        }

        Debug.Log(
            $"Standalone scene validation passed. Roots={roots.Length}, " +
            $"Renderers={renderers.Length}, Colliders={colliders.Length}, " +
            $"Players={playerMarkers.Length}, AI={aiMarkers.Length}.");
    }

    private static void RequireExactName(
        IEnumerable<GameObject> objects,
        string requiredName,
        ICollection<string> errors)
    {
        var count = objects.Count(item => string.Equals(
            item.name, requiredName, StringComparison.Ordinal));
        if (count != 1)
        {
            errors.Add(
                $"Expected exactly one '{requiredName}' object; found {count}.");
        }
    }

    private static bool IsPlayerMarker(string name) =>
        name.StartsWith("PVE_PlayerSpawn_", StringComparison.Ordinal) ||
        name.StartsWith("Team1_Spawn_", StringComparison.Ordinal) ||
        name.StartsWith("Team1_Backup_Spawn_", StringComparison.Ordinal) ||
        name.StartsWith("Team2_Spawn_", StringComparison.Ordinal) ||
        name.StartsWith("Team2_Backup_Spawn_", StringComparison.Ordinal) ||
        name.StartsWith("PVP_Team1Spawn_", StringComparison.Ordinal) ||
        name.StartsWith("PVP_Team2Spawn_", StringComparison.Ordinal);

    private static bool IsAiMarker(string name) =>
        name.StartsWith("PVE_EnemySpawn_", StringComparison.Ordinal) ||
        name.StartsWith("PVE_HVTSpawn_", StringComparison.Ordinal);

    private static bool ContainsWithClearance(
        BoxCollider box,
        Vector3 worldPoint,
        float clearance)
    {
        var local = box.transform.InverseTransformPoint(worldPoint) - box.center;
        var half = box.size * 0.5f - new Vector3(clearance, 0f, clearance);
        if (half.x <= 0f || half.z <= 0f)
        {
            return false;
        }

        return Math.Abs(local.x) <= half.x &&
               Math.Abs(local.y) <= box.size.y * 0.5f &&
               Math.Abs(local.z) <= half.z;
    }

    private static string PathOf(Transform item)
    {
        var parts = new Stack<string>();
        for (var current = item; current != null; current = current.parent)
        {
            parts.Push(current.name);
        }

        return string.Join("/", parts);
    }
}
