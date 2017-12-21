using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//trinity parcero
//scenemanager. handles setup and scene-level stuff
public class SceneManager : MonoBehaviour {

    //OBJECT REFS
    //ref to jellyfish
    public Jellyfish jelObject;
    //ref to whale
    public Whale whaleObject;
    //ref to fish
    public Fish fishObject;
    //ref to shiny
    public Shiny shinyPrefab;
    //ref to plant
    public GameObject plantPrefab;
    //ref to center cube
    public GameObject cube;

    //OBJECTS
    //actual centercube object
    GameObject centerCube;

    //flockcenter object for camera to follow
    public GameObject flockCenter;
    //flockfront object for camera to follow
    //public GameObject flockFront;

    //current shiny sphere
    public Shiny shiny;

    //LISTS OF STUFF
    //list of jellyfish
    public List<Jellyfish> jellies;
    //whales n waypoints
    public List<Whale> whales;
    public List<GameObject> wayPoints;
    //list of fish
    public List<Fish> fish;
    //list of obstacles
    public List<GameObject> obstacles;
    //plants
    public List<GameObject> plants;
    //list of resistance areas
    public List<GameObject> resAreas;

    //RESISTANCE
    public float resCoeff;

	// Use this for initialization
	void Start ()
    {
        //set up
        jellies = new List<Jellyfish>();
        whales = new List<Whale>();
        plants = new List<GameObject>();
        //obstacles = new List<GameObject>();

        //generate stuff
        NewShiny();
        centerCube = Instantiate(cube);
        GenerateJellies();
        GenerateWhales();
        GeneratePlants();
        

        //camera gameobjects
        MoveToCenter(flockCenter);
        MoveToCenter(centerCube);
        //MoveToCenter(flockFront);
    }
	
	// Update is called once per frame
	void Update ()
    {
        //draw debug lines
        //DIRECTION LINE IS NOT TO SCALE WITH ACTUAL MAGNITUDE! I MADE IT BIG SO ITS MORE VISIBLE!
        Debug.DrawLine(centerCube.transform.position, centerCube.transform.position + AverageFlockDirection(), Color.black);

        foreach(Whale w in whales)
        {
            Debug.DrawLine(w.vehiclePosition, w.velocity * 20, Color.red);
        }

        MoveToCenter(centerCube);
        MoveToCenter(flockCenter);
        //MoveToCenter(flockFront);

        ResistanceField(resCoeff);
    }

    public void NewShiny()
    {
        shiny = Instantiate(shinyPrefab);
        shiny.sceneMan = this;
        for(int i = 0; i < jellies.Count; i++)
        {
            jellies[i].target = shiny;
        }
        //keep location somewhat nearby flock so they can possibly reach it
        float randX;
        float randZ;
        if(flockCenter.transform.position.x > 700)
        {
            randX = flockCenter.transform.position.x - Random.Range(50, 150);
        }
        else if(flockCenter.transform.position.x < 100)
        {
            randX = flockCenter.transform.position.x + Random.Range(50, 150);
        }
        else
        {
            randX = flockCenter.transform.position.x + Random.Range(-100, 150);
        }
        if(flockCenter.transform.position.z > 700)
        {
            randZ = flockCenter.transform.position.z - Random.Range(50, 150);
        }
        else if(flockCenter.transform.position.z < 100)
        {
            randZ = flockCenter.transform.position.z + Random.Range(50, 150);
        }
        else
        {
            randZ = flockCenter.transform.position.z + Random.Range(-100, 150);
        }
        
        //check terrainheight at random location and set y accordingly
        float terrainHeight = Terrain.activeTerrain.SampleHeight(new Vector3(randX, 0f, randZ));
        shiny.transform.position = new Vector3(randX, terrainHeight + 100f, randZ);
    }

    //generate jellies. makes a bunch of jellyfish in a certain area
    public void GenerateJellies()
    {
        int randNum = Random.Range(20, 41);
        //instantiate 20 to 40 jellies at random positions within a certain area
        for (int i = 0; i < randNum; i++)
        {
            jellies.Add(Instantiate(jelObject));
            float randX = Random.Range(200, 300);
            float randZ = Random.Range(200, 300);
            //check terrainheight at random location and set y accordingly
            float terrainHeight = Terrain.activeTerrain.SampleHeight(new Vector3(randX, 0f, randZ));
            jellies[i].transform.position = new Vector3(randX, terrainHeight + 10f, randZ);
            jellies[i].sceneMan = this;
            jellies[i].target = shiny;
        }
    }
   
