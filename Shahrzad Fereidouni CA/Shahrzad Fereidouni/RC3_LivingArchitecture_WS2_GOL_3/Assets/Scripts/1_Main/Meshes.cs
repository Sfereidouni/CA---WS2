using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meshes : MonoBehaviour {

    /*
	// Use this for initialization
	void Start () {

        var filter = GetComponent<MeshFilter>();
        filter.sharedMesh =  MeshVoxel1();

	}
	
	// Update is called once per frame
	void Update () {
		
    }


    Mesh MeshVoxel1()
    {
        var positions = new Vector3[4];
        positions[0] = new Vector3(0.0f, 0.0f, 0.0f);
        positions[1] = new Vector3(1.0f, 0.0f, 1.0f);
        positions[2] = new Vector3(1.0f, 1.0f, 0.0f);
        positions[3] = new Vector3(0.0f, 1.0f, 1.0f);

        int[] indices = new int[]
        {
            2,1,0,
            2,0,3,
            3,1,2,
            0,1,3

        };

        Color[] colorsT = new Color[4];
        colorsT[0] = new Color(1f, 0f, 0.5f, 1);
        colorsT[1] = new Color(1f, 1f, 0.5f, 1);
        colorsT[2] = new Color(0f, 1f, 0.5f, 1);
        colorsT[3] = new Color(0f, 0f, 0.5f, 1);

        Mesh type1Mesh = new Mesh();

        type1Mesh.vertices = positions;
        type1Mesh.colors = colorsT;
        type1Mesh.SetTriangles(indices, 0);

        return type1Mesh;
    }

    Mesh MeshVoxel2()
    {
        Mesh type2Mesh = new Mesh();

        var positions = new Vector3[6];
        positions[0] = new Vector3(0.0f, 0.0f, 0.0f);
        positions[1] = new Vector3(1.0f, 0.0f, 1.0f);
        positions[2] = new Vector3(0.0f, 1.0f, 0.5f);
        positions[3] = new Vector3(0.5f, 1.0f, 0.0f);
        positions[4] = new Vector3(0.5f, 1.0f, 1.0f);
        positions[5] = new Vector3(1.0f, 1.0f, 0.5f);


        int[] indices = new int[]
        {
            0,1,3,
            1,5,3,
            0,3,2,
            3,4,2,
            3,5,4,
            1,4,5,
            0,4,2,
            1,4,0,

        };


        type2Mesh.vertices = positions;

        Color[] colors = new Color[6];
        colors[0] = new Color(1f, 0f, 0.5f, 1);
        colors[1] = new Color(1f, 1f, 0.5f, 1);
        colors[2] = new Color(0f, 1f, 0.5f, 1);
        colors[3] = new Color(0f, 0f, 0.5f, 1);
        colors[4] = new Color(0.5f, 1f, 0.5f, 1);
        colors[5] = new Color(1f, 0f, 0.5f, 0);

        type2Mesh.colors = colors;
        type2Mesh.SetTriangles(indices, 0);

        return type2Mesh;
    }

    Mesh MeshVoxel4()
    {
        Mesh type4Mesh = new Mesh();
        var positions  = new Vector3[4];
        positions[0] = new Vector3(0.0f, 0.0f, 0.0f);
        positions[1] = new Vector3(1.0f, 0.0f, 1.0f);
        positions[2] = new Vector3(1.0f, 1.0f, 0.0f);
        positions[3] = new Vector3(0.0f, 1.0f, 1.0f);

        int[] indices = new int[]
        {
            2,1,0,
            2,0,3,
            3,1,2,
            0,1,3
        };

        type4Mesh.vertices = positions;

        Color[] colors = new Color[4];
        colors[0] = new Color(1f, 0f, 0.5f, 1);
        colors[1] = new Color(1f, 1f, 0.5f, 1);
        colors[2] = new Color(0f, 1f, 0.5f, 1);
        colors[3] = new Color(0f, 0f, 0.5f, 1);

        type4Mesh.colors = colors;
        type4Mesh.SetTriangles(indices, 0);

        return type4Mesh;
    }


    Mesh MeshVoxel3()
    {
        var positions = new Vector3[4]
        {
            new Vector3(-0.5f, -0.5f, 0.0f),
            new Vector3(0.5f, -0.5f, 0.0f),
            new Vector3(-0.5f, 0.5f, 0.0f),
            new Vector3(0.5f, 0.5f, 0.0f)
        };

        var colors = new Color[4]
        {
            new Color(0.0f,0.0f,0.5f),
            new Color(1.0f,0.0f,0.5f),
            new Color(0.0f,1.0f,0.5f),
            new Color(1.0f,1.0f,0.5f)
        };

        var indices = new int[] { 0, 1, 2, 2, 1, 3 };

        var type3Mesh = new Mesh();
        type3Mesh.vertices = positions;
        type3Mesh.colors = colors;
        type3Mesh.SetIndices(indices, MeshTopology.Triangles, 0);

        return type3Mesh;

    }






    */

}