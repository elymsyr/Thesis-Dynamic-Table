using UnityEngine;
using Unity.MLAgents.Actuators;
using Unity.MLAgents;
using TMPro;
using Unity.MLAgents.Sensors;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using System.Collections.ObjectModel;

public class dynamicTable090 : Agent
{
    [Header("UI")]
    [SerializeField] private bool showUI = false;
    [SerializeField] private bool TEST = true;
    [SerializeField] private GameObject text;

    [Header("Maze")]
    [SerializeField] private bool maze = false;
    [SerializeField] [Range(0,9)]private int mazeNumber = 9; // Default : 0 | Can be set for initialized trains.
    public int lim = 98; // Also set this for initialized trains.
    public bool changeMaze = false;
    [SerializeField] private GameObject mazesParent;
    private Vector3 mazeLocationSet = new Vector3(158f,6f,1.25f);
    private List<Transform> mazes;
    private GameObject randomMaze => mazes[UnityEngine.Random.Range(0, mazes.Count)].gameObject;
    private GameObject selectedMaze;
    [Header("Set")]
    [SerializeField] private GameObject pathfinder;
    [SerializeField] private GameObject product;
    [SerializeField] private GameObject target;
    public LayerMask layerMask;
    public Material test;
    [Range(0f,15f)] public float MoveSpeed = 12f;

    // public Vector3 point(float scale, int pointNumber) => pointNumber != 0 ? new Vector3(0, scale/2, 0) : new Vector3(scale/2, 0, 0);

    private int winState = 0;
    private Queue<int> gameStates = new Queue<int>();
    private AStar090 AStar;
    private int RowsNColumns = 22;
    private float closeness = 0;
    private float scale = 4f; 
    private TextMeshPro ui;
    private float directionPoint = 0;
    private int rows = 22;
    private int columns = 22;
    private int number_wins = 0; 
    private Transform[,] boxesArray;
    private Vector3[,] boxesLoc;
    private float[] wallBorders = {14.2f,-15.2f,14.2f,-15.2f};
    public float[] getBorders => wallBorders;
    private int size = 8;
    private Transform[] activeArray;
    private Rigidbody productRigidbody;
    private List<Tuple<int, int>> specifiedPoints = new List<Tuple<int, int>>(){new Tuple<int, int>(0, 0),new Tuple<int, int>(0, 1),new Tuple<int, int>(1, 0),new Tuple<int, int>(2, 0),new Tuple<int, int>(0, 2),new Tuple<int, int>(0, 5),new Tuple<int, int>(0, 6),new Tuple<int, int>(0, 7),new Tuple<int, int>(1, 7),new Tuple<int, int>(2, 7),new Tuple<int, int>(5, 0),new Tuple<int, int>(6, 0),new Tuple<int, int>(7, 0),new Tuple<int, int>(7, 1),new Tuple<int, int>(7, 2),new Tuple<int, int>(7, 5),new Tuple<int, int>(7, 6),new Tuple<int, int>(7, 7),new Tuple<int, int>(6, 7),new Tuple<int, int>(5, 7),};
    private productCollision090 productClass;
    private StatsRecorder recorder;
    private int lastStep = 0;
    private float heighpoint;
    private List<float> observation; // RAY
    private RayPerceptionOutput.RayOutput[] rayOutputs; // RAY
    private RayPerceptionSensorComponent3D rayPerceptionSensor; // RAY
    private Transform pathFirstNode = null;
    private int checkpointCount = 100;
    private int lastCheckpoint = 0;


