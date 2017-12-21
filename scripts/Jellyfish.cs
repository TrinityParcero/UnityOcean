using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//trinity parcero
//jellyfish. flockers
public class Jellyfish : Vehicle {

    //ultimate force
    public Vector3 ultForce;

    //force weights
    public float alignW;
    public float cohereW;
    public float stayInBoundsW;
    public float stayAboveTerrainW;
    public float seekW;
    public float avoidObsW;
    public float separateW;

    //seek target
    public Shiny target;

	// Use this for initialization
	void Start ()
    {
        ultForce = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update ()
    {
        vehiclePosition = transform.position;
        
        ultForce += StayInBounds().normalized * stayInBoundsW;
        ultForce += StayAboveTerrain().normalized * stayAboveTerrainW;

        //flocking stuff
        ultForce += Separate(sceneMan.jellies).normalized * separateW;
        ultForce += Align(sceneMan.jellies).normalized * alignW;
        ultForce += Cohere(sceneMan.jellies).normalized * cohereW;

        ultForce += Seek(target.vehiclePosition).normalized * seekW;
        ultForce += AvoidObstaclesSmall().normalized * avoidObsW;

        ApplyForce(ultForce);

        //move, update position, zero out ultForce
        Move();
        transform.position = vehiclePosition;
        ultForce = Vector3.zero;
    }
}
