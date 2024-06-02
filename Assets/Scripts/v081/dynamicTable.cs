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

public class dynamicTable081 : Agent
{
    [Header("UI")]
    [SerializeField] private bool showUI = false;
    [SerializeField] private GameObject text;
    
    [Header("Target Run")]
    [SerializeField] public bool targetMovement = false;
    [SerializeField] public float getTargetSpeed => customTargetSpeed;
    [SerializeField] public float customTargetSpeed = 6f;
    [Header("Maze")]
    [SerializeField] private bool maze = false;
    public Material focusTarget;
    [SerializeField] private bool setDifficulty = false;
    [SerializeField] [Range(0f, 0.99f)]private float MazeDifficulty = 0.2f;
    [Header("Set")]
    [SerializeField] private GameObject pathfinder;
    [SerializeField] private GameObject product;
    [SerializeField] private GameObject target;
    [Range(0f,15f)] public float MoveSpeed = 12f;
    private int win = 0;
    private int winState = 0;
    private Queue<int> gameStates = new Queue<int>();
    private AStar081 AStar;
    private int RowsNColumns = 22;
    private float closeness = 0;
    private float scale = 4f; 
    private TextMeshPro ui;
    private float directionPoint = 0;
    private int rows = 22;
    private int columns = 22;
    private Transform[,] boxesArray;
    private Vector3[,] boxesLoc;
    private float[] wallBorders = {14.2f,-15.2f,14.2f,-15.2f};
    public float[] getBorders => wallBorders;
    private int size = 8;
    private Transform[] activeArray;
    private Rigidbody productRigidbody;
    private List<Tuple<int, int>> specifiedPoints = new List<Tuple<int, int>>(){new Tuple<int, int>(0, 0),new Tuple<int, int>(0, 1),new Tuple<int, int>(1, 0),new Tuple<int, int>(2, 0),new Tuple<int, int>(0, 2),new Tuple<int, int>(0, 5),new Tuple<int, int>(0, 6),new Tuple<int, int>(0, 7),new Tuple<int, int>(1, 7),new Tuple<int, int>(2, 7),new Tuple<int, int>(5, 0),new Tuple<int, int>(6, 0),new Tuple<int, int>(7, 0),new Tuple<int, int>(7, 1),new Tuple<int, int>(7, 2),new Tuple<int, int>(7, 5),new Tuple<int, int>(7, 6),new Tuple<int, int>(7, 7),new Tuple<int, int>(6, 7),new Tuple<int, int>(5, 7),};
    private productCollision081 productClass;
    private StatsRecorder recorder;
    private int lastStep = 0;
    private float heighpoint;
    private CreateBoard081 Labyrynt;
    private List<float> observation; // RAY
    private RayPerceptionOutput.RayOutput[] rayOutputs; // RAY
    private RayPerceptionSensorComponent3D rayPerceptionSensor; // RAY