    void Awake()
    {
        mazes = new List<Transform>();
        if (pathfinder != null){AStar = pathfinder.GetComponent<AStar090>();}
        if(AStar.pathFinder){AStar.initPathFinder();}

        if(maze){
            for (int i = 0; i < mazesParent.transform.childCount; i++)
            {
                Transform childMaze = mazesParent.transform.GetChild(i);
                mazes.Add(childMaze);
            }
        }

        rows = RowsNColumns;
        columns = RowsNColumns;
        productClass = product.GetComponent<productCollision090>();
        observation = new List<float>(10); // RAY
        Transform ray = product.transform.GetChild(0); // RAY
        rayPerceptionSensor = ray.GetComponent<RayPerceptionSensorComponent3D>(); // RAY    
        recorder = Academy.Instance.StatsRecorder;

        if (text != null)
        {
            ui = text.GetComponent<TextMeshPro>();
        }

        productRigidbody = product.GetComponent<Rigidbody>();

        int childIndex = 0;
        int rowIndex = 0;
        int columnIndex = 0;
        boxesArray = new Transform[rows, columns];
        boxesLoc = new Vector3[rows, columns];
        Transform pieces = transform.Find("Pieces");
        for (int i = 0; i < rows*columns; i++)
        {
            Transform child = pieces.GetChild(childIndex);
            boxesArray[rowIndex, columnIndex] = child;
            boxesLoc[rowIndex, columnIndex] = child.localPosition;
            columnIndex++;
            if (columnIndex >= columns)
            {
                columnIndex = 0;
                rowIndex++;
            }
            childIndex++;
        }
    }

