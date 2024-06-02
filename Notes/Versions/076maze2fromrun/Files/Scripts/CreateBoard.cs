using System.Collections.Generic;
using UnityEngine;

public class CreateBoard076 : MonoBehaviour
{
    [Header("Table Settings")]
    [SerializeField] private bool Maze = false;
    public bool gMaze => Maze;
    [SerializeField] [Range(1,1000)]private float freq = 100;
    public float gfreq => freq;    
    [SerializeField] [Range(15,50)] public int rows = 30;
    [SerializeField] [Range(15,50)] public int columns = 30;
    [SerializeField] [Range(0.01f,0.4f)] private float difficulty = 0.03f;
    [Header("Target Settings")]
    public bool TargetRun = false;
    public bool RandomTargetSpeed = true;
    [SerializeField] [Range(2f,6f)] public float TargetMoveSpeed = 4f;
    private float gap = 0.2f;
    [Header("Prefabs & Others")]
    public Material boxMaterial;
    public Material coverMaterial;
    public Material wallMaterial;
    public Material mazeMaterial;
    private GameObject pieces;
    private GameObject cover;
    private Transform[,] boxesArray;
    private float[] wallBorders;
    public float[] getBorders => wallBorders;
    public Transform[,] getPieces => boxesArray;
    [SerializeField] private GameObject productPrefab;
    private GameObject product;
    public GameObject getProduct => product;
    [SerializeField] private GameObject targetPrefab;
    private GameObject target;
    public GameObject getTarget => target;
    private float scale = 4f;
    public GameObject[] getWalls => wallsArray;

    public float productScale => scale;
    public GameObject[] wallsArray; 
    public List<GameObject> maze;
    private int wallDistCheck = 0;
    public float getD => difficulty;
    private int wallNumber = 0;
    public int getWallNumber => wallNumber;
    private float startDistance = 0;
    public float getSDistance => startDistance;

    public float GetWallDist(GameObject ball){
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
        if (walls.Length == 0)
        {
            Debug.LogWarning("No walls found.");
        }
        float closestDistance = Mathf.Infinity;

        foreach (GameObject wall in walls)
        {
            float distance = Vector3.Distance(ball.transform.position, wall.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
            }
        }
        return closestDistance;
    }    

    public void CreateEnv(float difficulty){
        Vector3 boardSize = CreateBoxes();
        CreateWalls(boardSize);
        LoadPrefabs();
        CreateMaze();
        ObjectPos(difficulty);
    }
    
    public Vector3 CreateBoxes()
    {

        // Pieces
        pieces = new GameObject("Pieces");
        pieces.transform.parent = transform;    

        Vector3 boxSize = new Vector3(1, 6, 1);
        float totalWidth = rows * (boxSize.x + gap);
        float totalHeight = columns * (boxSize.z + gap);

        Vector3 startPos = transform.position - new Vector3(totalWidth / 2f, 0f, totalHeight / 2f);
        Vector3 boardSize = new Vector3(totalWidth, boxSize.y, totalHeight);

        boxesArray = new Transform[rows, columns];

        for (int x = 0; x < rows; x++)
        {
            for (int z = 0; z < columns; z++)
            {
                Vector3 position = startPos + new Vector3(x * (boxSize.x + gap), 0f, z * (boxSize.z + gap));
                GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                box.name = "Box";
                box.transform.position = position;
                box.transform.localScale = boxSize;
                box.transform.parent = pieces.transform;

                if (boxMaterial != null)
                {
                    Renderer renderer = box.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        renderer.material = boxMaterial;
                    }
                }

                boxesArray[x, z] = box.transform;
            }
        }

        Vector3 coverSize = new Vector3(totalWidth+1f, 4, totalHeight+1f);
        cover = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cover.name = "Board";
        cover.transform.parent = transform;
        cover.transform.position = startPos + new Vector3(totalWidth / 2f, -0.3f, totalHeight / 2f);
        cover.transform.localScale = coverSize;
        Vector3 boardPosition = cover.transform.localPosition;
        boardPosition.x -= gap*3;
        boardPosition.z -= gap*3;
        cover.transform.localPosition = boardPosition;

        if (coverMaterial != null)
        {
            Renderer coverRenderer = cover.GetComponent<Renderer>();
            if (coverRenderer != null)
            {
                coverRenderer.material = coverMaterial;
            }
        }