    //generate whales. makes a couple whales at a waypoint
    public void GenerateWhales()
    {
        for(int i = 0; i < 4; i++)
        {
            whales.Add(Instantiate(whaleObject));
            float terrainHeight = Terrain.activeTerrain.SampleHeight(wayPoints[i].transform.position);
            whales[i].transform.position = wayPoints[i].transform.position;
            whales[i].sceneMan = this;
            whales[i].currentWP = i+1;
        }
        
    }

    //generate plants. puts some plants in the environment. randomly varies scale and rotation a bit
    public void GeneratePlants()
    {
        for(int i = 0; i < 20; i++)
        {
            plants.Add(Instantiate(plantPrefab));
            float randX = Random.Range(20, 700);
            float randZ = Random.Range(20, 700);
            float terrainHeight = Terrain.activeTerrain.SampleHeight(new Vector3(randX, 0, randZ));
            plants[i].transform.position = new Vector3(randX, terrainHeight + 8f, randZ);
            //random scale
            float scaleX = Random.Range(10, 40);
            float scaleY = Random.Range(10, 40);
            float scaleZ = Random.Range(10, 40);
            plants[i].transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
            //random rotation
            float rotX = Random.Range(0, 20);
            float rotY = Random.Range(0, 360);
            float rotZ = Random.Range(0, 20);
            plants[i].transform.rotation = new Quaternion(rotX, rotY, rotZ, 0);
        }
    }

    //movetocenter. moves an object to the flocks average loc
    public void MoveToCenter(GameObject obj)
    {
        Vector3 desiredPosition = new Vector3();
        //get flocks average position
        //add up flocks positions
        for (int i = 0; i < jellies.Count; i++)
        {
            desiredPosition += jellies[i].vehiclePosition;
        }
        //divide by count of flock
        desiredPosition = desiredPosition / jellies.Count;

        obj.transform.position = desiredPosition;
    }

    //averageFlockDirection. gets the average flock direction
    public Vector3 AverageFlockDirection()
    {
        Vector3 desiredVelocity = new Vector3();

        //get the flocks average direction
        //add up flocks velocity
        for (int i = 0; i < jellies.Count; i++)
        {
            desiredVelocity += jellies[i].velocity;
        }
        //divide by count of flock
        desiredVelocity = desiredVelocity / jellies.Count;

        return desiredVelocity.normalized * 20; //is scaled to make it more visible when drawn
    }

    //resistanceField. checks to see if any vehicles are intersecting the resistance fields
    //and if they are, slows them down by applying a force opposite their velocity
    public void ResistanceField(float coeff)
    {
        Vector3 resistance;
        for(int resIt = 0; resIt < resAreas.Count; resIt++)
        {
            //jellies
            for (int i = 0; i < jellies.Count; i++)
            {
                //check for collision
                if ((resAreas[resIt].transform.position - jellies[i].vehiclePosition).sqrMagnitude < Mathf.Pow((resAreas[resIt].transform.localScale.x /2), 2))
                {
                    //drag effect
                    resistance = jellies[i].velocity * -1;
                    resistance.Normalize();
                    resistance = resistance * coeff;
                    jellies[i].ultForce += resistance;
                }
            }
            //whales
            for (int j = 0; j < whales.Count; j++)
            {
                //check for collision
                if ((resAreas[resIt].transform.position - whales[j].vehiclePosition).sqrMagnitude < Mathf.Pow((resAreas[resIt].transform.localScale.x / 2), 2))
                {
                    //drag effect
                    resistance = whales[j].velocity * -1;
                    resistance.Normalize();
                    resistance = resistance * coeff;
                    whales[j].ultForce += resistance;
                }
            }
            //fish
            for (int k = 0; k < fish.Count; k++)
            {
                //check for collision
                //check for collision
                if ((resAreas[resIt].transform.position - fish[k].vehiclePosition).sqrMagnitude < Mathf.Pow((resAreas[resIt].transform.localScale.x / 2), 2))
                {
                    //drag effect
                    resistance = fish[k].velocity * -1;
                    resistance.Normalize();
                    resistance = resistance * coeff;
                    fish[k].ultForce += resistance;
                }
            }
        }
        
    }
    
}