    void Update(){
        if(AStar.pathFinder && Vector3.Distance(AStar.lastPos,product.transform.localPosition)>1f){ 
            AStar.lastPos = product.transform.localPosition;
            pathFirstNode = AStar.PathFinder(AStar.getPoint(product.transform), AStar.getPoint(target.transform));
            if(pathFirstNode != null){AStar.changeMaterial(pathFirstNode, test);}
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if(product.transform.localPosition.y < 0){EndEpisode();}
        int index = 0;
        int movingPartsIndex = 0;
        GetActiveArray();

        foreach (Transform child in boxesArray)
        {
            if (child != null)
            {
                if (Array.IndexOf(activeArray, child) != -1){
                    float newYPosition = child.localPosition.y + actions.ContinuousActions[movingPartsIndex] * MoveSpeed * Time.deltaTime;
                    newYPosition = Mathf.Clamp(newYPosition, 0f, 4.7f);
                    child.localPosition = new Vector3(child.localPosition.x, newYPosition, child.localPosition.z);
                    movingPartsIndex++;
                }
                else
                {
                    int i = index / rows;
                    int j = index % columns;
                    child.transform.localPosition = boxesLoc[i, j];
                }
                index++;
            }
            else{
                Debug.Log("Null child founded!");
            }
        }
        if(!productClass.triggered){
            if(pathFirstNode != null){
                directionPoint = Vector3.Dot(productRigidbody.velocity.normalized, (gameObject.transform.InverseTransformPoint(pathFirstNode.position) - product.transform.localPosition).normalized);
            }
            else{
                directionPoint = Vector3.Dot(productRigidbody.velocity.normalized, (target.transform.localPosition - product.transform.localPosition).normalized);
            }
            float heightPoint = product.transform.localPosition.y;
            float closeness = targetCloseness();
            float speed = productRigidbody.velocity.magnitude;
            if(directionPoint<0.65f && directionPoint>0){directionPoint*=-1;}
            if(speed < 0.1f){speed = 0.1f;}
            if(closeness<0.1){closeness = 0.1f;}
            float reward_increase = directionPoint * speed * speed * 0.0001f; // Change for rereremaze directionPoint  * speed * 0.0002f;
            float reward_decrease = (float)Math.Pow(closeness, 0.1f);
            float reward = reward_increase / reward_decrease;
            if(directionPoint>=0){
                reward += heightPoint*0.0001f*0.2f;
            }
            AddReward(reward);
        }
        if (showUI)
        {
            updateUI();
        }
        lastStep = StepCount;
    }
    
    public override void OnEpisodeBegin()
    {
        AStar.TestProperty = TEST;
        
        if(maze){
            bool mazeChange = false;
            if(!changeMaze){
                if((CompletedEpisodes-lastCheckpoint)%checkpointCount == 0){
                    float winPercentage = CalculatePercentageOfOnes();
                    lastCheckpoint = CompletedEpisodes;
                    if(winPercentage >= lim){
                        if(mazeNumber == 0 && lim == 99){checkpointCount=50;lim=95;mazeNumber++;mazeChange = true;gameStates.Clear();}
                        else if(mazeNumber == 0){checkpointCount=50;lim=99;}
                        else if(mazeNumber<9){mazeNumber++;checkpointCount=100;mazeChange = true;gameStates.Clear();}
                        else if(mazeNumber == 9){checkpointCount=50;lim=99;mazeChange = true;gameStates.Clear();}
                        else{checkpointCount = 25;lim=99;mazeNumber = 9;mazeChange = true;gameStates.Clear();}
                    }
                    else{
                        if(checkpointCount>=100){
                            checkpointCount = 25;
                        }
                        else{checkpointCount+=25;}
                    }
                }
            }
            if(mazeChange || changeMaze){ // changeMaze : Manual Change
                if(selectedMaze != null){Destroy(selectedMaze);}
                if(mazeNumber >= 1 && mazeNumber <= 8){selectedMaze = Instantiate(mazes[mazeNumber-1].gameObject, mazeLocationSet, Quaternion.identity);}
                else if(mazeNumber == 9){selectedMaze = Instantiate(randomMaze, mazeLocationSet, Quaternion.identity);}
            }
        }

        gameStates.Enqueue(winState);
        if(gameStates.Count > 100){
            gameStates.Dequeue();
        }
        winState = 0;
        recorder.Add("Custom/Win Percentage of Last 100 Episodes",CalculatePercentageOfOnes(),StatAggregationMethod.Average);
        recorder.Add("Custom/Completed Episodes",CompletedEpisodes,StatAggregationMethod.Average);
        recorder.Add("Custom/Avg Step",lastStep,StatAggregationMethod.Average);
        if(maze){
            recorder.Add("Custom/Maze Number",mazeNumber,StatAggregationMethod.Average);
            recorder.Add("Custom/Checkpoints",lastCheckpoint,StatAggregationMethod.Average);
        }
        product.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        productRigidbody.velocity = Vector3.zero;
        activeArray = new Transform[size*size-specifiedPoints.Count];
        
        ObjectPos();

        AStar.resetNodeAll(AStar.getPoint(target.transform));
    }

    private float CalculatePercentageOfOnes()
    {   if(gameStates.Count > 0){
            int totalOnes = 0;
            foreach (int item in gameStates)
            {
                if (item == 1)
                {
                    totalOnes++;
                }
            }
            return ((float)totalOnes / gameStates.Count) * 100f;
        }
        else{return 0;}
    }     

    public void triggerReset(){
        AddReward(-3f);
        EndEpisode();
    }
    
    public void winReset(){
        winState = 1;
        number_wins++;
        AddReward(2f);
        EndEpisode();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        foreach (Transform child in activeArray)
        {
            if (child != null)
            {
                sensor.AddObservation(child.localPosition.y);
            }
            else
            {
                sensor.AddObservation(0);
            }
        }
        sensor.AddObservation(product.transform.localPosition);

        if(pathFirstNode != null){sensor.AddObservation(gameObject.transform.InverseTransformPoint(pathFirstNode.position));}
        else{sensor.AddObservation(target.transform.localPosition);}
        

        sensor.AddObservation(productRigidbody.velocity.magnitude);
        RayCollect(); // RAY
        sensor.AddObservation(observation); // RAY
    }

    private void RayCollect(){
        if (observation != null){
            observation.Clear();
        }
        rayOutputs = RayPerceptionSensor.Perceive(rayPerceptionSensor.GetRayPerceptionInput()).RayOutputs;
        for (int i = 0; i < rayOutputs.Length-1; i++)
        {
            var rayDirection = rayOutputs[i].EndPositionWorld - rayOutputs[i].StartPositionWorld;
            float rayHitDistance = rayOutputs[i].HitFraction * rayDirection.magnitude;
            observation.Add(rayHitDistance);
        }
    }

    private Vector3 randomPos(){
        return new Vector3(UnityEngine.Random.Range(wallBorders[0]-scale, wallBorders[1]+scale), UnityEngine.Random.Range(6f,7f), UnityEngine.Random.Range(wallBorders[2]-scale, wallBorders[3]+scale));
    }

    public void ObjectPos(){
        bool control = false;
        int nTime = 0;
        while(!control){

            if(nTime >= 50){
                if(selectedMaze!=null){Destroy(selectedMaze);}
            }

            Vector3 target_start;
            Vector3 product_start;
            do{
                target_start = randomPos();
                product_start = randomPos();
            }while(Vector3.Distance(target_start, product_start) < 8f);
            target.transform.localPosition = target_start;
            product.transform.localPosition = product_start;

            Vector3[] directions = {
                // Cardinal directions along XZ plane
                Vector3.forward,
                Vector3.back,
                Vector3.left,
                Vector3.right,
                // Diagonal directions along XZ plane
                new Vector3(1, 0, 1).normalized,
                new Vector3(-1, 0, 1).normalized,
                new Vector3(1, 0, -1).normalized,
                new Vector3(-1, 0, -1).normalized,
                // Secondary directions along XZ plane
                Quaternion.Euler(0, 45, 0) * Vector3.forward,
                Quaternion.Euler(0, 45, 0) * Vector3.back,
                Quaternion.Euler(0, 45, 0) * Vector3.left,
                Quaternion.Euler(0, 45, 0) * Vector3.right,
                // Secondary diagonal directions along XZ plane
                Quaternion.Euler(0, 45, 0) * new Vector3(1, 0, 1).normalized,
                Quaternion.Euler(0, 45, 0) * new Vector3(-1, 0, 1).normalized,
                Quaternion.Euler(0, 45, 0) * new Vector3(1, 0, -1).normalized,
                Quaternion.Euler(0, 45, 0) * new Vector3(-1, 0, -1).normalized,
                // Additional directions
                Quaternion.Euler(0, 22.5f, 0) * Vector3.forward,
                Quaternion.Euler(0, 22.5f, 0) * Vector3.back,
                Quaternion.Euler(0, 22.5f, 0) * Vector3.left,
                Quaternion.Euler(0, 22.5f, 0) * Vector3.right,
                Quaternion.Euler(0, 22.5f, 0) * new Vector3(1, 0, 1).normalized,
                Quaternion.Euler(0, 22.5f, 0) * new Vector3(-1, 0, 1).normalized,
                Quaternion.Euler(0, 22.5f, 0) * new Vector3(1, 0, -1).normalized,
                Quaternion.Euler(0, 22.5f, 0) * new Vector3(-1, 0, -1).normalized,
            };
            float offset = 0.2f;
            Vector3[] positions = {
                new Vector3(target.transform.position.x+offset, target.transform.position.y, target.transform.position.z),
                new Vector3(target.transform.position.x-offset, target.transform.position.y, target.transform.position.z),
                new Vector3(target.transform.position.x, target.transform.position.y, target.transform.position.z+offset),
                new Vector3(target.transform.position.x, target.transform.position.y, target.transform.position.z-offset),
                new Vector3(product.transform.position.x+offset, product.transform.position.y, product.transform.position.z),
                new Vector3(product.transform.position.x-offset, product.transform.position.y, product.transform.position.z),
                new Vector3(product.transform.position.x, product.transform.position.y, product.transform.position.z+offset),
                new Vector3(product.transform.position.x, product.transform.position.y, product.transform.position.z-offset)              
            };

            bool check = true;
            foreach (Vector3 direction in directions)
            {
                RaycastHit hit;
                if (Physics.Raycast(target.transform.position, direction, out hit, 2.5f, layerMask))
                {
                    check = false;
                    break;
                }
                if (check){
                    if (Physics.Raycast(product.transform.position, direction, out hit, 3.2f, layerMask))
                    {
                        check = false;
                        break;
                    }
                }
            }
            if(check){
                foreach (Vector3 position in positions.Take(8).ToArray()){
                    foreach (Vector3 direction in directions.Take(8).ToArray()){
                        RaycastHit hit;
                        
                        float rayL = 0.4f;
                        if (Physics.Raycast(position, direction, out hit, rayL, layerMask))
                        {
                            check = false;
                            break;
                        }
                    }
                }
            }
            if(check){control = true;}
            if(!control){
                nTime++;
            }
        }
    }


    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        for (int i = 0; i < size*size-specifiedPoints.Count; i++)
        {
            continuousActions[i] = UnityEngine.Random.Range(-1f, 1f);
        }       
    }

