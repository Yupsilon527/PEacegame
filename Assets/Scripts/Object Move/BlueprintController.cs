using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueprintController : MonoBehaviour
{

    [Tooltip("Material used to indicate to the player that this building location is valid.")]
    public Material ValidMaterial;
    [Tooltip("Material used to indicate to the player that this building location is invalid.")]
    public Material InvalidMaterial;

    #region Monobehavior
    Transform RangeIndicator;
    MeshFilter[] meshfilters;
    MeshRenderer[] meshrenderers;

    void Awake()
    {
        GetMeshComponents();
    }
    public void GetMeshComponents()
    {
        meshfilters = GetComponentsInChildren<MeshFilter>();
        meshrenderers = GetComponentsInChildren<MeshRenderer>();
    }
    #endregion
    #region Materials
    bool CanBeBuilt = false;
    public void ChangeState(bool canBuild)
    {
        if (CanBeBuilt!= canBuild)
        {
            foreach (MeshRenderer meshRenderer in meshrenderers)
            {
                meshRenderer.material = canBuild ? ValidMaterial : InvalidMaterial;
            }
        }
        CanBeBuilt = canBuild;
    }
    #endregion
    #region Adjust Mesh
    public void LoadObject(GameObject loadObject)
    {
        ClearMesh();
        int iM = 0;
        foreach (MeshFilter MF in loadObject.GetComponentsInChildren<MeshFilter>())
        {
            meshfilters[iM].mesh = MF.sharedMesh;
            meshfilters[iM].transform.localPosition = MF.transform.localPosition;
            meshfilters[iM].transform.localRotation = MF.transform.localRotation;
            meshfilters[iM].transform.localScale = MF.transform.lossyScale;
            meshrenderers[iM].gameObject.SetActive(true);
            iM++;
        }
    }
    public void ClearMesh()
    {
        foreach (MeshRenderer meshRenderer in meshrenderers)
        {
            meshRenderer.gameObject.SetActive( false);
        }
    }
    #endregion
}
