using System.Collections.Generic;
using UnityEngine;

public class CreateBoard075 : MonoBehaviour
{
    [Header("Table Settings")]
    [SerializeField] private bool randomTableSize = false;
    public bool getRandomTableSize => randomTableSize;
    [SerializeField] private bool Maze = false;
    public bool gMaze => Maze;
    [SerializeField] [Range(1,1000)]private float freq = 100;
    public float gfreq => freq;    
    [SerializeField] [Range(15,50)] public int rows = 30;
    [SerializeField] [Range(15,50)] public int columns = 30;
    [SerializeField] [Range(0.01f,0.99f)] private float difficulty = 0.03f;
    public bool randomScale = false;
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
    private GameObject master;
    private int wallDistCheck = 0;
    public float getD => difficulty;
    public int wallNumber = 0;
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
        master = new GameObject("Maze");
        if(Maze){
            CreateMaze(difficulty); 
        }
        ObjectPos(difficulty);
    }

    public void ResetEnv(float difficulty){
        ClearEnvironment();
        int new_size = Random.Range(20,40);
        rows = new_size;
        columns = new_size;
        Vector3 boardSize = CreateBoxes();
        CreateWalls(boardSize);
        ObjectPos(difficulty);
        RecreateMaze(difficulty);
    }

    public void ResetEnvSimp(){
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

    public float ObjectPos(float difficulty){
        Vector3 target_start;
        Vector3 product_start;
        do {
            target_start = randomPos();
            product_start = randomPos();
            wallDistCheck++;
            if (wallDistCheck > 50){
                wallDistCheck = 0;
                difficulty = (float)System.Math.Pow(difficulty,0.0001f) * difficulty * 0.77f;
                RecreateMaze(difficulty);
            }
        } while (Vector3.Distance(target_start, product_start) < 8f || GetWallDist(target) < 3f || GetWallDist(product) < 2f);
        wallDistCheck = 0;
        target.transform.localPosition = target_start;
        product.transform.localPosition = product_start;
        startDistance = Vector3.Distance(target_start, product_start);
        productCollision075 productClass = product.GetComponent<productCollision075>();
        productClass.InitializeProduct(target,gameObject);
        if(randomScale){
            var new_scale = Random.Range(3f,5f);
            scale = new_scale;
            product.transform.localScale = new Vector3(new_scale,new_scale,new_scale);            
        }
        else{scale = 4f;product.transform.localScale = new Vector3(scale,scale,scale);}
        target.SendMessage("GetBorders");
        return difficulty;
    }

    private Vector3 randomPos(){
        return new Vector3(Random.Range(wallBorders[0]-(scale/2)-0.5f, wallBorders[1]+(scale/2)+0.5f), Random.Range(6.5f,9.8f), Random.Range(wallBorders[2]-(scale/2)-0.5f, wallBorders[3]+(scale/2)+0.5f));
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

    public void CreateMaze(float difficulty)
    {
        wallNumber = 0;
        master.transform.parent = transform;
        int LayerIgnoreRaycast = LayerMask.NameToLayer("Walls");
        maze = new List<GameObject>(System.Convert.ToInt32(((wallBorders[0]-wallBorders[1])/7f)*((wallBorders[2]-wallBorders[3])/7f)));
        for (float x = wallBorders[1]+0.1f; x < wallBorders[0]-0.1f; x += 7f)
        {
            for (float z = wallBorders[3]; z < wallBorders[2]; z += 7f)
            {
                if (Random.Range(0f, 1f) < difficulty)
                {
                    wallNumber++;
                    Quaternion rot = Quaternion.Euler(0f,Random.Range(0f, 360f),0f);
                    GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    wall.layer = LayerIgnoreRaycast;
                    wall.tag = "Wall";
                    wall.transform.parent = transform;
                    Vector3 position = new Vector3(x, 0f, z);

                    wall.transform.localScale = new Vector3(0.2f,15f,5f);
                    wall.transform.localPosition = position + new Vector3(0.5f, 8f, 0.5f);
                    wall.transform.localRotation = rot;
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
                    wall.transform.parent = master.transform;
                    maze.Add(wall);
                }
            }
        }
    }

    public void RecreateMaze(float difficulty){
        foreach (var wall in maze){
            if (maze!=null){
                Destroy(wall);
            }
        }
        maze.Clear();
        CreateMaze(difficulty);
    }
}
