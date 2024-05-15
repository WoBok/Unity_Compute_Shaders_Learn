using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawProceduralNowTest : MonoBehaviour
{
    // Start is called before the first frame update
    public Material material;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }
    private void OnRenderObject()
    {
        material.SetPass(0);
        Graphics.DrawProceduralNow(MeshTopology.Points, 1, 1);
    }
}
