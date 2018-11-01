using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeController : MonoBehaviour {
    public GameObject cellPrefab;
    public GameObject spawnReference;
    public GameObject east;
    public GameObject south;
    public GameObject southTrigger;

    public float bias; // bias of the maze
    public int width;  // Adjustable width of maze
    private int length; // tracks length of the maze
    private bool initialTrigger; // bool to see if the maze generation has started

    private int setNum; // keeps track of # of sets created
    private Cell[] cells; // stores cell info like set number, eastWall, and southWall
    private GameObject[] eastWalls; // holds the east Walls that get created

    // Initialize everything
    private void Start()
    {
        setNum = 0;
        length = 1;
        initialTrigger = false;

        cells = new Cell[width];
        for (int i = 0; i < width; i++)
        {
            cells[i] = new Cell();
        }
        eastWalls = new GameObject[width];
        for(int i = 0; i < width; i++)
        {
            eastWalls[i] = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only create the first Row the first time the player enters the maze Trigger
        if (!initialTrigger) {
            if (other.gameObject.CompareTag("Player"))
            {
                createRow();
                initialTrigger = true;
            }
        }
    }

    // Eller's Algorithm:
    // http://www.neocomputer.org/projects/eller.html
    public void createRow()
    {
        // if a cell has a bottom wall, remove it from its set
        for (int i = 0; i < width; i++)
        {
            if (cells[i].southWall)
            {      
                cells[i].set = -1;
            }
        }

        for (int i = 0; i < width; i++)
        {
            // make all independent cells their own set
            if (cells[i].set == -1)
            {
                cells[i].set = setNum++;  
            }

            // initialize arrays
            cells[i].southWall = false;
            cells[i].eastWall = false;
            eastWalls[i] = null;
        }

        // Randomly add or union disjoint sets in a row
        for (int i = 0; i < width - 1; i++)
        {
            // if two cells are members of same set, we MUST add a wall
            if (cells[i].set == cells[i + 1].set || Random.value < bias)
            {   
                cells[i].eastWall = true;
            }
            else
            {
                unionSets(cells, i, i + 1);
            }
        }

        // Create bottom walls, track the # of vertical passages. 
        // If only 1 vertical passage in a set do NOT create a wall.
        Dictionary<int, int> numPassages = new Dictionary<int, int>(); 
        for (int i = 0; i < width; i++)
        {
            if (!numPassages.ContainsKey(cells[i].set))
            {
                numPassages[cells[i].set] = 1;
            }
            else
            {
                numPassages[cells[i].set] = numPassages[cells[i].set] + 1;
            }
        }
        for (int i = 0; i < width; i++)
        {
            if (numPassages[cells[i].set] > 1 && Random.value > bias)
            {
                cells[i].southWall = true;
                numPassages[cells[i].set] = numPassages[cells[i].set] - 1;
            }
        }

        instantiateRow();   // call method to instantiate the prefabs 
        length++; // increment the length of the maze by 1
    }

    // Looks at the data stored in cell array and creates walls based on if cells have walls
    public void instantiateRow()
    {
        for (int i = 0; i < width; i++)
        {
            // Construct width amount of cells
            Instantiate(
                         cellPrefab,
                         spawnReference.GetComponent<Transform>().position + new Vector3(i * 2.5f, 0, -length * 2.5f),
                         spawnReference.GetComponent<Transform>().rotation);

            // Construct the inner walls
            if (cells[i].eastWall)
            {
                var eastWall = Instantiate(
                                             east,
                                             spawnReference.GetComponent<Transform>().position + new Vector3(i * 2.5f + 1.5f, 1.5f, -length * 2.5f),
                                             spawnReference.GetComponent<Transform>().rotation);
                eastWalls[i] = eastWall;    // Add to an array so we can delete them if needed upon terminating the maze.
                if (length == 1)    // first row of inner walls are indestructible
                {
                    eastWall.GetComponent<Wall>().hp = -1;
                }
            }
            if (cells[i].southWall)
            {
                var southWall = Instantiate(
                            south,
                            spawnReference.GetComponent<Transform>().position + new Vector3(i * 2.5f, 1.5f, -length * 2.5f - 1.15f),
                            spawnReference.GetComponent<Transform>().rotation);
                if (length == 1)    // first row of inner walls are indestructible
                {
                    southWall.GetComponent<Wall>().hp = -1;
                }
            }

            // Construct the outer walls
            if (i == 0)
            {
               var westWall = (GameObject) Instantiate(
                             east,
                             spawnReference.GetComponent<Transform>().position + new Vector3(i * 2.5f - 1.5f, 1.5f, -length * 2.5f),
                             spawnReference.GetComponent<Transform>().rotation);
                
                   westWall.GetComponent<Wall>().hp = -1;
                
            }
            if (i == width - 1) 
            {
                var eastWall = (GameObject) Instantiate(
                            east,
                            spawnReference.GetComponent<Transform>().position + new Vector3(i * 2.5f + 1.5f, 1.5f, -length * 2.5f),
                            spawnReference.GetComponent<Transform>().rotation);

                    eastWall.GetComponent<Wall>().hp = -1;

            }
        }

        // construct the trigger for the next row
        var trigger = (GameObject)Instantiate(
                                             southTrigger,
                                             spawnReference.GetComponent<Transform>().position + new Vector3(-1.25f + 2.5f * width / 2, 1.5f, -1.7f - length * 2.5f),
                                             spawnReference.GetComponent<Transform>().rotation);
        Vector3 scale = trigger.GetComponent<Transform>().localScale;
        trigger.transform.localScale = new Vector3(2.5f * width, scale.y, scale.z);
        trigger.GetComponent<WallTriggerController>().mazeStart = this.gameObject;
    }
    // Called when a projectile is shot outside of maze bounds away from the maze. 
    // Completes the maze, adds terminal horizontal walls and removes vertical walls separating disjoint sets
    public void finishMaze()
    {
        for (int i = 0; i < width - 1; i++)
        {
            if (cells[i].set != cells[i + 1].set && cells[i].eastWall)
            {
                // Destroy walls separating disjoint sets
                cells[i].eastWall = false;
                Destroy(eastWalls[i]);  
                unionSets(cells, i, i + 1);
            }
        }

        // Add bottom walls to all cells
        for (int i = 0; i < width; i++)
        {
                var southWall = (GameObject) Instantiate(south,
                            spawnReference.GetComponent<Transform>().position + new Vector3(i * 2.5f, 1.5f, -(length-1) * 2.5f - 1.15f),
                            spawnReference.GetComponent<Transform>().rotation);
                southWall.GetComponent<Wall>().hp = -1;
        }
    }

    // Merges disjoint sets of the given i,j values
    public static void unionSets(Cell[] arr, int i, int j)
    {
        int replaceNum = -1;
        int replaceWith = -1;

        if(arr[i].set < arr[j].set)
        {
            replaceWith = arr[i].set;
            replaceNum = arr[j].set;
        }
        else if(arr[j].set < arr[i].set)
        {
            replaceWith = arr[j].set;
            replaceNum = arr[i].set;
        }

        for (int k = 0; k < arr.Length; k++)
        {
            if (arr[k].set == replaceNum)
            {
                arr[k].set = replaceWith;
            }
        }
    }
}

public class Cell
{
    public bool southWall = true;
    public bool eastWall = false;
    public int set = -1;
}
