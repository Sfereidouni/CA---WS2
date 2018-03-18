using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;


public class Environment : MonoBehaviour {

	// VARIABLES
    bool haveSeparatedByDensity = false;
    List<List<GameObject>> voxelsByDensity = new List<List<GameObject>>();
    int currentDensityToDisplay = 0;

	// Texture to be used as start of CA input
	public Texture2D seedImage;
    
	// Number of frames to run which is also the height of the CA
	public int timeEnd = 2;
	int currentFrame = 0;

    //variables for size of the 3d grid
	int width;
	int length;
	int height;

    // Array for storing voxels
    GameObject[,,] voxelGrid;
    //Voxel[,,] voxelGrid;

	// Reference to the voxel we are using
	public GameObject voxelPrefab;

	// Spacing between voxels
	float spacing = 1f;

    //Layer Densities
    int totalAliveCells = 0;
    float layerdensity = 0;
    float[] layerDensities;

    //Max Age
    int maxAge = 0;

    //Max Densities
    int maxDensity3dMO = 0;
    int minDensity3dMO = 26;
    int maxDensity3dVN = 0;
    int maxDensity3dJS = 0;

    // Setup Different Game of Life Rules
    GOLRule deathrule = new GOLRule();
    GOLRule rule1 = new GOLRule();
    GOLRule rule2 = new GOLRule();
    GOLRule rule3 = new GOLRule();
    GOLRule rule4 = new GOLRule();
    GOLRule rule5 = new GOLRule();
    GOLRule rule6= new GOLRule();
    GOLRule ruleVN = new GOLRule();
    GOLRule ruleNew = new GOLRule();
    GOLRule lastRule = new GOLRule();


    //boolean switches
    //toggles pausing the game
    bool pause = false;

    bool export = false;

	// FUNCTIONS
	// Use this for initialization
	void Start () 
    {
		// Read the image width and height
		width = seedImage.width;
		length = seedImage.height;
		height = timeEnd;


        //Setup GOL Rules
        rule1.setupRule(2,3,3,3);
        rule2.setupRule(1, 3, 3, 6);
        rule3.setupRule(2, 6, 4, 5);
        rule4.setupRule(4, 4, 1, 2);
        rule5.setupRule(2, 3, 2, 4);
        rule6.setupRule(1, 2, 6, 8);
        ruleVN.setupRule(1,6, 1, 6);
        deathrule.setupRule(0, 0, 0, 0);
        ruleNew.setupRule(1, 2, 3, 3);
        lastRule.setupRule(9, 9, 9, 9);

        //Layer Densities
        layerDensities = new float[timeEnd];

        // Create a new CA grid
        CreateGrid ();
        SetupNeighbors3d();
    }
	
	// Update is called once per frame
	void Update () 
    {

        // Calculate the CA state, save the new state, display the CA and increment time frame
        if (currentFrame < timeEnd - 1)
        {
            if (pause == false)
            {
            
            // Calculate the future state of the voxels
            CalculateCA();
            CalculateVN();
            // Update the voxels that are printing
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < length; j++)
                {
                       //Voxel currentVoxel = voxelGrid[i, j, 0];
                       GameObject currentVoxel = voxelGrid[i, j, 0];
                        currentVoxel.GetComponent<Voxel>().UpdateVoxel();
                }

            }
            
            // Save the CA state
            SaveCA();

            //Update 3d Densities
            updateDensities3d();
            // Increment the current frame count
            currentFrame++;

            
           // Debug.Log("Current itteration: " + currentFrame);
            }

            // >>>
            //Debug.Log("Currently there are " + totalAliveCells + " alive cells.");