    private void updateUI()
    {
        ui.text = "Product States\nTarget Location: "+TargetLocation()+"\nAre we winning? "+CalculatePercentageOfOnes()+"\nBoard Size: "+rows+"x"+columns+"\nDirection: "+directionPoint+"\nSpeed: "+productRigidbody.velocity.magnitude+"\nPosition: "+product.transform.localPosition+"\nHeighpoint: "+heighpoint+"\nDistance to Target: "+closeness+"\nReward: "+GetCumulativeReward()+"\nAction Count: "+StepCount+"\nGame Count: "+CompletedEpisodes+"\nWin Count: "+number_wins+"\nActive Parts Map: \n"+ActiveMap();
    }

    private string TargetLocation(){
        ReadOnlyCollection<float> observation = GetObservations();
        if(observation.Count == 0){
            return "         ";
        }
        else{
            string str = observation.Count+": ";
            str +=observation[47] + " | ";
            str +=observation[48] + " | ";
            str +=observation[49];    
            return str;
        }
    }

    private string ActiveMap(){
        string arrayString = "";
        int index = 0;
        int[] alpha = new int[specifiedPoints.Count];
        for(int i=0;i<specifiedPoints.Count;i++){
            alpha[i] = specifiedPoints[i].Item1*size+specifiedPoints[i].Item2;
        }
        for (int i = 0; i < size*size; i++)
        {
            if (alpha.Contains(i)){arrayString += " _ ";}
            else{
                if (activeArray[index] != null){arrayString += (int)activeArray[index].transform.localPosition.y+" ";}
                else{arrayString += "X ";}
                index++;
            }
            if ((i+1)%size == 0){arrayString += "\n";}
        }
        return arrayString;       
    }

