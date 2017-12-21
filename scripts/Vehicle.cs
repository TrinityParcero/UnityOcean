using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//trinity parcero
//controls vehicle movement and rotation
public class Vehicle : MonoBehaviour
{
    public SceneManager sceneMan;

    //fields
    public Vector3 vehiclePosition;
    public Vector3 velocity;
    public Vector3 acceleration;
    public Vector3 direction;

    public float maxSpeed;
    public float mass;
    public float maxForce;
    public float radius;
    


    // Use this for initialization
    void Start()
    {
        vehiclePosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }

    //moves the vehicle based on acceleration, velocity, etc
    public void Move()
    {
        //movement
        velocity += acceleration * Time.deltaTime;
        vehiclePosition += velocity * Time.deltaTime;

        //direction vector is normalized velocity
        direction = velocity.normalized;

        //transform.Rotate(Vector3.up, RotateToFace(direction));
        //zero out acceleration
        acceleration = Vector3.zero;
    }



    //applies a given force
    public void ApplyForce(Vector3 force)
    {
        acceleration += force / mass;
    }

    //vehicles seek center if they get out of bounds
    public Vector3 StayInBounds()
    {
        //edges
        if (vehiclePosition.x > 800 ||
           vehiclePosition.x < -800 ||
           vehiclePosition.z > 800 ||
           vehiclePosition.z < -800)
        {
            return(Seek(new Vector3(0, 30, 0)));
        }
        else
        {
            return Vector3.zero;
        }
    }

    //stay above terrain. keeps vehicles above the terrain. also keeps them from going too high
    public Vector3 StayAboveTerrain()
    {
        float terrainHeight = Terrain.activeTerrain.SampleHeight(vehiclePosition);
        float upperBounds = 250f;
        float lowerBounds = terrainHeight + 20f;

        if (vehiclePosition.y > upperBounds)
        {
            return (Seek(vehiclePosition + -Vector3.up));
        }
        else if(vehiclePosition.y < lowerBounds)
        {
            return(Seek(vehiclePosition + Vector3.up));
        }
        else
        {
            return Vector3.zero;
        }
        
    }

    //seek method. returns steering force
    public Vector3 Seek(Vector3 targetPos)
    {
        //desired velocity = targetpos - currentpos
        Vector3 desiredVelocity = targetPos - vehiclePosition;

        //scale desired velocity by maxspeed
        desiredVelocity = desiredVelocity.normalized * maxSpeed;
        
        //steering force = desired - current velocity
        Vector3 steerForce = desiredVelocity - velocity;
        
        //return the steering force
        return steerForce;
    }

    //pursue method. seeks a humans future position
    public void Pursue(Vehicle v)
    {
        //seek the humans position plus 2 times its current velo
        ApplyForce(Seek(v.vehiclePosition + v.velocity)*1.25f);
    }

    //evade method. flees a zombies future position
    public void Evade(Vehicle v)
    {
        ApplyForce(Flee(v.vehiclePosition + v.velocity)*5f);
    }

    //supposed to make things rotate to face direction vector. returns rotation
    //finally works!! yay!
    public void RotateToFace()
    {
        
        
        float angle = Mathf.Atan2(velocity.x, velocity.z) * Mathf.Rad2Deg;

        //set rotations angle as an Euler rotation
        transform.rotation = Quaternion.Euler(0, angle-180, 0);
    }

    //flee method. returns steering force
    public Vector3 Flee(Vector3 targetPos)
    {
        //desired velocity is currentpos - targetpos
        Vector3 desiredVelocity = vehiclePosition - targetPos;
        
        desiredVelocity = desiredVelocity.normalized * maxSpeed;
        
        Vector3 steerForce = desiredVelocity - velocity;
        steerForce.y = 0;
        
        return steerForce;
    }
    

    //avoid obstacles. version for fish and jellyfish
    public Vector3 AvoidObstaclesSmall()
    {
        float avoidRadius = 80;
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
                        return((Seek(-transform.right)));
                    }
                }

                else
                {
                    return Vector3.zero;
                }

                //check if obstacle is close below
                if (Mathf.Abs(vehiclePosition.y - obstacle.transform.position.y) < 20)
                {
                    return(Seek(transform.up));
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
    //separation. keeps distance between individual flockers
    public Vector3 Separate(List<Jellyfish> flock)
    {
        float flockRadius = 15f;

           for(int i = 0; i < flock.Count; i++)
           {
                if ((flock[i].vehiclePosition - vehiclePosition).sqrMagnitude < Mathf.Pow(flockRadius, 2))
                {
                    return(Flee(sceneMan.jellies[i].vehiclePosition));
                }
           }

        return Vector3.zero;
        
    }
    
    //align. seek average direction vector of flock
    public Vector3 Align(List<Jellyfish> flock)
    {
        Vector3 desiredVelocity = new Vector3();

        //get the flocks average direction
        //add up flocks velocity
        for(int i = 0; i < flock.Count; i++)
        {
            desiredVelocity += flock[i].velocity;
        }
        //divide by count of flock
        desiredVelocity = desiredVelocity / flock.Count;
        //normalize result
        desiredVelocity.Normalize();

        return Seek(vehiclePosition + desiredVelocity);
        
        
    }

    //cohere. seek average position vector of flock (the center)
    public Vector3 Cohere(List<Jellyfish> flock)
    {
        Vector3 desiredPosition = new Vector3();
        //get flocks average position
        //add up flocks positions
        for(int i = 0; i < flock.Count; i++)
        {
            desiredPosition += flock[i].vehiclePosition;
        }
        //divide by count of flock
        desiredPosition = desiredPosition / flock.Count;
        //seek that position
        return Seek(desiredPosition);
    }
    
    //code heavily based on gamedevelopment.tutsplus.com/tutorials/understanding-steering-behaviors-wander--gamedev-1624
    //wander. default semi-random movement
    public Vector3 Wander()
    {
        float wanderRadius = 8f;

        //vector from vehicle position to center of circle
        Vector3 circleCenter = transform.forward * 20;

        //straight ahead radius displacements are measured in relation to
        Vector3 straightRadius = circleCenter + velocity.normalized * wanderRadius;
        
        //get a random angle
        float randomRotation = Random.Range(0, 361);

        //displacement vector is scaled by the radius
        Vector3 dis = circleCenter * wanderRadius;
        dis.x = Mathf.Cos(randomRotation) * wanderRadius;
        dis.y = Mathf.Tan(randomRotation) * wanderRadius;
        dis.z = Mathf.Sin(randomRotation) * wanderRadius;
        
        return(Seek((circleCenter + dis)));
        
    }
}
