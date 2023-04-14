using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCreator : MonoBehaviour
{
    [SerializeField]
    Texture m_texture;

    Vector3[] m_vertices = new Vector3[]
    {
        new Vector3(-1f, 1f, 0f), new Vector3(1f, 1f, 0f),
        new Vector3(1f, -1f, 0f), new Vector3(-1f, -1f, 0f)
    };

    int[] triangles = new int[] {0, 1, 2, 0, 2, 3, };
    Vector2[] m_uvs = new Vector2[]
     {
         new Vector2(0f, 1f), new Vector2(1, 1f),
         new Vector2(1f, 0f), new Vector2(0f, 0f)
};

    Mesh m_mesh;
    // Start is called before the first frame update
    void Start()
    {
        m_mesh = new Mesh();
        m_mesh.vertices = m_vertices;
        m_mesh.triangles = triangles;
        m_mesh.uv = m_uvs;
        m_mesh.RecalculateNormals();
        m_mesh.RecalculateBounds();
        m_mesh.name = "Rectangle";
        var mFilter  = gameObject.AddComponent<MeshFilter>();
        mFilter.mesh = m_mesh;
        var renderer = gameObject.AddComponent<MeshRenderer>();
        Material mat = new Material(Shader.Find("Standard"));
        mat.SetTexture("_MainTex", m_texture);
        renderer.material = mat;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