    void Awake()
    {
        if (pathfinder != null){AStar = pathfinder.GetComponent<AStar081>();}
        Labyrynt = transform.GetComponent<CreateBoard081>();
        Labyrynt.wallBorders = wallBorders;
        if(maze){
            Labyrynt.CreateMaze();
        }
        rows = RowsNColumns;
        columns = RowsNColumns;
        productClass = product.GetComponent<productCollision081>();
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
        // if (!productClass.triggered){
        //     if(maze && AStar.getPath != null && AStar.getPath.Count > 0){
        //         directionPoint = Vector3.Dot(productRigidbody.velocity.normalized, (new Vector3(AStar.getPath[0].transform.localPosition.x, 0f, AStar.getPath[0].transform.localPosition.z) - new Vector3(product.transform.localPosition.x, 0f, product.transform.localPosition.z)).normalized);
        //     }
        //     else{
        //         directionPoint = Vector3.Dot(productRigidbody.velocity.normalized, (new Vector3(target.transform.localPosition.x, 0f, target.transform.localPosition.z) - new Vector3(product.transform.localPosition.x, 0f, product.transform.localPosition.z)).normalized);
        //     }
        //     closeness = shrunk(targetCloseness(), min:0.001f, max:30, newMin:1, newMax:10);
        //     heighpoint = Math.Abs(product.transform.localPosition.y - 6.1f);
        //     if(heighpoint<0.1f){heighpoint=0.1f;}
        //     float speed = productRigidbody.velocity.magnitude;
        //     if(directionPoint<0.4f && directionPoint>0){directionPoint*=-1;}
        //     // else if (directionPoint > 0.85f){directionPoint = 1;}
        //     if(speed < 0.1f){speed = 0.1f;}
        //     float reward = ((directionPoint*speed*speed)-heighpoint)/closeness;
        //     AddReward(shrunk(reward, reward_state:true));
        // }
        // if(!productClass.triggered){
        //     directionPoint = Vector3.Dot(productRigidbody.velocity.normalized, (target.transform.localPosition - product.transform.localPosition).normalized);
        //     float heightPoint = product.transform.localPosition.y;
        //     float closeness = targetCloseness();
        //     float speed = productRigidbody.velocity.magnitude;
        //     float adder = MazeDifficulty+0.5f;
        //     if(adder < 0.5f){adder = 0.5f;}
        //     if(directionPoint<adder && directionPoint>0){directionPoint*=-1;}
        //     if(speed < 0.1f){speed = 0.1f;}
        //     if(closeness<0.1){closeness = 0.1f;}
        //     float reward_increase = directionPoint * speed * speed * 0.0001f;
        //     float reward_decrease = (float)Math.Pow(closeness, 0.1f);
        //     float reward = reward_increase / reward_decrease;
        //     if(directionPoint>=0){
        //         reward += heightPoint*0.0001f*0.03f;
        //     }
        //     AddReward(reward);            
        // }
        if(!productClass.triggered){
            directionPoint = Vector3.Dot(productRigidbody.velocity.normalized, (target.transform.localPosition - product.transform.localPosition).normalized);
            float heightPoint = product.transform.localPosition.y;
            float closeness = targetCloseness();
            float speed = productRigidbody.velocity.magnitude;
            if(directionPoint<0.6f && directionPoint>0 && !maze){directionPoint*=-1;}
            if(speed < 0.1f){speed = 0.1f;}
            if(closeness<0.1){closeness = 0.1f;}
            float reward_increase = directionPoint * speed * 0.0002f;
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

    private float shrunk(float reward, float min = -100 , float max = 100, float newMin = -0.1f, float newMax = 0.1f, bool reward_state = false){
        if(reward_state==true){
            if(reward>0){newMin=0; min = 0;}
            if(reward<0){newMax=0; max = 0;}
        }
        return newMin + ((newMax-newMin)*(reward-min)/(max-min));
    }    
    
    public override void OnEpisodeBegin()
    {
        if(!setDifficulty){
            if(MazeDifficulty > 0.5f){MazeDifficulty = 0.5f;}
            else if(MazeDifficulty < 0.05f){MazeDifficulty = 0.05f;}
        }
        MaxStep = (int)(400); // 300f+ MazeDifficulty*100
        if(CompletedEpisodes%5 == 0){ // maze && (int)(MazeDifficulty*10+1)%(CompletedEpisodes+1) < 3
            Labyrynt.SetMaze(MazeDifficulty);
        }
        gameStates.Enqueue(winState);
        if(gameStates.Count > 100){
            gameStates.Dequeue();
        }
        winState = 0;
        recorder.Add("Custom/Win Percentage of Last 100 Episodes",CalculatePercentageOfOnes(),StatAggregationMethod.Average);
        recorder.Add("Custom/Completed Episodes",CompletedEpisodes,StatAggregationMethod.Average);
        recorder.Add("Custom/Avg Step",lastStep,StatAggregationMethod.Average);
        recorder.Add("Custom/Max Step",MaxStep,StatAggregationMethod.Average);
        if(maze){
            recorder.Add("Custom/Maze Difficulty",MazeDifficulty,StatAggregationMethod.Average);
            recorder.Add("Custom/Number of Walls",Labyrynt.wallNumber,StatAggregationMethod.Average);
        }
        product.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        productRigidbody.velocity = Vector3.zero;
        activeArray = new Transform[size*size-specifiedPoints.Count];
        ObjectPos();
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
        if(!setDifficulty){MazeDifficulty -= 0.065f;}
        AddReward(-4f);
        EndEpisode();
    }
    
    public void winReset(){
        win++;
        if(!setDifficulty){MazeDifficulty += 0.065f;}
        winState = 1;
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

        if(!maze){
            sensor.AddObservation(target.transform.localPosition);
        }
        else{
            if(AStar.getPath != null && AStar.getPath.Count > 0){
                sensor.AddObservation(gameObject.transform.InverseTransformPoint(AStar.getPath[0].position));
                // AStar.changeMaterial(AStar.getPath[0], focusTarget);
            }
            else{sensor.AddObservation(target.transform.localPosition);}
        }

        sensor.AddObservation(targetCloseness());
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
        Vector3 target_start;
        Vector3 product_start;
        do{
            target_start = randomPos();
            product_start = randomPos();
        }while(Vector3.Distance(target_start, product_start) < 25f);
        target.transform.localPosition = target_start;
        product.transform.localPosition = product_start;
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
        ui.text = "Product States\nTarget Location: "+TargetLocation()+"\nAre we winning? "+CalculatePercentageOfOnes()+"\nBoard Size: "+rows+"x"+columns+"\nDirection: "+directionPoint+"\nSpeed: "+productRigidbody.velocity.magnitude+"\nPosition: "+product.transform.localPosition+"\nHeighpoint: "+heighpoint+"\nDistance to Target: "+closeness+"\nReward: "+GetCumulativeReward()+"\nAction Count: "+StepCount+"\nGame Count: "+CompletedEpisodes+"\nWin Count: "+win+"\nActive Parts Map: \n"+ActiveMap();
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