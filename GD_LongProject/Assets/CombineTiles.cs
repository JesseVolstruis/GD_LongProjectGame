using UnityEngine;
using System.Collections.Generic;

public class CombineTiles : MonoBehaviour
{
    [ContextMenu("Combine Meshes")]
    void CombineMeshes()
    {
        // Get ALL mesh filters in children, even nested ones
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>(includeInactive: false);

        // We must ignore the parent’s own MeshFilter (if there is one)
        List<CombineInstance> combineList = new List<CombineInstance>();

        Material sharedMat = null;

        foreach (MeshFilter mf in meshFilters)
        {
            if (mf.transform == transform)  
                continue; // skip parent

            MeshRenderer mr = mf.GetComponent<MeshRenderer>();
            if (mr == null) 
                continue;

            if (sharedMat == null)
                sharedMat = mr.sharedMaterial;

            CombineInstance ci = new CombineInstance();
            ci.mesh = mf.sharedMesh;
            ci.transform = mf.transform.localToWorldMatrix;
            combineList.Add(ci);

            // Disable original objects
            mr.enabled = false;
        }

        // Create combined mesh
        Mesh combinedMesh = new Mesh();
        combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; // large meshes safe
        combinedMesh.CombineMeshes(combineList.ToArray(), true, true);

        // Attach or reuse MeshFilter + Renderer
        MeshFilter parentMF = GetComponent<MeshFilter>();
        if (parentMF == null) parentMF = gameObject.AddComponent<MeshFilter>();
        parentMF.sharedMesh = combinedMesh;

        MeshRenderer parentMR = GetComponent<MeshRenderer>();
        if (parentMR == null) parentMR = gameObject.AddComponent<MeshRenderer>();
        parentMR.sharedMaterial = sharedMat;

        Debug.Log("✨ Combined " + combineList.Count + " meshes into one!");
    }
}