        return boardSize;
    }

    public void CreateWalls(Vector3 boardSize)
    {
        wallsArray = new GameObject[4];
        wallBorders = new float[4];
        float wallThickness = 0.2f;
        float wallHeight = 15f;

        Vector3[] wallPositions = {
            new Vector3(boardSize.x / 2f, wallHeight / 2f, 0f),
            new Vector3(-boardSize.x / 2f, wallHeight / 2f, 0f),
            new Vector3(0f, wallHeight / 2f, boardSize.z / 2f),
            new Vector3(0f, wallHeight / 2f, -boardSize.z / 2f)
        };

        Quaternion[] wallRotations = {
            Quaternion.Euler(0f, 0f, 0f),
            Quaternion.Euler(0f, 0f, 0f),
            Quaternion.Euler(0f, 90f, 0f),
            Quaternion.Euler(0f, 90f, 0f)
        };

        int LayerIgnoreRaycast = LayerMask.NameToLayer("Walls");

        for (int i = 0; i < wallPositions.Length; i++)
        {
            Vector3 position = wallPositions[i];
            Quaternion rotation = wallRotations[i];

            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.layer = LayerIgnoreRaycast;
            wall.tag = "Wall";
            wall.transform.parent = transform;
            Collider wallCollider = wall.GetComponent<BoxCollider>();
            wallCollider.isTrigger = true;
            wall.transform.localPosition = position;
            wall.transform.localScale = new Vector3(wallThickness, wallHeight, boardSize.z+2.5f);
            wall.transform.rotation = rotation;
            wall.name = "Wall"+(i+1);
            Vector3 wallPosition = wall.transform.localPosition;
            if(i==0){
                wallPosition.x += 1f;
                wallPosition.z -= 0.5f;
            }
            else if(i==1){
                wallPosition.x -= 2f;
                wallPosition.z -= 0.5f;
            }
            else if(i==2){
                wallPosition.z += 1f;
                wallPosition.x -= 0.5f;
                
            }
            else{
                wallPosition.z -= 2f;
                wallPosition.x -= 0.5f;
            }
            wall.transform.localPosition = wallPosition;
            if (i > 1){wallBorders[i] = wall.transform.localPosition.z;}
            else{wallBorders[i] = wall.transform.localPosition.x;}
            if (wallMaterial != null)
            {
                Renderer renderer = wall.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material = wallMaterial;
                }
            }
            wallsArray[i] = wall;
        }
    }

    public void LoadPrefabs(){
        product = Instantiate(productPrefab);
        product.transform.parent = transform;
        product.name = "Product";
        product.transform.localRotation = Quaternion.identity;

        target = Instantiate(targetPrefab);
        target.transform.parent = transform;
        target.SendMessage("AwakeMe");     
        target.name = "Target";
        target.transform.localRotation = Quaternion.identity;
    }

    public float ObjectPos(float diff){
        Vector3 target_start = randomPos();
        Vector3 product_start = randomPos();      
        bool positining = true;
        wallDistCheck = 0;
        int positiningCount = 0;
        int mazePositiningCount = 0;
        
        while(positining){
            do {
                if (wallDistCheck > 10){
                    wallDistCheck = 0;
                    diff = (float)System.Math.Pow(diff,0.0001f) * diff * 0.84f;
                    SetMaze(diff);
                }
                target_start = randomPos();
                product_start = randomPos();
                target.transform.localPosition = target_start;
                product.transform.localPosition = product_start;
                wallDistCheck++;
                mazePositiningCount++;
                if(mazePositiningCount > 100){
                    ResetWalls();
                    target_start = randomPos();
                    product_start = randomPos();
                    target.transform.localPosition = target_start;
                    product.transform.localPosition = product_start;
                    break;                    
                }
            } while (GetWallDist(target) < 2f || GetWallDist(product) < 2.6f);

            if(Vector3.Distance(target_start, product_start) > 5f){
                positining = false;
            }

            if(positiningCount > 10){
                ResetWalls();
                target.transform.localPosition = randomPos();
                product.transform.localPosition = randomPos();
                break;
            }
            positiningCount++;
        }

        startDistance = Vector3.Distance(target_start, product_start);
        productCollision076 productClass = product.GetComponent<productCollision076>();
        productClass.InitializeProduct(target,gameObject);
        target.SendMessage("GetBorders");
        return diff;
    }

    private Vector3 randomPos(){
        return new Vector3(Random.Range(wallBorders[0]-(scale/2)-2.1f, wallBorders[1]+(scale/2)+2.1f), Random.Range(6.5f,9.8f), Random.Range(wallBorders[2]-(scale/2)-2.1f, wallBorders[3]+(scale/2)+2.1f));
    }    

    public void ClearEnvironment()
    {
        foreach (Transform child in boxesArray)
        {
            Destroy(child.gameObject);
        }
        foreach (GameObject child in wallsArray)
        {
            Destroy(child.gameObject);
        }
        Destroy(pieces.gameObject);
        Destroy(cover.gameObject);
        wallsArray = null;
        wallBorders = null;
        boxesArray = null;
        pieces = null;
        cover = null;
    }

    public void CreateMaze()
    {
        int wallNumberStart = 0;
        wallNumber = 0;
        int LayerIgnoreRaycast = LayerMask.NameToLayer("Walls");
        for (float x = wallBorders[1]-0.1f; x <= wallBorders[0]+0.1f; x += 10f)
        {
            for (float z = wallBorders[3]-0.1f; z <= wallBorders[2]+0.1f; z += 10f)
            {
                wallNumberStart++;
            }
        }
        maze = new List<GameObject>(wallNumberStart);
        for (float x = wallBorders[1]-0.1f; x <= wallBorders[0]+0.1f; x += 10f)
        {
            for (float z = wallBorders[3]-0.1f; z <= wallBorders[2]+0.1f; z += 10f)
            {
                GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                wall.layer = LayerIgnoreRaycast;
                wall.tag = "Wall";
                wall.transform.parent = transform;
                wall.transform.localPosition = new Vector3(0, -11f, 0);
                wall.transform.localScale = new Vector3(0.2f,15f,5f);
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
        wallNumber = 0;
        foreach(GameObject wall in maze)
        {
            wall.transform.localPosition = new Vector3(0f, -11f, 0f);
        }        
    }

    public void SetMaze(float diff){
        ResetWalls();
        wallNumber = 0;
        int wall_index = -1;
        for (float x = wallBorders[1]-0.1f; x < wallBorders[0]+0.1f; x += 10f)
        {
            for (float z = wallBorders[3]-0.1f; z < wallBorders[2]+0.1f; z += 10f)
            {
                if (Random.Range(0f, 1f) < diff)
                {
                    if(wall_index++ < maze.Count && maze[wall_index] != null){
                        wallNumber++;
                        maze[wall_index].transform.localRotation = Quaternion.Euler(0f,Random.Range(0f, 360f),0f);
                        maze[wall_index].transform.localPosition = new Vector3(x+1f, 8f, z+1f);
                    }
                }
            }
        } 
    }
}
