using System.Collections.Generic;
using UnityEngine;

public class CreateBoard082 : MonoBehaviour
{
    public Material mazeMaterial;
    public float[] wallBorders;
    private List<GameObject> maze;
    public int wallNumber = 0;
    public int getWallNumber => wallNumber;
    private bool mazeCreated = false;

    public void CreateMaze()
    {
        int wallNumberStart = 0;
        wallNumber = 0;
        int LayerIgnoreRaycast = LayerMask.NameToLayer("Walls");
        for (float x = wallBorders[1]-10.1f; x <= wallBorders[0]+0.1f; x += 10f)
        {
            for (float z = wallBorders[3]-10.1f; z <= wallBorders[2]+0.1f; z += 10f)
            {
                wallNumberStart++;
            }
        }
        maze = new List<GameObject>(wallNumberStart);
        for (float x = wallBorders[1]-10.1f; x <= wallBorders[0]+0.1f; x += 10f)
        {
            for (float z = wallBorders[3]-10.1f; z <= wallBorders[2]+0.1f; z += 10f)
            {
                GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                wall.layer = LayerIgnoreRaycast;
                wall.tag = "Wall";
                wall.transform.parent = transform;
                wall.transform.localPosition = new Vector3(0, -11f, 0);
                wall.transform.localScale = new Vector3(0.5f,15f,5f);
                BoxCollider collider = wall.GetComponent<BoxCollider>();
                collider.isTrigger = true;
                if (mazeMaterial != null)
                {
                    Renderer renderer = wall.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        renderer.material = mazeMaterial;
                    }
                }
                maze.Add(wall);
            }
        }
    }

    public void ResetWalls(){
        if(mazeCreated){CreateMaze();}
        wallNumber = 0;
        foreach(GameObject wall in maze)
        {
            wall.transform.localPosition = new Vector3(0f, -11f, 0f);
        }        
    }

    public void SetMaze(float dif){
        if(mazeCreated){CreateMaze();}
        ResetWalls();
        wallNumber = 0;
        int wall_index = -1;
        float slide = Random.Range(7.5f, 12.5f);
        for (float x = wallBorders[1]-10.1f; x <= wallBorders[0]+0.1f; x += 10f)
        {
            for (float z = wallBorders[3]-10.1f; z <= wallBorders[2]+0.1f; z += 10f)
            {
                if (Random.Range(0f, 1f) < dif)
                {
                    if(wall_index++ < maze.Count && maze[wall_index] != null){
                        wallNumber++;
                        maze[wall_index].transform.localRotation = Quaternion.Euler(0f,Random.Range(0f, 360f),0f);
                        maze[wall_index].transform.localPosition = new Vector3(x+slide, 8f, z+slide);
                    }
                }
            }
        } 
    }
}
