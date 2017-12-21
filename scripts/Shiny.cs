using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//trinity parcero
//shiny. controls the shiny sphere
//which jellyfish flock around and follow
public class Shiny : Vehicle
{

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        vehiclePosition = transform.position;
        DropShiny();
        Move();
        transform.position = vehiclePosition;
    }

    //moveShiny. moves the sphere that the jellyfish seek to a random location
    //not really useful anymore but eh
    public void MoveShiny()
    {
        transform.position =
        new Vector3(Random.Range(-100, 101), Random.Range(200, 300), Random.Range(-100, 101));
    }

    //dropShiny. simulates the shiny sphere falling and shrinking as it approaches the bottom
    public void DropShiny()
    {
        Vector3 gravity = new Vector3(0, -1f, 0);
        int counter = 0;
        float terrainHeight = Terrain.activeTerrain.SampleHeight(vehiclePosition);
        if (sceneMan.shiny.vehiclePosition.y > terrainHeight + 10)
        {
            //falling motion
            ApplyForce(gravity);
            //shrinking effect
            if(counter == 40)
            {
                transform.localScale = transform.localScale * .9f;
                counter = 0;
            }
            counter++;
            
        }
        else
        {
            //make a new shiny object
            sceneMan.NewShiny();
            Destroy(this.gameObject);
            counter = 0;
        }

    }
}
