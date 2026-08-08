using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class LeafMeshCombiner : EditorWindow
{
    private GameObject leafRoot;

    [MenuItem("Tools/Performance/Combine Leaf Meshes")]
    public static void ShowWindow()
    {
        GetWindow<LeafMeshCombiner>("Leaf Mesh Combiner");
    }

    private void OnGUI()
    {
        GUILayout.Label("Leaf Mesh Combiner", EditorStyles.boldLabel);

        EditorGUILayout.Space();

        leafRoot = (GameObject)EditorGUILayout.ObjectField(
            "Leaf Root",
            leafRoot,
            typeof(GameObject),
            true
        );

        EditorGUILayout.Space();

        if (GUILayout.Button("Combine Leaves"))
        {
            if (leafRoot == null)
            {
                Debug.LogError("Bitte zuerst ein Leaf Root auswählen.");
                return;
            }

            CombineLeaves();
        }
    }

    private void CombineLeaves()
    {
        MeshFilter[] meshFilters =
            leafRoot.GetComponentsInChildren<MeshFilter>(false);

        if (meshFilters.Length == 0)
        {
            Debug.LogError("Keine MeshFilter gefunden.");
            return;
        }

        List<CombineInstance> combineInstances =
            new List<CombineInstance>();

        Material leafMaterial = null;

        foreach (MeshFilter meshFilter in meshFilters)
        {
            if (meshFilter.sharedMesh == null)
                continue;

            MeshRenderer renderer =
                meshFilter.GetComponent<MeshRenderer>();

            if (renderer == null)
                continue;

            // Das neue Mesh wird relativ zum LeafRoot aufgebaut.
            CombineInstance combine = new CombineInstance();

            combine.mesh = meshFilter.sharedMesh;

            combine.transform =
                leafRoot.transform.worldToLocalMatrix *
                meshFilter.transform.localToWorldMatrix;

            combineInstances.Add(combine);

            if (leafMaterial == null)
                leafMaterial = renderer.sharedMaterial;
        }

        if (combineInstances.Count == 0)
        {
            Debug.LogError(
                "Keine gültigen Leaf Meshes zum Kombinieren gefunden."
            );

            return;
        }

        GameObject combinedObject =
            new GameObject("CombinedLeaves");

        Undo.RegisterCreatedObjectUndo(
            combinedObject,
            "Create Combined Leaves"
        );

        combinedObject.transform.SetParent(
            leafRoot.transform,
            false
        );

        combinedObject.transform.localPosition = Vector3.zero;
        combinedObject.transform.localRotation = Quaternion.identity;
        combinedObject.transform.localScale = Vector3.one;

        MeshFilter combinedFilter =
            combinedObject.AddComponent<MeshFilter>();

        MeshRenderer combinedRenderer =
            combinedObject.AddComponent<MeshRenderer>();

        Mesh combinedMesh = new Mesh();

        combinedMesh.name =
            leafRoot.name + "_CombinedLeaves";

        // Falls die Blätter zusammen über 65k Vertices kommen.
        combinedMesh.indexFormat =
            UnityEngine.Rendering.IndexFormat.UInt32;

        combinedMesh.CombineMeshes(
            combineInstances.ToArray(),
            true,
            true
        );

        combinedMesh.RecalculateBounds();

        combinedFilter.sharedMesh = combinedMesh;
        combinedRenderer.sharedMaterial = leafMaterial;

        // Leaf Shadows hatten wir ohnehin deaktiviert.
        combinedRenderer.shadowCastingMode =
            UnityEngine.Rendering.ShadowCastingMode.Off;

        combinedRenderer.receiveShadows = false;

        // Mesh als Asset speichern, damit es nach Editor-Neustart erhalten bleibt.
        string folder = "Assets/CombinedMeshes";

        if (!AssetDatabase.IsValidFolder(folder))
        {
            AssetDatabase.CreateFolder(
                "Assets",
                "CombinedMeshes"
            );
        }

        string meshPath =
            AssetDatabase.GenerateUniqueAssetPath(
                folder + "/" +
                leafRoot.name +
                "_CombinedLeaves.asset"
            );

        AssetDatabase.CreateAsset(
            combinedMesh,
            meshPath
        );

        AssetDatabase.SaveAssets();

        Debug.Log(
            "Leaf Meshes kombiniert: " +
            combineInstances.Count +
            " Meshes -> 1 Mesh."
        );

        Selection.activeGameObject = combinedObject;
    }
}