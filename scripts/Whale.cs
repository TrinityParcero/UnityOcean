using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//trinity parcero
//whale. path followers
public class Whale : Vehicle
{
    //index of wp currently being seeked
    public int currentWP;

    public Vector3 ultForce;

    //force weights
    public float stayInBoundsW;
    public float stayAboveTerrainW;
    public float seekW;
    public float separateW;
    public float avoidObsW;

    // Use this for initialization
    void Start ()
    {
        ultForce = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update ()
    {
        vehiclePosition = transform.position;

        SetCurrentWP();
        ultForce += FollowPath().normalized * seekW;
        ultForce += AvoidObstaclesBig().normalized * avoidObsW;
        ultForce += StayInBounds().normalized * stayInBoundsW;
        ultForce += StayAboveTerrain().normalized * stayAboveTerrainW;
        ultForce += SeparateWhales(sceneMan.whales).normalized * separateW;
        
        

        ApplyForce(ultForce);

        //move, update position, zero out ultForce
        RotateToFace();
        Move();
        transform.position = vehiclePosition;
        ultForce = Vector3.zero;
    }

    //setcurrentwp. set current waypoint. basically an arrive method
    public void SetCurrentWP()
    {
        if(currentWP < sceneMan.wayPoints.Count-1)
        {
            //check to see if we're close enough to current wp to switch
            if ((vehiclePosition - sceneMan.wayPoints[currentWP].transform.position).sqrMagnitude < 200)
            {
                currentWP++;
            }
        }
        else
        {
            //if current is almost to count, reset to 0
            currentWP = 0;
        }
    }

    //followpath. simple path following. seeks a series of waypoints
    public Vector3 FollowPath()
    {
        return Seek(sceneMan.wayPoints[currentWP].transform.position);
    
    }

    //separation. whale version
    public Vector3 SeparateWhales(List<Whale> flock)
    {
        float flockRadius = 80f;
        for (int i = 0; i < flock.Count; i++)
        {
            if ((flock[i].vehiclePosition - vehiclePosition).sqrMagnitude < Mathf.Pow(flockRadius, 2))
            {
                return(Flee(sceneMan.whales[i].vehiclePosition));
            }
            else
            {
                return Vector3.zero;
            }
        }

        return Vector3.zero;
    }

    //avoid obstacles. version for whales
    public Vector3 AvoidObstaclesBig()
    {
        float avoidRadius = 100;
        Vector3 vectorToObsCenter;
        foreach (GameObject obstacle in sceneMan.obstacles)
        {
            avoidRadius = obstacle.transform.localScale.z *50;
            //sort out obstacles that are too far forward or behind
            vectorToObsCenter = obstacle.transform.position - vehiclePosition;
            if (Vector3.Dot(vectorToObsCenter, transform.forward) > 0 &&
                vectorToObsCenter.sqrMagnitude < Mathf.Pow(avoidRadius, 2))
            {
                float dotProd = Vector3.Dot(vectorToObsCenter, transform.right);

                //check if distance right or left is too far left or right
                if (Mathf.Pow(dotProd, 2) < Mathf.Pow(avoidRadius, 2))
                {
                    //if its to left, desired velocity is right
                    if (dotProd < 0)
                    {
                        return(Seek(transform.right));
                    }

                    //if its to right, desired velocity is left
                    else
                    {
                        return(Seek(-transform.right));
                    }
                }

                //check if obstacle is close below
                else if (Mathf.Abs(vehiclePosition.y - obstacle.transform.position.y) < 50)
                {
                    return (Seek(transform.up));
                }

                else
                {
                    return Vector3.zero;
                }
            }
            else
            {
                return Vector3.zero;
            }


        }

        return Vector3.zero;
    }
}
