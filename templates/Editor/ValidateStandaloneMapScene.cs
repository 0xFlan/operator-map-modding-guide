using System;
using System.Collections.Generic;
using System.IO;
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
    private const string ExpectedPackageManifestPath =
        "../Package/operator-map-package.json";
    private const string ExpectedOperationId =
        "author.example-map.pve";
    private const string GameplayBoundsName = "GameplayBounds";
    private const float MarkerWallClearance = 2.0f;
    private const int MinimumPlayerMarkers = 1;

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
        var selectedOperation = LoadSelectedOperation(errors);

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
            if (collider == null)
            {
                errors.Add(
                    $"Terrain '{PathOf(terrain.transform)}' has no matching " +
                    "TerrainCollider on the same GameObject.");
                continue;
            }

            if (collider.terrainData != terrain.terrainData)
            {
                errors.Add(
                    $"Terrain '{PathOf(terrain.transform)}' and its collider " +
                    "do not use the same TerrainData.");
            }
        }

        if (terrains.Length == 0)
        {
            errors.Add("Scene has no Terrain.");
        }

        if (terrainColliders.Length == 0)
        {
            errors.Add("Scene has no TerrainCollider.");
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
        var enemyMarkers = transforms
            .Where(item => IsEnemyMarker(item.name))
            .OrderBy(item => item.name, StringComparer.Ordinal)
            .ToArray();
        var hvtMarkers = transforms
            .Where(item => IsHvtMarker(item.name))
            .OrderBy(item => item.name, StringComparer.Ordinal)
            .ToArray();
        var aiMarkers = enemyMarkers.Concat(hvtMarkers).ToArray();

        if (playerMarkers.Length < MinimumPlayerMarkers)
        {
            errors.Add(
                $"Scene has {playerMarkers.Length} player marker(s); " +
                $"minimum is {MinimumPlayerMarkers}.");
        }

        if (selectedOperation != null && selectedOperation.IsPve)
        {
            if (selectedOperation.minEnemies < 1)
            {
                errors.Add(
                    $"Selected PVE operation '{ExpectedOperationId}' has " +
                    $"invalid minEnemies={selectedOperation.minEnemies}.");
            }
            else if (enemyMarkers.Length < selectedOperation.minEnemies)
            {
                errors.Add(
                    $"Scene has {enemyMarkers.Length} ordinary enemy marker(s); " +
                    $"selected PVE operation minEnemies is " +
                    $"{selectedOperation.minEnemies}.");
            }
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
            $"Players={playerMarkers.Length}, Enemies={enemyMarkers.Length}, " +
            $"HVTs={hvtMarkers.Length}, Mode={selectedOperation?.mode ?? "unknown"}.");
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

    private static bool IsEnemyMarker(string name) =>
        name.StartsWith("PVE_EnemySpawn_", StringComparison.Ordinal);

    private static bool IsHvtMarker(string name) =>
        name.StartsWith("PVE_HVTSpawn_", StringComparison.Ordinal);

    private static bool ContainsWithClearance(
        BoxCollider box,
        Vector3 worldPoint,
        float clearance)
    {
        var local = box.transform.InverseTransformPoint(worldPoint) - box.center;
        var worldUnitsPerLocalX =
            box.transform.TransformVector(Vector3.right).magnitude;
        var worldUnitsPerLocalZ =
            box.transform.TransformVector(Vector3.forward).magnitude;
        if (worldUnitsPerLocalX <= Mathf.Epsilon ||
            worldUnitsPerLocalZ <= Mathf.Epsilon)
        {
            return false;
        }

        var localClearanceX = clearance / worldUnitsPerLocalX;
        var localClearanceZ = clearance / worldUnitsPerLocalZ;
        var half = box.size * 0.5f -
                   new Vector3(localClearanceX, 0f, localClearanceZ);
        if (half.x <= 0f || half.z <= 0f)
        {
            return false;
        }

        return Math.Abs(local.x) <= half.x &&
               Math.Abs(local.y) <= box.size.y * 0.5f &&
               Math.Abs(local.z) <= half.z;
    }

    private static PackageOperation LoadSelectedOperation(
        ICollection<string> errors)
    {
        var projectRoot = Directory.GetParent(Application.dataPath)?.FullName;
        if (string.IsNullOrEmpty(projectRoot))
        {
            errors.Add("Could not resolve the Unity project root.");
            return null;
        }

        var manifestPath = Path.IsPathRooted(ExpectedPackageManifestPath)
            ? Path.GetFullPath(ExpectedPackageManifestPath)
            : Path.GetFullPath(Path.Combine(
                projectRoot, ExpectedPackageManifestPath));
        if (!File.Exists(manifestPath))
        {
            errors.Add(
                $"Package manifest does not exist: '{manifestPath}'.");
            return null;
        }

        PackageManifest manifest;
        try
        {
            manifest = JsonUtility.FromJson<PackageManifest>(
                File.ReadAllText(manifestPath));
        }
        catch (Exception exception)
        {
            errors.Add(
                $"Package manifest could not be read: " +
                $"{exception.GetType().Name}: {exception.Message}");
            return null;
        }

        var maps = manifest?.maps ?? Array.Empty<PackageMap>();
        var sceneMatches = maps.Where(map =>
            map != null && string.Equals(
                map.scenePath,
                ExpectedScenePath,
                StringComparison.Ordinal)).ToArray();
        if (sceneMatches.Length != 1)
        {
            errors.Add(
                $"Expected exactly one map for scene " +
                $"'{ExpectedScenePath}'; found {sceneMatches.Length}.");
            return null;
        }

        var sceneMap = sceneMatches[0];
        var operations = sceneMap.operations ?? Array.Empty<PackageOperation>();
        var matches = operations.Where(operation =>
            operation != null && string.Equals(
                operation.operationId,
                ExpectedOperationId,
                StringComparison.Ordinal)).ToArray();
        if (matches.Length != 1)
        {
            errors.Add(
                $"Expected exactly one operation '{ExpectedOperationId}' for " +
                $"scene '{ExpectedScenePath}'; found {matches.Length}.");
            return null;
        }

        var selected = matches[0];
        if (!selected.IsPve && !selected.IsPvp)
        {
            errors.Add(
                $"Selected operation '{ExpectedOperationId}' has unsupported " +
                $"mode '{selected.mode}'.");
            return null;
        }

        return selected;
    }

    [Serializable]
    private sealed class PackageManifest
    {
        public PackageMap[] maps;
    }

    [Serializable]
    private sealed class PackageMap
    {
        public string scenePath;
        public PackageOperation[] operations;
    }

    [Serializable]
    private sealed class PackageOperation
    {
        public string operationId;
        public string mode;
        public int minEnemies;

        public bool IsPve =>
            string.Equals(mode, "pve", StringComparison.OrdinalIgnoreCase);

        public bool IsPvp =>
            string.Equals(mode, "pvp", StringComparison.OrdinalIgnoreCase);
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
