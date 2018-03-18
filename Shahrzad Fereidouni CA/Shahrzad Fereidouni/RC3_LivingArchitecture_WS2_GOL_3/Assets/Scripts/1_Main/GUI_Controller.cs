using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GUI_Controller : MonoBehaviour {

    public GUISkin mySkin;
    public int score = 0;
    public int age = 0;
    public int density = 0;
    public int frame = 0;
    public Texture2D seeds;

    /*
    private void Update()
    {
        // Load 2d CA
        if (Input.GetKeyDown("1"))
        {
            SceneManager.LoadScene("CA_2d");
            DynamicGI.UpdateEnvironment();
        }

        // Load 2d CA with history record
        if (Input.GetKeyDown("2"))
        {
            SceneManager.LoadScene("CA_2d_history");
            DynamicGI.UpdateEnvironment();
        }

        // Load 3d CA
        if (Input.GetKeyDown("3"))
        {
            SceneManager.LoadScene("CA_3d");
            DynamicGI.UpdateEnvironment();
        }

        // Reset current scene
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            DynamicGI.UpdateEnvironment();
        }

        // Quit application
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
*/
    private void OnGUI()
    {
         //Set a label for our game
        GUI.skin = mySkin;
        
        //GUI.Label(new Rect(new Vector2(10,30), new Vector2(200,100)), "Origin Image: ");
        

        // Set the population count 
        Scene scene = SceneManager.GetActiveScene();
        
        if (scene.name == "1_Main")
        {
            score = GameObject.Find("Environment").GetComponent<Environment>().AliveCells();
            age = GameObject.Find("Environment").GetComponent<Environment>().MaxAge();
            frame = GameObject.Find("Environment").GetComponent<Environment>().CurrentFrame();
            density = (int) (GameObject.Find("Environment").GetComponent<Environment>().Density()*100);
            //seeds = GameObject.Find("Environment").GetComponent<Environment>().seed();
        }
        GUI.Label(new Rect(new Vector2(130, 25), new Vector2(100, 100)), "Population: " + score.ToString());
        GUI.Label(new Rect(new Vector2(130, 50), new Vector2(300,300)), "Oldest: " + age.ToString());
        GUI.Label(new Rect(new Vector2(130, 75), new Vector2(300, 100)), "Iteration: " + frame.ToString());
        GUI.Label(new Rect(new Vector2(130, 100), new Vector2(300, 100)), "Density: " + density.ToString()+ "%");
        // GUI.Label(new Rect(new Vector2(Screen.width - 175, 50), new Vector2(300,100)), "Origin " + seeds);
        
    }

}
