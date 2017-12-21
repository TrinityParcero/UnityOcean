using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CycleCamera : MonoBehaviour {

    //fields
    public GameObject[] cameras;
    public GameObject fullView;
    public GameObject averagePosition;
    public GameObject frontFollow;
    public string currentView;
    public int camIndex;
    

	// Use this for initialization
	void Start ()
    {
        
        cameras = new GameObject[6];
        camIndex = 0;
        cameras[0] = fullView;
        cameras[1] = averagePosition;
        cameras[2] = frontFollow;

        //set all other cams to disabled
        for(int i = 0; i < cameras.Length; i++)
        {
            if(i != camIndex)
            {
                cameras[i].SetActive(false);
            }
            
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        //update currentview so gui can display name of view
        if(camIndex == 0) { currentView = "Full View"; }
        if(camIndex == 1) { currentView = "Center View"; }
        if(camIndex == 2) { currentView = "Front View"; }



        //if c is pressed, switch camera
        if (Input.GetKeyDown(KeyCode.C))
        {
            cameras[camIndex].SetActive(false);

            //if we're at the end of the array, reset
            if(camIndex == 2)
            {
                cameras[0].SetActive(true);
                camIndex = 0;
            }

            else
            {
                cameras[camIndex + 1].SetActive(true);
                camIndex++;
            }
        }
	}

    private void OnGUI()
    {
        GUI.Box(new Rect(50, 50, 200, 50), "Press C to switch camera! \n Current view: " + currentView);
    }
}
