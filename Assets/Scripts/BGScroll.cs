using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgScroll : MonoBehaviour
{

public float scrollSpeed = 0.04f;

private MeshRenderer mesh;

private float y_Scroll;

    void Awake()
    {
        mesh = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Scroll();
    }

    void Scroll()
    {
        y_Scroll = Time.time * scrollSpeed;

        Vector2 offset = new Vector2(0f, y_Scroll);
        mesh.sharedMaterial.SetTextureOffset("_MainTex", offset);
    }
}