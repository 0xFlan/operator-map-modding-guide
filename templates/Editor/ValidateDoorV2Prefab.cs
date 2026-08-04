using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

// Copy this file into Assets/Editor in the map-authoring project. Keep the
// OPERATOR DoorV2 scripts and their dependencies resolved before validation.
// This validator uses serialized field names instead of compile-time private
// game types, so the file itself does not redistribute OPERATOR code.
public static class ValidateDoorV2Prefab
{
    private const string PrefabAssetPath = "Assets/Prefabs/_DoorV2_BASE.prefab";
    private const string ReportPath = "Builds/OperatorDoorValidation/doorv2-prefab-validation.txt";
    private const string OfficialPrefabGuid = "803422c907641034e99a99778ef7d30b";

    // Keep both switches true when validating the untouched developer source.
    // Set RequireOfficialSourceGuid=false only after creating an authorized
    // prefab variant with a new .meta GUID. Set RequireOfficialScalarValues=
    // false only when the variant specification intentionally changes a
    // scalar; all reference-graph checks still run.
    private const bool RequireOfficialSourceGuid = true;
    private const bool RequireOfficialScalarValues = true;

    private sealed class Audit
    {
        public readonly StringBuilder Text = new StringBuilder();
        public int Errors;
        public int Warnings;

        public void Pass(string message) => Text.AppendLine("PASS " + message);

        public void Error(string message)
        {
            Errors++;
            Text.AppendLine("ERROR " + message);
        }

        public void Warning(string message)
        {
            Warnings++;
            Text.AppendLine("WARN " + message);
        }
    }

    [MenuItem("OPERATOR Map/Validate DoorV2 Prefab")]
    public static void Validate()
    {
        var audit = new Audit();
        audit.Text.AppendLine("OPERATOR DoorV2 prefab validation");
        audit.Text.AppendLine("asset=" + PrefabAssetPath);
        audit.Text.AppendLine("unity=" + Application.unityVersion);

        var source = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabAssetPath);
        if (source == null)
        {
            audit.Error("Prefab asset was not found. Edit PrefabAssetPath in ValidateDoorV2Prefab.cs.");
            Finish(audit);
            return;
        }

        var guid = AssetDatabase.AssetPathToGUID(PrefabAssetPath);
        audit.Text.AppendLine("guid=" + guid);
        if (RequireOfficialSourceGuid && !string.Equals(guid, OfficialPrefabGuid, StringComparison.Ordinal))
            audit.Error("Official source GUID mismatch. Expected " + OfficialPrefabGuid + ".");
        else
            audit.Pass("Prefab GUID policy");

        GameObject root = null;
        try
        {
            root = PrefabUtility.LoadPrefabContents(PrefabAssetPath);
            if (root == null)
            {
                audit.Error("PrefabUtility.LoadPrefabContents returned null.");
                Finish(audit);
                return;
            }

            ValidateRoot(root, audit);
            ValidateGraph(root, audit);
        }
        catch (Exception exception)
        {
            audit.Error("Validator exception: " + exception.GetType().Name + ": " + exception.Message);
        }
        finally
        {
            if (root != null)
                PrefabUtility.UnloadPrefabContents(root);
        }

