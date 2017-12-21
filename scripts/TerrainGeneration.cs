using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TerrainGeneration class
// Placed on a terrain game object
// Generates a Perlin noise-based heightmap

public class TerrainGeneration : MonoBehaviour 
{

	private TerrainData myTerrainData;
    public int resolution = 129;
    public Vector3 worldSize;
	
    public float timeStep = 0.5f;
	float[,] heightArray;


	void Start () 
	{
		myTerrainData = gameObject.GetComponent<TerrainCollider>().terrainData;

		myTerrainData.heightmapResolution = resolution;
        worldSize = new Vector3(800, 200, 800);
        myTerrainData.size = worldSize;
        heightArray = new float[resolution, resolution];

		// Fill the height array with values!
		// Uncomment the Ramp and Perlin methods to test them out!
		//Flat(1.0f);
	    //Ramp();
		Perlin();

		// Assign values from heightArray into the terrain object's heightmap
		myTerrainData.SetHeights (0, 0, heightArray);
	}
	

	void Update () 
	{
		
	}

	/// <summary>
	/// Flat()
	/// Assigns heightArray identical values
	/// </summary>
	void Flat(float value)
	{
		// Fill heightArray with 1's
		for(int i = 0; i < resolution; i++)
		{
			for(int j = 0; j < resolution; j++)
			{
                heightArray[i, j] = 0;
			}
		}
	}
		

	/// <summary>
	/// Ramp()
	/// Assigns heightsArray values that form a linear ramp
	/// </summary>
	void Ramp()
	{
        // Fill heightArray with linear values
        for (int i = 0; i < resolution; i++)
        {
            for (int j = 0; j < resolution; j++)
            {
                float value = (float)j / (float)resolution;
                heightArray[i, j] = value;
            }
        }

    }

	/// <summary>
	/// Perlin()
	/// Assigns heightsArray values using Perlin noise
	/// </summary>
	void Perlin()
	{
        // Fill heightArray with Perlin-based values
        float xCoord = 1f;
        float yCoord = 1f;
        for(int i = 0; i < resolution; i++)
        {
            yCoord = yCoord + timeStep;

            //reset column
            xCoord = 1f;

            for(int j = 0; j< resolution; j++)
            {
                xCoord = xCoord + timeStep;
                float sampleVal = Mathf.PerlinNoise(xCoord, yCoord);
                heightArray[i, j] = sampleVal;
            }
        }



	}

}
