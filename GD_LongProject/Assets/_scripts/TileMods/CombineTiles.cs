using UnityEngine;
using System.Collections.Generic;

public class CombineTiles : MonoBehaviour
{
    [ContextMenu("Combine Meshes")]
    void Start()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        List<CombineInstance> combineList = new List<CombineInstance>();

        Material mat = null;

        foreach (MeshFilter mf in meshFilters)
        {
            MeshRenderer mr = mf.GetComponent<MeshRenderer>();

            // Skip the parent itself (the combiner)
            if (mf.transform == transform)
                continue;

            // Skip objects with no mesh
            if (mf.sharedMesh == null)
                continue;

            // Skip objects that have no renderer (parents / logic objects)
            if (mr == null)
                continue;

            // Save material once
            if (mat == null)
                mat = mr.sharedMaterial;

            CombineInstance ci = new CombineInstance();
            ci.mesh = mf.sharedMesh;
            ci.transform = mf.transform.localToWorldMatrix;

            combineList.Add(ci);

            // Disable old objects
            mf.gameObject.SetActive(false);
        }

        if (combineList.Count == 0)
        {
            Debug.LogError("No valid meshes to combine!");
            return;
        }

        // Create new combined mesh
        Mesh combinedMesh = new Mesh();
        combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        combinedMesh.CombineMeshes(combineList.ToArray(), true, true);

        // Add components safely
        MeshFilter mfParent = gameObject.AddComponent<MeshFilter>();
        mfParent.sharedMesh = combinedMesh;

        MeshRenderer mrParent = gameObject.AddComponent<MeshRenderer>();
        mrParent.sharedMaterial = mat;

        Debug.Log("✨ Mesh combined successfully!");
    }
}