        Finish(audit);
    }

    private static void ValidateRoot(GameObject root, Audit audit)
    {
        if (root.activeSelf)
            audit.Error("The source root must be inactive while its complete reference graph is inspected or cloned.");
        else
            audit.Pass("Source root is inactive");

        try
        {
            if (!string.Equals(root.tag, "Door", StringComparison.Ordinal))
                audit.Error("Root tag must be Door; actual=" + root.tag + ".");
            else
                audit.Pass("Root tag=Door");
        }
        catch (UnityException)
        {
            audit.Error("The authoring project does not define the Door tag. Import the compatible TagManager contract first.");
        }

        var missingScripts = 0;
        foreach (var item in root.GetComponentsInChildren<Transform>(true))
            missingScripts += GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(item.gameObject);
        if (missingScripts != 0)
            audit.Error("Missing MonoBehaviour scripts=" + missingScripts + ". Do not save or build this prefab.");
        else
            audit.Pass("No missing MonoBehaviour scripts");

        RequireComponentCount(root, "NetworkIdentity", 1, audit);
        RequireComponentCount(root, "DoorV2", 1, audit);
        RequireComponentCount(root, "DoorHandleV2", 2, audit);
        RequireComponentCount(root, "DoorHitBox", 1, audit);
        RequireComponentCount(root, "ShootableDoorPart", 3, audit);
        RequireComponentCount(root, "MilkRigidbodySync", 1, audit);
        RequireComponentCount(root, "NavmeshCut", 1, audit);
        RequireComponentCount(root, "NodeLink2", 2, audit);
    }

    private static void ValidateGraph(GameObject root, Audit audit)
    {
        var door = FindComponents(root, "DoorV2").SingleOrDefault();
        var handles = FindComponents(root, "DoorHandleV2");
        if (door == null || handles.Count != 2)
            return;

        var doorData = new SerializedObject(door);
        doorData.Update();

        var pivot = RequireObjectReference<Transform>(doorData, "PivotTransform", root, true, audit);
        var front = RequireNamedComponentReference(doorData, "HandleFront", "DoorHandleV2", root, audit);
        var back = RequireNamedComponentReference(doorData, "HandleBack", "DoorHandleV2", root, audit);
        RequireObjectReference<GameObject>(doorData, "DoorModelParent", root, true, audit);
        var rigidbody = RequireObjectReference<Rigidbody>(doorData, "rb", root, true, audit);
        RequireAssetReference(doorData, "DoorPhysicsMaterial", audit);
        RequireNamedComponentReference(doorData, "DoorPhysicsSync", "MilkRigidbodySync", root, audit);
        RequireObjectReference<BoxCollider>(doorData, "DoorHitBox", root, true, audit);
        RequireObjectReference<BoxCollider>(doorData, "latchCollider", root, true, audit);
        RequireObjectReference<BoxCollider>(doorData, "HingeTopCollider", root, true, audit);
        RequireObjectReference<BoxCollider>(doorData, "HingeBottomCollider", root, true, audit);
        RequireNamedComponentReference(doorData, "NavMeshCut", "NavmeshCut", root, audit);
        var openableLink = RequireNamedComponentReference(doorData, "DoorOpenableNavLink", "NodeLink2", root, audit);
        var walkableLink = RequireNamedComponentReference(doorData, "DoorWalkableNavLink", "NodeLink2", root, audit);
        RequireObjectReference<AudioSource>(doorData, "audioSource", root, true, audit);

        if (front == null || back == null || front == back)
            audit.Error("HandleFront and HandleBack must be distinct DoorHandleV2 components.");
        else if (!handles.Contains(front) || !handles.Contains(back))
            audit.Error("HandleFront or HandleBack does not resolve to one of the two components in this prefab graph.");
        else
            audit.Pass("Door owns two distinct local handle components");

        if (openableLink == null || walkableLink == null || openableLink == walkableLink)
            audit.Error("DoorOpenableNavLink and DoorWalkableNavLink must be distinct NodeLink2 components.");
        else
            audit.Pass("Openable and walkable links are distinct");

        if (pivot != null)
        {
            RequireDescendant(pivot, rigidbody == null ? null : rigidbody.transform, "rb", audit);
            RequireDescendant(pivot, front == null ? null : front.transform, "HandleFront", audit);
            RequireDescendant(pivot, back == null ? null : back.transform, "HandleBack", audit);
        }

        foreach (var field in new[] { "doorUnlock", "doorLocked", "doorClose", "doorThud", "doorBreach" })
            RequireNonEmptyObjectArray(doorData, field, audit);

        var destroyedDoor = FindProperty(doorData, "DestroyedDoor", audit);
        if (destroyedDoor != null && destroyedDoor.objectReferenceValue != null)
        {
            if (!IsInsideRoot(destroyedDoor.objectReferenceValue, root))
                audit.Error("DestroyedDoor points outside this prefab graph.");
            RequireNonEmptyObjectArray(doorData, "DestroyedDoorRB", audit);
        }
        else
        {
            audit.Warning("DestroyedDoor is null. This is allowed only for a documented non-destructible variant.");
        }

        if (RequireOfficialScalarValues)
        {
            RequireInt(doorData, "DoorMask", 4545, audit);
            RequireInt(doorData, "PlayerMovementLayerMask", 33554436, audit);
            RequireFloat(doorData, "maxRotationY", 110f, audit);
            RequireFloat(doorData, "Damping", 0.5f, audit);
        }

        ValidateHandle(front, back, true, door, root, audit);
        ValidateHandle(back, front, false, door, root, audit);

        audit.Warning("Editor validation cannot prove A* endpoint graph attachment, Mirror replication, damage, interaction IK, or restart cleanup. Run the live matrix in docs/09-interactive-prefabs-and-doorsv2.md.");
    }

    private static void ValidateHandle(
        Component handle,
        Component rival,
        bool expectedFront,
        Component door,
        GameObject root,
        Audit audit)
    {
        if (handle == null)
            return;
        var data = new SerializedObject(handle);
        data.Update();
        var owner = RequireNamedComponentReference(data, "doorV2", "DoorV2", root, audit);
        var rivalReference = RequireNamedComponentReference(data, "RivalDoorHandle", "DoorHandleV2", root, audit);
        RequireLocalObjectReference(data, "myPushObject", root, audit);
        RequireLocalObjectReference(data, "Handle", root, audit);
        RequireLocalObjectReference(data, "Center", root, audit);
        RequireNullObjectReference(data, "MyLocalPlayer", audit);
        RequireNullObjectReference(data, "PlayerInteractionSystem", audit);

        if (owner != door)
            audit.Error(Path(handle) + ".doorV2 does not point to the owning DoorV2.");
        if (rivalReference != rival)
            audit.Error(Path(handle) + ".RivalDoorHandle is not reciprocal.");

        var front = FindProperty(data, "IsFrontHandle", audit);
        if (front != null && front.boolValue != expectedFront)
            audit.Error(Path(handle) + ".IsFrontHandle expected=" + expectedFront + ".");
        else if (front != null)
            audit.Pass(Path(handle) + " front/back role=" + expectedFront);
    }

    private static void RequireComponentCount(GameObject root, string typeName, int expected, Audit audit)
    {
        var actual = FindComponents(root, typeName).Count;
        if (actual != expected)
            audit.Error(typeName + " count expected=" + expected + ", actual=" + actual + ".");
        else
            audit.Pass(typeName + " count=" + expected);
    }

    private static List<Component> FindComponents(GameObject root, string typeName)
    {
        var result = new List<Component>();
        foreach (var item in root.GetComponentsInChildren<Transform>(true))
        {
            foreach (var component in item.GetComponents<Component>())
            {
                if (component != null && string.Equals(component.GetType().Name, typeName, StringComparison.Ordinal))
                    result.Add(component);
            }
        }
        return result;
    }

    private static T RequireObjectReference<T>(
        SerializedObject owner,
        string field,
        GameObject root,
        bool requireInsideRoot,
        Audit audit) where T : UnityEngine.Object
    {
        var property = FindProperty(owner, field, audit);
        if (property == null)
            return null;
        var value = property.objectReferenceValue;
        if (!(value is T typed))
        {
            audit.Error(owner.targetObject.GetType().Name + "." + field + " must reference " + typeof(T).Name + ".");
            return null;
        }
        if (requireInsideRoot && !IsInsideRoot(typed, root))
        {
            audit.Error(owner.targetObject.GetType().Name + "." + field + " points outside this prefab graph.");
            return null;
        }
        audit.Pass(owner.targetObject.GetType().Name + "." + field + " -> " + Path(typed));
        return typed;
    }

    private static Component RequireNamedComponentReference(
        SerializedObject owner,
        string field,
        string expectedTypeName,
        GameObject root,
        Audit audit)
    {
        var property = FindProperty(owner, field, audit);
        if (property == null)
            return null;
        var value = property.objectReferenceValue as Component;
        if (value == null || !string.Equals(value.GetType().Name, expectedTypeName, StringComparison.Ordinal))
        {
            audit.Error(owner.targetObject.GetType().Name + "." + field + " must reference " + expectedTypeName + ".");
            return null;
        }
        if (!IsInsideRoot(value, root))
        {
            audit.Error(owner.targetObject.GetType().Name + "." + field + " points outside this prefab graph.");
            return null;
        }
        audit.Pass(owner.targetObject.GetType().Name + "." + field + " -> " + Path(value));
        return value;
    }

    private static void RequireAssetReference(SerializedObject owner, string field, Audit audit)
    {
        var property = FindProperty(owner, field, audit);
        if (property == null)
            return;
        if (property.objectReferenceValue == null)
            audit.Error(owner.targetObject.GetType().Name + "." + field + " is null.");
        else
            audit.Pass(owner.targetObject.GetType().Name + "." + field + " -> " + AssetDatabase.GetAssetPath(property.objectReferenceValue));
    }

    private static void RequireLocalObjectReference(SerializedObject owner, string field, GameObject root, Audit audit)
    {
        var property = FindProperty(owner, field, audit);
        if (property == null)
            return;
        if (property.objectReferenceValue == null)
            audit.Error(owner.targetObject.GetType().Name + "." + field + " is null.");
        else if (!IsInsideRoot(property.objectReferenceValue, root))
            audit.Error(owner.targetObject.GetType().Name + "." + field + " points outside this prefab graph.");
        else
            audit.Pass(owner.targetObject.GetType().Name + "." + field + " -> " + Path(property.objectReferenceValue));
    }

    private static void RequireNullObjectReference(SerializedObject owner, string field, Audit audit)
    {
        var property = FindProperty(owner, field, audit);
        if (property == null)
            return;
        if (property.objectReferenceValue != null)
            audit.Error(owner.targetObject.GetType().Name + "." + field + " is runtime state and must be null in the source prefab.");
        else
            audit.Pass(owner.targetObject.GetType().Name + "." + field + " is null runtime state");
    }

    private static void RequireNonEmptyObjectArray(SerializedObject owner, string field, Audit audit)
    {
        var property = FindProperty(owner, field, audit);
        if (property == null)
            return;
        if (!property.isArray || property.arraySize == 0)
        {
            audit.Error(owner.targetObject.GetType().Name + "." + field + " must be a non-empty array.");
            return;
        }
        var nullEntries = 0;
        for (var index = 0; index < property.arraySize; index++)
        {
            if (property.GetArrayElementAtIndex(index).objectReferenceValue == null)
                nullEntries++;
        }
        if (nullEntries != 0)
            audit.Error(owner.targetObject.GetType().Name + "." + field + " contains null entries=" + nullEntries + ".");
        else
            audit.Pass(owner.targetObject.GetType().Name + "." + field + " entries=" + property.arraySize);
    }

    private static void RequireInt(SerializedObject owner, string field, int expected, Audit audit)
    {
        var property = FindProperty(owner, field, audit);
        if (property == null)
            return;
        if (property.intValue != expected)
            audit.Error(owner.targetObject.GetType().Name + "." + field + " expected=" + expected + ", actual=" + property.intValue + ".");
        else
            audit.Pass(owner.targetObject.GetType().Name + "." + field + "=" + expected);
    }

    private static void RequireFloat(SerializedObject owner, string field, float expected, Audit audit)
    {
        var property = FindProperty(owner, field, audit);
        if (property == null)
            return;
        if (Mathf.Abs(property.floatValue - expected) > 0.0001f)
            audit.Error(owner.targetObject.GetType().Name + "." + field + " expected=" + expected + ", actual=" + property.floatValue + ".");
        else
            audit.Pass(owner.targetObject.GetType().Name + "." + field + "=" + expected);
    }

    private static SerializedProperty FindProperty(SerializedObject owner, string field, Audit audit)
    {
        var property = owner.FindProperty(field);
        if (property == null)
            audit.Error(owner.targetObject.GetType().Name + " has no serialized field named " + field + ". Check the exact OPERATOR build and script import.");
        return property;
    }

    private static void RequireDescendant(Transform ancestor, Transform child, string label, Audit audit)
    {
        if (ancestor == null || child == null)
            return;
        if (child != ancestor && !child.IsChildOf(ancestor))
            audit.Error(label + " must move under PivotTransform.");
        else
            audit.Pass(label + " is under PivotTransform");
    }

    private static bool IsInsideRoot(UnityEngine.Object value, GameObject root)
    {
        Transform transform = null;
        if (value is GameObject gameObject)
            transform = gameObject.transform;
        else if (value is Component component)
            transform = component.transform;
        return transform != null && (transform == root.transform || transform.IsChildOf(root.transform));
    }

    private static string Path(UnityEngine.Object value)
    {
        Transform transform = null;
        if (value is GameObject gameObject)
            transform = gameObject.transform;
        else if (value is Component component)
            transform = component.transform;
        if (transform == null)
            return value == null ? "null" : value.name;
        var parts = new List<string>();
        for (var item = transform; item != null; item = item.parent)
            parts.Add(item.name);
        parts.Reverse();
        return string.Join("/", parts);
    }

    private static void Finish(Audit audit)
    {
        audit.Text.AppendLine("SUMMARY errors=" + audit.Errors + ", warnings=" + audit.Warnings + ".");
        var absoluteReportPath = System.IO.Path.GetFullPath(ReportPath);
        Directory.CreateDirectory(System.IO.Path.GetDirectoryName(absoluteReportPath));
        File.WriteAllText(absoluteReportPath, audit.Text.ToString(), new UTF8Encoding(false));
        AssetDatabase.Refresh();
        Debug.Log(audit.Text.ToString() + "Report=" + absoluteReportPath);
        if (audit.Errors != 0)
            throw new BuildFailedException("DoorV2 prefab validation failed with " + audit.Errors + " error(s). Report=" + absoluteReportPath);
    }
}
