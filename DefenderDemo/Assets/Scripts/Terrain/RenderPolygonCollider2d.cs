using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PolygonCollider2D))]
public class RenderPolygonCollider2d : MonoBehaviour
{
    public Material materialToUse = null;

	// Use this for initialization
    void Start ()
    {
        UpdateMesh();
	}

    [ContextMenu("GenerateMesh")]
    protected void UpdateMesh()
    {
        PolygonCollider2D poly = GetComponent<PolygonCollider2D>();
        Vector2[] vertices2D = poly.points;

        // Use the triangulator to get indices for creating triangles
        Triangulator tr = new Triangulator(vertices2D);
        int[] indices = tr.Triangulate();

        // Create the Vector3 vertices
        Vector3[] vertices = new Vector3[vertices2D.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(vertices2D[i].x, vertices2D[i].y, 0);
        }

        // Create the mesh
        Mesh msh = new Mesh();
        msh.vertices = vertices;
        msh.triangles = indices;
        msh.RecalculateNormals();
        msh.RecalculateBounds();

        // Set up game object with mesh;
        MeshRenderer render = transform.GetOrAddComponent<MeshRenderer>();
        MeshFilter filter = transform.GetOrAddComponent<MeshFilter>();
        filter.mesh = msh;
        if (materialToUse)
            render.material = materialToUse;
    }
}
