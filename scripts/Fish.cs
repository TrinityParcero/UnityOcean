using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//trinity parcero. fish. flow field followers
public class Fish : Vehicle
{
    public Vector3 ultForce;

    //force weights
    public float stayInBoundsW;
    public float stayAboveTerrainW;
    public float avoidObstaclesW;
    public float seekW;

    // Use this for initialization
    void Start ()
    {
        ultForce = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
