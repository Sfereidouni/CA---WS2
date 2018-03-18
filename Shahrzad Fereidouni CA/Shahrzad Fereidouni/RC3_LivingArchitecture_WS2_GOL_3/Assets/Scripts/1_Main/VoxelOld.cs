using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelOld : MonoBehaviour 
{
    //public MeshTable MeshTable;
    public Mesh[] MeshTable;

    private MeshFilter _filter;
    private MeshRenderer _renderer;
    private Material _material;


    private void Start()
    {
        _filter = GetComponent<MeshFilter>();

        //_renderer = GetComponent<MeshRenderer>();
        //_material = renderer.material;
    }


    // VARIABLES
    //state
    private int state = 0;
    //next state
    private int futureState = 0;
    //age
    private int age = 0;
    //density3dMO
    private int density3dMO = 0;
    //density3dVN
    private int density3dVN = 0;
    //material property block for setting material properties with renderer
    private MaterialPropertyBlock props;
    //the mesh renderer
    private new MeshRenderer renderer;
    //var stores my 3d address
	public Vector3 address;

    //MeshFilter setMesh;

    //The Mesh Filter takes a mesh from your assets and passes it to the Mesh Renderer for rendering on the screen
    //One Voxel can contain different meshes which are the representation of different types of voxels
    //public MeshFilter type0mesh, type1Mesh, type2Mesh, type3Mesh;

    //variable to store a type for this voxel
	int type;

    //von neumann neighbors
    private VoxelOld[] neighbors3dVN = new VoxelOld[6];

    //moore's neighbors
    private VoxelOld[] neighbors3dMO = new VoxelOld[26];

    private VoxelOld voxelAbove;
    private VoxelOld voxelBelow;
    private VoxelOld voxelRight;
    private VoxelOld voxelLeft;
    private VoxelOld voxelFront;
    private VoxelOld voxelBack;

    // FUNCTIONS

    public void SetupVoxel(int i, int j, int k, int type)
    {
        //set reference to time end 
        props = new MaterialPropertyBlock();
        renderer = gameObject.GetComponent<MeshRenderer>();
        //initially set to false
        renderer.enabled = false;
        //set my address as a vector
		address = new Vector3 (i,j,k);

        //type0mesh = gameObject.GetComponent<MeshFilter>();

        //gets the type of this voxel and sets the mesh filter by type - allows us to preload
        //different meshes and render a different mesh for different voxels based on the type
	
        /*
        if (age%2==0)
        {
            type = 1;
        }
        */

        if (MeshTable == null)
            Debug.Log("No MeshTable assigned");

        if (_filter == null)
            Debug.Log("No filter assigned");


        _filter.sharedMesh = MeshTable[type];

                                    
        /*
        switch (type) 
        {
		case 1:
			MeshFilter setMesh = 
			break;
		case 2:
			MeshFilter setMesh2 = gameObject.GetComponent<MeshFilter> ();
			setMesh2 = type2Mesh;
			break;
		case 3:
			MeshFilter setMesh3 = gameObject.GetComponent<MeshFilter> ();
			setMesh3 = type3Mesh;
			break;	
		default:
			MeshFilter setMeshDefault = gameObject.GetComponent<MeshFilter> ();
			setMeshDefault = type3Mesh;
			break;
		} 
		*/
    }

    /*
    public MeshFilter MeshSwitch()
    {
        MeshFilter setMesh = gameObject.GetComponent<MeshFilter>();
        setMesh = type1Mesh;
        return type1Mesh;

    }*/



	// Update function
	public void UpdateVoxel () {
		// Set the future state
		state = futureState;
        // If voxel is alive update age
        if (state == 1)
        {
            age++;
        
        }
        // If voxel is death disable the game object mesh renderer and set age to zero
        if (state == 0)
        {
            age = 0;
        }
    }


    /// <summary>
    /// Setters and Getters - Allow us to access and set private variables
    /// </summary>
    /// <param name="_state"></param>
	// Set the state of the voxel
	public void SetState(int _state){
		state = _state;
	}

	// Set the future state of the voxel
	public void SetFutureState(int _futureState){
		futureState = _futureState;
	}

    // Get the age of the voxel
    public void SetAge(int _age){
		age = _age;
       /* if (age % 2 == 0)
        {
            setMesh = gameObject.GetComponent<MeshFilter>();
            setMesh = type1Mesh;
        }
        */
	}

	// Get the state of the voxel
	public int GetState(){
		return state;
	}

	// Get the age of the voxel
	public int GetAge(){
		return age;
	}

    //Set 3d Moores Neighborhood Density 
    public void setDensity3dMO(int _density3dMO)
    {
        density3dMO = _density3dMO;
    }
    //Get 3d Moores Neighborhood Density 
    public int getDensity3dMO()
    {
        return density3dMO;
    }

    //Set 3d Von Neumann Neighborhood Density 
    public void setDensity3dVN(int _density3dVN)
    {
        density3dVN = _density3dVN;
    }
    //Get 3d Von Neumann Neighborhood Density 
    public int getDensity3dVN()
    {
        return density3dVN;
    }



    /// <summary>
    /// VOXEL NEIGHBORHOOD GETTERS/SETTERS
    /// </summary>
    /// 

    //MOORES NEIGHBORS (26 PER VOXEL)
    public void setNeighbors3dMO(VoxelOld[] _setNeighbors3dMO)
    {
        neighbors3dMO = _setNeighbors3dMO;
    }

    public VoxelOld[] getNeighbors3dMO()
    {
        return neighbors3dMO;
    }

    //VON NEUMANN NEIGHBORS (6 PER VOXEL)
    public void setNeighbors3dVN(VoxelOld[] _setNeighbors3dVN)
    {
        neighbors3dVN = _setNeighbors3dVN;
    }

    public VoxelOld[] getNeighbors3dVN()
    {
        return neighbors3dVN;
    }


    //voxel above this
    public void setVoxelAbove(VoxelOld _voxelAbove)
    {
        voxelAbove = _voxelAbove;
    }

    public VoxelOld getVoxelAbove()
    {
        return voxelAbove;
    }

    //voxel below this
    public void setVoxelBelow(VoxelOld _voxelBelow)
    {
        voxelBelow = _voxelBelow;
    }

    public VoxelOld getVoxelBelow()
    {
        return voxelBelow;
    }

    //voxel right of this
    public void setVoxelRight(VoxelOld _voxelRight)
    {
        voxelRight = _voxelRight;
    }

    public VoxelOld getVoxelRight()
    {
        return voxelRight;
    }

    //voxel left of this
    public void setVoxelLeft(VoxelOld _voxelLeft)
    {
        voxelLeft = _voxelLeft;
    }

    public VoxelOld getVoxelLeft()
    {
        return voxelLeft;
    }

    //voxel in front of this
    public void setVoxelFront(VoxelOld _voxelFront)
    {
        voxelFront = _voxelFront;
    }

    public VoxelOld getVoxelFront()
    {
        return voxelFront;
    }

    //voxel in back of this
    public void setVoxelBack(VoxelOld _voxelBack)
    {
        voxelBack = _voxelBack;
    }

    public VoxelOld getVoxelBack()
    {
        return voxelBack;
    }

    // Update the voxel display
    public void VoxelDisplay()
    {
        if (state == 1)
        {
            // Set Color
            Color col = new Color(1, 0, 0, 1);
            props.SetColor("_Color", col);
            // Updated the mesh renderer color
            renderer.enabled = true;
            renderer.SetPropertyBlock(props);
        }

        if (state == 0)
        {
            renderer.enabled = false;
        }
    }

    public void VoxelDisplay(int _r, int _g, int _b)
    {
        if (state == 1)
        {
            // Set Color
            Color col = new Color(_r, _g, _b, 1);
            props.SetColor("_Color", col);
            // Updated the mesh renderer color
            renderer.enabled = true;
            renderer.SetPropertyBlock(props);
        }

        if (state == 0)
        {
            renderer.enabled = false;
        }
    }

    /// <summary>
    /// Create Color Gradient Between 2 Colors by Age
    /// </summary>
    /// <param name="_maxAge"></param>
    public void VoxelDisplayAge(int _maxAge)
    {
        /*
        if (age % 2 == 0)
        {
            MeshFilter setMesh = gameObject.GetComponent<MeshFilter>();
            setMesh = type1Mesh;
        }


        if (state == 1)
        {
            // Remap the age value relative to maxage to range of 0,1
            float mappedvalue = Remap(age, 0, _maxAge, 0.0f, 1.0f);
            //two colors to interpolate between
            Color color1 = new Color(1, 1, 1, 1);
            Color color2 = new Color(1, 1, 0, 1);
            //interpolate color from mapped value
            Color mappedcolor = Color.Lerp(color1, color2, mappedvalue);
            props.SetColor("_Color", mappedcolor);
            // Updated the mesh renderer color
            renderer.enabled = true;
            renderer.SetPropertyBlock(props);
        }
        if (state == 0)
        {
            renderer.enabled = false;
        }
        */
    }



    /// <summary>
    /// Create Color Gradient Between 2 Colors by Density
    /// </summary>
    /// <param name="_maxdensity3dMO"></param>
    public void VoxelDisplayDensity3dMO(int _maxdensity3dMO)
    {
        if (state == 1)
        {
            // Remap the density value relative to maxdensity to range of 0,1
            float mappedvalue = Remap(density3dMO, 0, _maxdensity3dMO, 0.0f, 1.0f);
            //two colors to interpolate between
            Color color1 = new Color(1, 1, 1, 1);
            Color color2 = new Color(1, 0, 1, 1);
            //interpolate color from mapped value
            Color mappedcolor = Color.Lerp(color1, color2, mappedvalue);
            props.SetColor("_Color", mappedcolor);
            // Updated the mesh renderer color
            renderer.enabled = true;
            renderer.SetPropertyBlock(props);
        }
        if (state == 0)
        {
            renderer.enabled = false;
        }
    }

    /// <summary>
    /// Create Color Gradient Between 2 Colors by Density
    /// </summary>
    /// <param name="_maxdensity3dMO"></param>
    public void VoxelDisplayDensity3dVN(int _maxdensity3dVN)
    {
        if (state == 1)
        {
            // Remap the density value relative to maxdensity to range of 0,1
            float mappedvalue = Remap(density3dMO, 0, _maxdensity3dVN, 0.0f, 1.0f);
            //two colors to interpolate between
            Color color1 = new Color(1, 1, 1, 1);
            Color color2 = new Color(0, 1, 1, 1);
            //interpolate color from mapped value
            Color mappedcolor = Color.Lerp(color1, color2, mappedvalue);
            props.SetColor("_Color", mappedcolor);
            // Updated the mesh renderer color
            renderer.enabled = true;
            renderer.SetPropertyBlock(props);
        }
        if (state == 0)
        {
            renderer.enabled = false;
        }
    }


    // Remap numbers - used here for getting a gradient of color across a range
    private float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}
