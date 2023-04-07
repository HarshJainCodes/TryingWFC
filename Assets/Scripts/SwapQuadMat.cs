using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapQuadMat : MonoBehaviour
{
    [SerializeField] MeshRenderer quadMeshRenderer;

    private void Start()
    {
    }

    public void SwapMat(Material mat)
    {
        quadMeshRenderer.material = mat;
    }
}