            // Display the printed voxels
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    for (int k = 1; k < height; k++)
                    {
                        //voxelGrid[i, j, k].GetComponent<Voxel>().VoxelDisplay();
                        //voxelGrid[i, j, k].GetComponent<Voxel>().VoxelDisplayAge(maxAge);
                        voxelGrid[i, j, k].GetComponent<Voxel>().VoxelDisplayDensity3dMO(maxDensity3dMO);
                        //voxelGrid[i, j, k].GetComponent<Voxel>().VoxelDisplayDensity3dVN(maxDensity3dVN);
                        //voxelGrid[i, j, k].GetComponent<Voxel>().VoxelDisplayDensity3dMO(minDensity3dMO);
                        //voxelGrid[i, j, k].GetComponent<Voxel>().VoxelDisplayDensity3dJS(maxDensity3dJS);
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.E)&&timeEnd>currentFrame-1)
        {
            if (export == false)
            {
                export = true;
            }
            else
            {
                export = false;
            }
        }

        if (export == true)
        {
            ExportToText();
            Debug.Log("export ready");
        }


        if (Input.GetKeyDown(KeyCode.D))
        {
            ExportPrepare();
            Debug.Log("READY FOR EXPORT");
        }

        if (currentFrame >= timeEnd - 1 && Input.GetKeyDown(KeyCode.X))
        {
            SeparateVoxelsByDensity();
            currentFrame++;
            haveSeparatedByDensity = true;
        }

        // Show densities 
        if (haveSeparatedByDensity)
        {
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                Transform childTransform = gameObject.transform.GetChild(i);
                childTransform.gameObject.SetActive(false);
            }

            List<GameObject> currentDensityVoxelsToDsiplay = voxelsByDensity[currentDensityToDisplay];
            for (int i = 0; i < currentDensityVoxelsToDsiplay.Count; i++)
            {
                currentDensityVoxelsToDsiplay[i].SetActive(true);
            }

            if (Input.GetKeyDown(KeyCode.D) && currentDensityToDisplay < voxelsByDensity.Count)
            {
                Debug.Log("density is" + currentDensityToDisplay);
                currentDensityToDisplay++;
            }
        }

        // Spin the CA if spacebar is pressed (be careful, GPU instancing will be lost!)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (gameObject.GetComponent<ModelDisplay>() == null)
            {
                gameObject.AddComponent<ModelDisplay>();
            }
            else 
            {
                Destroy(gameObject.GetComponent<ModelDisplay>());
            }
        }

        //toggle pause with "p" key
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (pause == false)
            {
                pause = true;
            }
            else
            {
                pause = false;
            }
        }

      if (currentFrame == timeEnd - 1) CalculateMeshNew();
        //Debug.Log("Mesh calculated");
       
        //if (currentFrame == timeEnd-1) VonNeumannLookup();

    }

    // Create grid function
    void CreateGrid()
    {
        // Allocate space in memory for the array
        
        voxelGrid = new GameObject[width, length, height];
        // Populate the array with voxels from a base image
        for (int i = 0; i < width; i++) 
        {
			for (int j = 0; j < length; j++) 
            {
				for (int k = 0; k < height; k++)
                {
        
					// Create values for the transform of the new voxel
					Vector3 currentVoxelPos = new Vector3 (i*spacing,k*spacing,j*spacing);
					Quaternion currentVoxelRot = Quaternion.identity;
                    //create the game object of the voxel
					GameObject currentVoxelObj = Instantiate (voxelPrefab, currentVoxelPos, currentVoxelRot);
                    //run the setupVoxel() function inside the 'Voxel' component of the voxelPrefab
                    //this sets up the instance of Voxel class inside the Voxel game object
                    currentVoxelObj.GetComponent<Voxel>().SetupVoxel(i,j,k,1);

                    // Set the state of the voxels
                    if (k == 0)
                    {
                        // Create a new state based on the input image

                        float t = seedImage.GetPixel(i, j).grayscale;

                        // black - > alive
                        if (t > 0.5f)
                            currentVoxelObj.GetComponent<Voxel>().SetState(0);
                        else
                            currentVoxelObj.GetComponent<Voxel>().SetState(1);

                    }
                    else
                    {
                        // Set the state to death
                        currentVoxelObj.GetComponent<MeshRenderer>().enabled = false;
                        currentVoxelObj.GetComponent<Voxel>().SetState(0);
                    }
                    // Save the current voxel in the voxelGrid array
                    voxelGrid[i, j, k] = currentVoxelObj;
                    // Attach the new voxel to the grid game object
                    currentVoxelObj.transform.parent = gameObject.transform;
				}
			}
		}
	}

    void CalculateVN()
    {
        // Go over all the voxels stored in the voxels array
        for (int i =1; i < width/2; i++)
        {
            for (int j =1; j < length - 1; j++)
            {
                GameObject currentVoxelObj = voxelGrid[i, j, 0];
                int currentVoxelState = currentVoxelObj.GetComponent<Voxel>().GetState();
                int aliveVNNeighbours = 0;

                // Calculate how many alive neighbours are around the current voxel
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {

                        if (x == 0 || y == 0)
                        {
                            GameObject currentNeigbour = voxelGrid[i + x, j + y, 0];
                            int currentNeigbourState = currentNeigbour.GetComponent<Voxel>().GetState();
                            aliveVNNeighbours += currentNeigbourState;
                            // >>> Counting until 9
                            //Debug.Log("alive neighbours : " + aliveNeighbours);
                        }
                    }
                }
                aliveVNNeighbours -= currentVoxelState;
                GOLRule currentRule = rule4;

                //if (layerdensity < 0.2) currentRule = rule1;
                //if (layerdensity < 0.2 && maxAge > 15) currentRule = rule4;
                //if (currentFrame > 30) currentRule = rule6;
                //if (maxAge == 17) currentRule = rule4;


                int inst0 = currentRule.getInstruction(0);
                int inst1 = currentRule.getInstruction(1);
                int inst2 = currentRule.getInstruction(2);
                int inst3 = currentRule.getInstruction(3);

                if (currentVoxelState == 1)
                {
                    // If there are less than two neighbours I am going to die
                    if (aliveVNNeighbours < inst0)
                    {
                        currentVoxelObj.GetComponent<Voxel>().SetFutureState(0);
                    }
                    // If there are two or three neighbours alive I am going to stay alive
                    if (aliveVNNeighbours >= inst0 && aliveVNNeighbours <= inst1)
                    {
                        currentVoxelObj.GetComponent<Voxel>().SetFutureState(1);
                    }
                    // If there are more than three neighbours I am going to die
                    if (aliveVNNeighbours > inst1)
                    {
                        currentVoxelObj.GetComponent<Voxel>().SetFutureState(0);
                    }
                }
                // Rule Set 2: for voxels that are death
                if (currentVoxelState == 0)
                {
                    // If there are exactly three alive neighbours I will become alive
                    if (aliveVNNeighbours >= inst2 && aliveVNNeighbours <= inst3)
                    {
                        currentVoxelObj.GetComponent<Voxel>().SetFutureState(1);
                    }
                }

            }
        }
    }

                // Calculate CA function
    void CalculateCA()
    {
		// Go over all the voxels stored in the voxels array
		for (int i = width/2; i < width-1; i++) 
        {
			for (int j = 1; j < length-1; j++)
            {
				GameObject currentVoxelObj = voxelGrid[i,j,0];
				int currentVoxelState = currentVoxelObj.GetComponent<Voxel> ().GetState ();
				int aliveNeighbours = 0;

				// Calculate how many alive neighbours are around the current voxel
				for (int x = -1; x <= 1; x++)
                {
					for (int y = -1; y <= 1; y++) 
                    {
						GameObject currentNeigbour = voxelGrid [i + x, j + y,0];
						int currentNeigbourState = currentNeigbour.GetComponent<Voxel> ().GetState();
						aliveNeighbours += currentNeigbourState;
                        // >>> Counting until 9
                        //Debug.Log("alive neighbours : " + aliveNeighbours);
					}
				}
				aliveNeighbours -= currentVoxelState;

                //CHANGE RULE BASED ON CONDITIONS HERE:
               GOLRule currentRule = rule1;
              //GOLRule currentRule = rule2;
              //GOLRule currentRule = rule3;
              //GOLRule currentRule = rule4;
              //GOLRule currentRule = rule5;
                               
                //if (currentFrame > width/4 && currentFrame < width/2) currentRule = rule3;

                //if (currentFrame > width/3 && currentFrame < width/2) currentRule = rule2;

                //if (currentFrame > width /2 && currentFrame <60) currentRule = rule1;

                //if (currentFrame > 70) currentRule = rule5;
               // if (currentFrame == timeEnd-2) currentRule = lastRule;

                //if (currentVoxelObj.GetComponent<Voxel>().GetAge() == 10) currentRule = rule2;

                if (Input.GetKeyDown(KeyCode.R)) currentRule = rule1;
                if (Input.GetKeyDown(KeyCode.T)) currentRule = rule2;
                if (Input.GetKeyDown(KeyCode.Y)) currentRule = rule3;
                if (Input.GetKeyDown(KeyCode.U)) currentRule = rule4;


                // if (currentFrame > 40) currentRule = rule2;

                //if(currentVoxelObj.GetComponent<Voxel>().GetAge()>3) currentRule = deathrule;

               //if(layerdensity < 0.2) currentRule = rule1;
               //if (layerdensity<0.2 && maxAge > 15) currentRule = rule4;
               //if (currentFrame > 30) currentRule = rule6;
                //if (maxAge == 17) currentRule = rule4;
                //if (layerdensity >0.2 &&) currentRule = rule1;
                 if (currentFrame > 40) currentRule = rule2;
                // if (layerdensity > .4) currentRule = rule1;
                //if (layerdensity<0.2 && currentFrame >30) currentRule = rule4;
                if (currentFrame == timeEnd - 2) currentRule = lastRule;

                //get the instructions
                int inst0 = currentRule.getInstruction(0);
                int inst1 = currentRule.getInstruction(1);
                int inst2 = currentRule.getInstruction(2);
                int inst3 = currentRule.getInstruction(3);

                // Rule Set 1: for voxels that are alive
                if (currentVoxelState == 1)
                {
					// If there are less than two neighbours I am going to die
					if (aliveNeighbours < inst0)
                    {
                        currentVoxelObj.GetComponent<Voxel> ().SetFutureState (0);
					}
					// If there are two or three neighbours alive I am going to stay alive
					if(aliveNeighbours >= inst0 && aliveNeighbours <= inst1)
                    {
                        currentVoxelObj.GetComponent<Voxel> ().SetFutureState (1);
					}
					// If there are more than three neighbours I am going to die
					if (aliveNeighbours > inst1) {
                        currentVoxelObj.GetComponent<Voxel> ().SetFutureState (0);
					}
				}
				// Rule Set 2: for voxels that are death
				if(currentVoxelState == 0)
                {
					// If there are exactly three alive neighbours I will become alive
					if(aliveNeighbours >= inst2 && aliveNeighbours <= inst3)
                    {
                        currentVoxelObj.GetComponent<Voxel> ().SetFutureState (1);
					}
				}


                //CalculateMesh();

                //age - here is an example of a condition where the cell is "killed" if its age is above a threshhold
                // in this case if this rule is put here after the Game of Life rules just above it, it would override 
                // the game of lie conditions if this condition was true
                /*
                if (currentVoxelObj.GetComponent<Voxel>().GetAge() > 5)
                {
                    currentVoxelObj.GetComponent<Voxel>().SetFutureState(0);
                }
                */

            }
		}
	}

   
    // Save the CA states - this is run after the future state of all cells is calculated to update/save
    //current state on the current level
	void SaveCA()
    {

        //counter stores the number of live cells on this level and is incremented below 
        //in the for loop for each cell with a state of 1
        totalAliveCells = 0;
		for(int i =0; i< width; i++)
        {
			for (int j = 0; j < length; j++) 
            {
                GameObject currentVoxelObj = voxelGrid[i, j, 0];
                int currentVoxelState = currentVoxelObj.GetComponent<Voxel>().GetState();
                // Save the voxel state
                GameObject savedVoxel = voxelGrid[i, j, currentFrame];
                savedVoxel.GetComponent<Voxel> ().SetState (currentVoxelState);                
                // Save the voxel age if voxel is alive
                if (currentVoxelState == 1) 
                {
                    int currentVoxelAge = currentVoxelObj.GetComponent<Voxel>().GetAge();
                    savedVoxel.GetComponent<Voxel>().SetAge(currentVoxelAge);
                    totalAliveCells++;

                    //track oldest voxels
                    if (currentVoxelAge>maxAge)
                    {
                        maxAge = currentVoxelAge;
                      //  Debug.Log("The oldest Voxel is " + maxAge + " generations old." );
                    }
                }
			}
		}

        float totalcells = length * width;
        layerdensity = totalAliveCells/ totalcells;
        //this stores the density of live cells for each entire layer of cells(each level)
        layerDensities[currentFrame] = layerdensity;


        //Kills the ones that are not in the current layer
        /*
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < length; j++)
            {
                for (int k = 1; k < 25; k++)
                {
                    GameObject currentVoxelObj = voxelGrid[i, j, k];
                    currentVoxelObj.GetComponent<Voxel>().SetState(0);

                }
            }
        }
        */

    }
    // Functions for GUI controller

    public int AliveCells()
    {
        
        return totalAliveCells;
    }

    public float Density()
    {
        return layerdensity;
    }

    public int CurrentFrame()
    {
        return currentFrame;
    }

    public int MaxAge()
    {
        return maxAge;
    }

    /// <summary>
    /// SETUP MOORES & VON NEUMANN 3D NEIGHBORS
    /// </summary>
    void SetupNeighbors3d()
    {
        for (int i = 1; i < width-1; i++)
        {
            for (int j = 1; j < length-1; j++)
            {
                for (int k = 1; k < height-1; k++)
                {
                    //the current voxel we are looking at...
                    GameObject currentVoxelObj = voxelGrid[i, j, k];

                    ////SETUP Von Neumann Neighborhood Cells////
                    Voxel[] tempNeighborsVN = new Voxel[6];
                    List<Voxel> tempNeighborsVNList = new List<Voxel>();

                    //left
                    Voxel VoxelLeft = voxelGrid[i - 1, j, k].GetComponent<Voxel>();
                    currentVoxelObj.GetComponent<Voxel>().setVoxelLeft(VoxelLeft);
                    tempNeighborsVN[0] = VoxelLeft;

                    //right
                    Voxel VoxelRight = voxelGrid[i + 1, j, k].GetComponent<Voxel>();
                    currentVoxelObj.GetComponent<Voxel>().setVoxelRight(VoxelRight);
                    tempNeighborsVN[2] = VoxelRight;

                    //back
                    Voxel VoxelBack = voxelGrid[i, j - 1, k].GetComponent<Voxel>();
                    currentVoxelObj.GetComponent<Voxel>().setVoxelBack(VoxelBack);
                    tempNeighborsVN[3] = VoxelBack;

                    //front
                    Voxel VoxelFront = voxelGrid[i, j + 1, k].GetComponent<Voxel>();
                    currentVoxelObj.GetComponent<Voxel>().setVoxelFront(VoxelFront);
                    tempNeighborsVN[1] = VoxelFront;

                    //below
                    Voxel VoxelBelow = voxelGrid[i, j, k - 1].GetComponent<Voxel>();
                    currentVoxelObj.GetComponent<Voxel>().setVoxelBelow(VoxelBelow);
                    tempNeighborsVN[4] = VoxelBelow;

                    //above
                    Voxel VoxelAbove = voxelGrid[i, j, k + 1].GetComponent<Voxel>();
                    currentVoxelObj.GetComponent<Voxel>().setVoxelAbove(VoxelAbove);
                    tempNeighborsVN[5] = VoxelAbove;

                    //Set the Von Neumann Neighbors [] in this Voxel
                    currentVoxelObj.GetComponent<Voxel>().setNeighbors3dVN(tempNeighborsVN);

                   /* foreach (Voxel vox in tempNeighborsVN)
                    {
                        if (vox.GetState()==1)
                        {
                            vox.VoxelDisplay(0, 0, 1);
                        }
                    }*/


                    ////SETUP Moore's Neighborhood////
                    Voxel[] tempNeighborsMO = new Voxel[26];
                    List<Voxel> tempNeighborsMOList = new List<Voxel>();

                    int tempcount = 0;
                    for (int m = -1; m < 2; m++)
                    {
                        for (int n = -1; n < 2; n++)
                        {
                            for (int p = -1; p < 2; p++)
                            {
                                if ((i + m >= 0) && (i + m < width) && (j + n >= 0) && (j + n < length) && (k + p >= 0) && (k + p < height))
                                {
                                    GameObject neighborVoxelObj = voxelGrid[i + m, j + n, k + p];
                                    if (neighborVoxelObj != currentVoxelObj)
                                    {
                                        Voxel neighborvoxel = voxelGrid[i + m, j + n, k + p].GetComponent<Voxel>();
                                        tempNeighborsMO[tempcount] = neighborvoxel;
                                        tempcount++;
                                    }
                                }
                            }
                        }
                    }
                    currentVoxelObj.GetComponent<Voxel>().setNeighbors3dMO(tempNeighborsMO);
                    List<Voxel> tempNeighborsJS = new List<Voxel>();

                    ////SETUP JS Neighborhood////
                    for (int m = 0; m < tempNeighborsMOList.Count; m++)
                    {
                        Voxel neighbor = tempNeighborsMOList[m];
                        if (tempNeighborsVNList.Contains(neighbor) != true)
                        {
                            tempNeighborsJS[m] = neighbor;
                        }
                    }

                    //currentVoxelObj.GetComponent<Voxel>().setNeighbors3dJS(tempNeighborsJS);
                }
            }
        }
    }
    /// <summary>
    /// Update 3d Densities for Each Voxel
    /// </summary>
    void updateDensities3d()
    {
        for (int i = 1; i < width-1; i++)
        {
            for (int j = 1; j < length-1; j++)
            {
                for (int k = 1; k < currentFrame; k++)
                {
                    GameObject currentVoxelObj = voxelGrid[i, j, k];

                    //UPDATE THE VON NEUMANN NEIGHBORHOOD DENSITIES FOR EACH VOXEL//
                    Voxel[] tempNeighborsVN = currentVoxelObj.GetComponent<Voxel>().getNeighbors3dVN();
                    int alivecount = 0;
                    foreach (Voxel vox in tempNeighborsVN)
                    {
                        if (vox.GetState() == 1)
                        {
                            alivecount++;
                        }
                    }
                    currentVoxelObj.GetComponent<Voxel>().setDensity3dVN(alivecount);
                    if (alivecount> maxDensity3dVN) {
                        maxDensity3dVN = alivecount;
                    }

                    //UPDATE THE MOORES NEIGHBORHOOD DENSITIES FOR EACH VOXEL//
                    Voxel[] tempNeighborsMO = currentVoxelObj.GetComponent<Voxel>().getNeighbors3dMO();
                    alivecount = 0;
                    foreach (Voxel vox in tempNeighborsMO)
                    {
                        if (vox.GetState() == 1)
                        {
                            alivecount++;
                        }
                    }

                    currentVoxelObj.GetComponent<Voxel>().setDensity3dMO(alivecount);
                    if (alivecount > maxDensity3dMO)
                    {
                        maxDensity3dMO = alivecount;
                    }

                    currentVoxelObj.GetComponent<Voxel>().setDensity3dMO(alivecount);
                    if (alivecount < minDensity3dMO)
                    {
                        minDensity3dMO = alivecount;
                    }
                
                   
                }
            }
        }
    }

    /// <summary>
    /// TESTING VON NEUMANN NEIGHBORS
    /// We can look at the specific voxels above,below,left,right,front,back and color....
    /// We can get all von neumann neighbors and color
    /// </summary>
    /// 
    void VonNeumannLookup()
    {
        //color specific voxel in the grid - [1,1,1]
        GameObject voxel_1 = voxelGrid[1, 1, 1];
        voxel_1.GetComponent<Voxel>().SetState(0);
        voxel_1.GetComponent<Voxel>().VoxelDisplay(0, 0, 1);

        //color specific voxel in the grid - [10,10,10]
        GameObject voxel_2 = voxelGrid[10, 10, 10];
        voxel_2.GetComponent<Voxel>().SetState(0);
        voxel_2.GetComponent<Voxel>().VoxelDisplay(0, 0, 1);

        //get neighbor right and color green
        Voxel voxel_1right = voxel_1.GetComponent<Voxel>().getVoxelRight();
        voxel_1right.SetState(0);
        voxel_1right.VoxelDisplay(0, 0, 1);

        //get neighbor above and color green
        Voxel voxel_1above = voxel_1.GetComponent<Voxel>().getVoxelAbove();
        voxel_1above.SetState(0);
        voxel_1above.VoxelDisplay(0, 0, 1);

        //get neighbor above and color magenta
        Voxel voxel_2above = voxel_2.GetComponent<Voxel>().getVoxelAbove();
        voxel_2above.SetState(0);
        voxel_2above.VoxelDisplay(0, 0, 1);

        //get all VN neighbors of a cell and color yellow
        //color specific voxel in the grid - [12,12,12]
        GameObject voxel_3 = voxelGrid[12, 12, 12];
        Voxel[] tempVNNeighbors = voxel_3.GetComponent<Voxel>().getNeighbors3dVN();
        foreach (Voxel vox in tempVNNeighbors)
        {
            vox.SetState(0);
           // vox.VoxelDisplay(0, 0, 1);
        }

        for(int i = 0; i < 20; i++)
        {

            GameObject voxel_4 = voxelGrid[i, i, i];

            if ( voxel_4 != null)
            {
                Voxel[] tempVNNeighbors2 = voxel_4.GetComponent<Voxel>().getNeighbors3dVN();
                Debug.Log("null");
                foreach (Voxel vox in tempVNNeighbors2)
                {
                    //vox.SetState(1);
                    if (vox.GetState() == 1)
                        vox.VoxelDisplay(0, 0, 1);
                }
            }
           
         }

    }


    void CalculateMeshNew()
    {
        for (int k = timeEnd - 2; k > 2; k--)

        {
            for (int j = length - 2; j > 2; j--)
            {
                for (int i = width - 2; i > 2; i--)
                {

                    GameObject voxel_VN = voxelGrid[i, j, k];
                    //bool[] StateBool = new bool[4];
                    //StateBool[0] = false;
                    //StateBool[1] = false;
                    //StateBool[2] = false;
                    //StateBool[3] = false;

                    Voxel[] VNNeighbMesh2D = new Voxel[4];
                    Voxel left_VN = voxel_VN.GetComponent<Voxel>().getVoxelLeft();
                    Voxel right_VN = voxel_VN.GetComponent<Voxel>().getVoxelRight();
                    Voxel front_VN = voxel_VN.GetComponent<Voxel>().getVoxelFront();
                    Voxel back_VN = voxel_VN.GetComponent<Voxel>().getVoxelBack();
                    Voxel down_VN = voxel_VN.GetComponent<Voxel>().getVoxelBelow();
                    Voxel up_VN = voxel_VN.GetComponent<Voxel>().getVoxelAbove();

                    VNNeighbMesh2D[0] = left_VN;
                    VNNeighbMesh2D[1] = right_VN;
                    VNNeighbMesh2D[2] = front_VN;
                    VNNeighbMesh2D[3] = back_VN;
                    int left = 0;
                    int right = 0;
                    int front = 0;
                    int back = 0;
                    int up = 0;
                    int down = 0;

                    /*
                    {

                        if (left_VN != null)
                        {
                            left = left_VN.GetState();
                            if (left == 1) StateBool[0] = true;
                        }

                        if (right_VN != null)
                        {
                            right = right_VN.GetState();
                            if (right == 1) StateBool[1] = true;
                        }

                        if (front_VN != null)
                        {
                            front = front_VN.GetState();
                            if (front == 1) StateBool[2] = true;
                        }

                        if(back_VN != null)
                        {
                            back = back_VN.GetState();
                            if (back == 1) StateBool[3] = true;
                        }


                        int index = GetTileIndex(StateBool[0], StateBool[1], StateBool[2], StateBool[3]);
                        if (index ==1) voxel_VN.GetComponent<Voxel>().GetComponent<MeshFilter>().mesh = voxel_VN.GetComponent<Voxel>().MeshTable[3];
                        if (index > 1) voxel_VN.GetComponent<Voxel>().GetComponent<MeshFilter>().mesh = voxel_VN.GetComponent<Voxel>().MeshTable[2];
                        if (index <4 && index >18) voxel_VN.GetComponent<Voxel>().GetComponent<MeshFilter>().mesh = voxel_VN.GetComponent<Voxel>().MeshTable[1];
                        else voxel_VN.GetComponent<Voxel>().GetComponent<MeshFilter>().mesh = voxel_VN.GetComponent<Voxel>().MeshTable[1];
                            */
                       if (left_VN != null)
                            left = left_VN.GetState();

                        if (right_VN != null)
                            right = right_VN.GetState();

                        if (front_VN != null)
                            front = front_VN.GetState();

                        if (back_VN != null)
                            back = back_VN.GetState();
                    if (down_VN != null)
                        down = down_VN.GetState();
                    if (up_VN != null)
                        back = up_VN.GetState();



                    if ((left == 1 || right == 1 )&&(front == 0 || back == 0)) voxel_VN.GetComponent<Voxel>().GetComponent<MeshFilter>().mesh = voxel_VN.GetComponent<Voxel>().MeshTable[1];
                    if ((left == 0 || right == 0) &&(front == 1 || back == 1)) voxel_VN.GetComponent<Voxel>().GetComponent<MeshFilter>().mesh = voxel_VN.GetComponent<Voxel>().MeshTable[2];
                    if (left == 0 && right == 0 && front == 0 && back == 0 && up == 0 && down == 0)
                    {

                        voxel_VN.GetComponent<Voxel>().SetState(0);
                        voxel_VN.GetComponent<Voxel>().GetComponent<MeshRenderer>().enabled = false;
                    }
                    //if (left == 0 && right == 1) voxel_VN.GetComponent<Voxel>().GetComponent<MeshFilter>().mesh = voxel_VN.GetComponent<Voxel>().MeshTable[1];
                   // if (left == 1 && right == 0) voxel_VN.GetComponent<Voxel>().GetComponent<MeshFilter>().mesh = voxel_VN.GetComponent<Voxel>().MeshTable[2];





                    //if (left == 0 || right == 1) voxel_VN.GetComponent<Voxel>().VoxelDisplay(0, 1, 1);
                        //if (front == 1 || back == 1) voxel_VN.GetComponent<Voxel>().VoxelDisplay(0, 0, 1);


                   
                   
                }
            }
        }
    }

    void CalculateMesh()
    {
        //for (int k = timeEnd - 2; k > 2; k--)

        {
            for (int j = length - 2; j > 2; j--)
            {
                for (int i = width - 2; i > 2; i--)
                {

                    GameObject voxel_VN = voxelGrid[i, j, 0];

                    Voxel[] VNNeighbMesh2D = new Voxel[4];
                    Voxel left_VN = voxel_VN.GetComponent<Voxel>().getVoxelLeft();
                    Voxel right_VN = voxel_VN.GetComponent<Voxel>().getVoxelRight();
                    Voxel front_VN = voxel_VN.GetComponent<Voxel>().getVoxelFront();
                    Voxel back_VN = voxel_VN.GetComponent<Voxel>().getVoxelBack();
                    VNNeighbMesh2D[0] = left_VN;
                    VNNeighbMesh2D[1] = right_VN;
                    VNNeighbMesh2D[2] = front_VN;
                    VNNeighbMesh2D[3] = back_VN;
                    int left = 0;
                    int right = 0;
                    int front = 0;
                    int back = 0;


                    {

                        if (left_VN != null)
                            left = left_VN.GetState();

                        if (right_VN != null)
                            right = right_VN.GetState();

                        if (front_VN != null)
                            front = front_VN.GetState();

                        if (back_VN != null)
                            back = back_VN.GetState();


                        if (left == 1 || right == 1) voxel_VN.GetComponent<Voxel>().GetComponent<MeshFilter>().mesh = voxel_VN.GetComponent<Voxel>().MeshTable[3];
                        if (front == 1 || back == 1) voxel_VN.GetComponent<Voxel>().GetComponent<MeshFilter>().mesh = voxel_VN.GetComponent<Voxel>().MeshTable[2];

                        //if (left == 0 || right == 1) voxel_VN.GetComponent<Voxel>().VoxelDisplay(0, 1, 1);
                        //if (front == 1 || back == 1) voxel_VN.GetComponent<Voxel>().VoxelDisplay(0, 0, 1);


                    }

                }
            }
        }
    }







    void CalculateMesh2()
    {
        //for (int k = timeEnd - 2; k > 2; k--)
            
        {
            for (int j = length-2; j>0; j--)
            {
                for (int i = width - 2; i > 0; i--)
                {
                    
                    GameObject voxel_VN = voxelGrid[i, j, 0];
                    bool[] StateBool = new bool[4];
                
                    Voxel [] VNNeighbMesh2D = new Voxel [4];
                    Voxel left_VN = voxel_VN.GetComponent<Voxel>().getVoxelLeft();
                    Voxel right_VN = voxel_VN.GetComponent<Voxel>().getVoxelRight();
                    Voxel front_VN = voxel_VN.GetComponent<Voxel>().getVoxelFront();
                    Voxel back_VN = voxel_VN.GetComponent<Voxel>().getVoxelBack();
                    VNNeighbMesh2D[0] = left_VN;
                    VNNeighbMesh2D[1] = right_VN;
                    VNNeighbMesh2D[2] = front_VN;
                    VNNeighbMesh2D[3] = back_VN;
                    int index =0;

                    foreach (Voxel vox in VNNeighbMesh2D)
                    {
                        if (vox != null) continue;


                        if (vox.GetState() == 1 )
                        StateBool[index]=true;
                        else StateBool [index] = false;
                        index++;
                        if (index == 4)
                        {
                            int meshType = GetTileIndex(StateBool[0], StateBool[1], StateBool[2], StateBool[3]);
                            if (meshType < 4) voxel_VN.GetComponent<Voxel>().SetType(1);
                            else if (meshType < 8) voxel_VN.GetComponent<Voxel>().SetType(2);
                            else if (meshType < 16) voxel_VN.GetComponent<Voxel>().SetType(3);
                            else voxel_VN.GetComponent<Voxel>().SetType(4);
                        }
                    }

                   
                }
            }
        }
    }
                    
                   
    void CalculateMeshLook()
    {
        //for (int k = timeEnd - 2; k > 2; k--)

        {
            for (int j = length - 2; j > 0; j--)
            {
                for (int i = width - 2; i > 0; i--)
                {

                    GameObject voxel_VN = voxelGrid[i, j, 0];
                    bool[] StateBool = new bool[4];

                    if (voxel_VN.GetComponent<Voxel>().getVoxelLeft().GetState() == 1 && voxel_VN.GetComponent<Voxel>().getVoxelLeft()!=null) StateBool[0] = true;
                    else StateBool[0] = false;
                    if (voxel_VN.GetComponent<Voxel>().getVoxelRight().GetState()==1 && voxel_VN.GetComponent<Voxel>().getVoxelRight() != null) StateBool[1] = true;
                    else StateBool[1] = false;
                    if (voxel_VN.GetComponent<Voxel>().getVoxelFront().GetState() == 1 && voxel_VN.GetComponent<Voxel>().getVoxelFront() != null) StateBool[2] = true;
                    else StateBool[2] = false;
                    if (voxel_VN.GetComponent<Voxel>().getVoxelBack().GetState() == 1 && voxel_VN.GetComponent<Voxel>().getVoxelBack() != null) StateBool[3] = true;
                    else StateBool[3] = false;

                        
                            int meshType = GetTileIndex(StateBool[0], StateBool[1], StateBool[2], StateBool[3]);
                            if (meshType < 4) voxel_VN.GetComponent<Voxel>().SetType(1);
                            else if (meshType < 8) voxel_VN.GetComponent<Voxel>().SetType(2);
                            else if (meshType < 16) voxel_VN.GetComponent<Voxel>().SetType(3);
                            else voxel_VN.GetComponent<Voxel>().SetType(4);

                    }


                }
            }
        }
   






  

    /// <summary>
    /// TESTING MOORES NEIGHBORS
    /// We can look at the specific voxels above,below,left,right,front,back and color....
    /// We can get all von neumann neighbors and color
    /// </summary>
    /// 
    void MooreLookup()
    {
        //get all MO neighbors of a cell and color CYAN
        //color specific voxel in the grid - [14,14,14]
        GameObject voxel_1 = voxelGrid[14, 14, 14];
        Voxel[] tempMONeighbors = voxel_1.GetComponent<Voxel>().getNeighbors3dMO();
        foreach (Voxel vox in tempMONeighbors)
        {
            vox.SetState(1);
            vox.VoxelDisplay(0, 1, 1);
        }

    }

    // Separate data based on density (getDensity3dMO)
    void SeparateVoxelsByDensity()
    {
        // Get all the stored desnities from the voxels
        List<int> availableDensities = new List<int>();
        for (int i = 1; i < width - 1; i++)
        {
            for (int j = 1; j < length - 1; j++)
            {
                for (int k = 1; k < height - 1; k++)
                {
                    Voxel currentVoxel = voxelGrid[i, j, k].GetComponent<Voxel>();
                    int currentVoxelDensity = currentVoxel.getDensity3dMO();
                    if (availableDensities.Contains(currentVoxelDensity) == false)
                    {
                        availableDensities.Add(currentVoxelDensity);
                    }
                }
            }
        }

        // Split the data into list that contain all the voxels with same density
        for (int l = 0; l < availableDensities.Count; l++)
        {
            List<GameObject> voxelsWithCurrentDensity = new List<GameObject>();
            int currentDensity = availableDensities[l];

            for (int i = 1; i < width - 1; i++)
            {
                for (int j = 1; j < length - 1; j++)
                {
                    for (int k = 1; k < height - 1; k++)
                    {
                        Voxel currentVoxel = voxelGrid[i, j, k].GetComponent<Voxel>();
                        int currentVoxelDensity = currentVoxel.getDensity3dMO();
                        if (currentVoxelDensity == currentDensity)
                        {
                            voxelsWithCurrentDensity.Add(currentVoxel.gameObject);
                        }
                    }
                }
            }

            voxelsByDensity.Add(voxelsWithCurrentDensity);

        }

    }
    void ExportPrepare()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < length; j++)
            {
                for (int k = 0; k < height; k++)
                {
                    Voxel currentVoxel = voxelGrid[i, j, k].GetComponent<Voxel>();
                    if (currentVoxel.GetState() == 0)
                    {
                        Destroy(currentVoxel.gameObject);
                    }
                }
            }
        }
    }

    private int GetTileIndex(bool b0, bool b1, bool b2, bool b3)
    {
        int result = 0;
        if (b0) result |= 1;
        if (b1) result |= 2;
        if (b2) result |= 4;
        if (b3) result |= 8;
        return result;

    }

    private void ExportToText()
    {
        string exportfilepath = "C:/Users/shahrzad/Documents";
        string exportfilename = DateTime.Now.ToString("yyMMdd_hm") + "_exportvoxels.txt";
        //exportfilename = String.Format("{0:yyyy-MM-dd}_{1}", DateTime.Now, exportfilename);

        //StreamWriter writer = File.CreateText(filepath+filename);
        using (StreamWriter writer = File.CreateText(exportfilepath + exportfilename))
        {


            for (int i = 1; i < width - 1; i++)
            {
                for (int j = 1; j < length - 1; j++)
                {
                    for (int k = 1; k < currentFrame; k++)
                    {
                        GameObject currentVoxelObj = voxelGrid[i, j, k];
                        Voxel vox = currentVoxelObj.GetComponent<Voxel>();
                        int state = vox.GetState();
                        if (state != 0)
                        {
                            Vector3 position = vox.address;
                            int densityMO = vox.getDensity3dMO();
                            int densityVN = vox.getDensity3dVN();
                            int age = vox.GetAge();

                            writer.WriteLine("voxelstate " + Convert.ToString(state));
                            writer.WriteLine("voxeldensitymo " + Convert.ToString(densityMO));
                            writer.WriteLine("voxeldensityvn " + Convert.ToString(densityVN));
                            writer.WriteLine("voxelage " + Convert.ToString(age));
                            writer.WriteLine(Convert.ToString(position.x) + "," + Convert.ToString(position.y) + "," + position.z);
                            writer.Flush();
                        }
                    }
                }
            }
        }
    }




}