    private void GetActiveArray()
    {
        int[] centerPoint = FindClosestTransform();
        bool isInList;
        int startX = centerPoint[0] - size / 2;
        int startY = centerPoint[1] - size / 2;
        int index = 0;
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                isInList = specifiedPoints.Any(tuple => tuple.Item1 == (i,j).Item1 && tuple.Item2 == (i,j).Item2);
                if (!isInList){
                    if(IsIndexValid(startX + i, startY + j) && boxesArray[startX + i, startY + j] != null){
                        activeArray[index] = boxesArray[startX + i, startY + j];
                    }
                    else{
                        activeArray[index] = null;
                    }                    
                index++;
                }
            }
        }
    }
    
    private bool IsIndexValid(int rowIndex, int colIndex)
    {
        return rowIndex >= 0 && rowIndex < rows && colIndex >= 0 && colIndex < columns;
    }    
    
    private int[] FindClosestTransform()
    {
        int[] closestPosition = new int[2];
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                Transform transform = boxesArray[i, j];
                if (transform != null)
                {
                    float distance = GetDistanceToChild(transform);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestPosition[0] = i;
                        closestPosition[1] = j;
                    }
                }
            }
        }

        return closestPosition;
    }
    
    private float targetCloseness()
    {
        return Vector3.Distance(product.transform.localPosition, target.transform.localPosition);
    }
    
    private float GetDistanceToChild(Transform child)
    {
        float distance = Vector3.Distance(gameObject.transform.InverseTransformPoint(new Vector3(child.position.x, 0f, child.position.z)), gameObject.transform.InverseTransformPoint(new Vector3(product.transform.position.x, 0f, product.transform.position.z)));
        return distance;
    }

}