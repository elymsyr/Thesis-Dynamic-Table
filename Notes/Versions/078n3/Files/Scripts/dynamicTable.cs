using UnityEngine;
using Unity.MLAgents.Actuators;
using Unity.MLAgents;
using TMPro;
using Unity.MLAgents.Sensors;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.MLAgents.Policies;
using Unity.Barracuda;
using System.Net;
using Unity.VisualScripting;

public class dynamicTable078 : Agent
{
    private int state = 0;
    private int winState = 0;
    private Queue<int> gameStates = new Queue<int>();
    [SerializeField] private bool showUI = false;
    [SerializeField] private GameObject text;
    private TextMeshPro ui;
    private float directionPoint = 0;
    private int win = 0;
    private CreateBoard078 table;
    private int rows;
    private int columns;
    public Transform[,] boxesArray;
    public Vector3[,] boxesLoc;
    private GameObject product;
    private GameObject target;
    private int size = 8;
    [Range(0f,15f)] public float MoveSpeed = 10f;
    private Transform[] activeArray;
    private Rigidbody productRigidbody;
    private List<Tuple<int, int>> specifiedPoints;
    private bool onTarget = false;
    private productCollision078 productClass;
    private List<float> observation;
    private RayPerceptionOutput.RayOutput[] rayOutputs;
    private RayPerceptionSensorComponent3D rayPerceptionSensor;
    private StatsRecorder recorder; 
    private float difficulty;
    private float multiplier = 0.0001f;
    private int lastStep = 0;
    private int trigger = 0;
    private float freq = 0;
    private int lose_streak = 0;
    private float heighpoint = 0.1f;

    void Awake()
    {
        recorder = Academy.Instance.StatsRecorder;
        observation = new List<float>(10);
        if (MaxStep<1){
            MaxStep = 600;
        }
        if (size == 7){ // 45 Observation 37 Actions
            specifiedPoints = new List<Tuple<int, int>>(){new Tuple<int, int>(0, 0),new Tuple<int, int>(0, 1),new Tuple<int, int>(1, 0),new Tuple<int, int>(0, 5),new Tuple<int, int>(0, 6),new Tuple<int, int>(1, 6),new Tuple<int, int>(5, 0),new Tuple<int, int>(6, 0),new Tuple<int, int>(6, 1),new Tuple<int, int>(6, 5),new Tuple<int, int>(6, 6),new Tuple<int, int>(5, 6)};
        }
        else{ // 52 Observation 55 Actions
            specifiedPoints = new List<Tuple<int, int>>(){new Tuple<int, int>(0, 0),new Tuple<int, int>(0, 1),new Tuple<int, int>(1, 0),new Tuple<int, int>(2, 0),new Tuple<int, int>(0, 2),new Tuple<int, int>(0, 5),new Tuple<int, int>(0, 6),new Tuple<int, int>(0, 7),new Tuple<int, int>(1, 7),new Tuple<int, int>(2, 7),new Tuple<int, int>(5, 0),new Tuple<int, int>(6, 0),new Tuple<int, int>(7, 0),new Tuple<int, int>(7, 1),new Tuple<int, int>(7, 2),new Tuple<int, int>(7, 5),new Tuple<int, int>(7, 6),new Tuple<int, int>(7, 7),new Tuple<int, int>(6, 7),new Tuple<int, int>(5, 7),};
        }

        if (text != null)
        {
            ui = text.GetComponent<TextMeshPro>();
        }        
        table = transform.GetComponent<CreateBoard078>();
        difficulty = table.getD;
        freq = table.gfreq;
        rows = table.rows;
        columns = table.columns;
        foreach (Transform child in table.transform)
        {
            Destroy(child.gameObject);
        }
        table.CreateEnv(difficulty);
        product = table.getProduct;
        Transform ray = product.transform.GetChild(0);
        rayPerceptionSensor = ray.GetComponent<RayPerceptionSensorComponent3D>();
        target = table.getTarget;


        if (product == null){Debug.Log("Product NULL");}
        if (target == null){Debug.Log("Target NULL");}
        productRigidbody = product.GetComponent<Rigidbody>();
        boxesArray = table.getPieces;
        rows = table.rows;
        columns = table.columns;
        productClass = product.GetComponent<productCollision078>();
        productClass.InitializeProduct(target,gameObject);
        boxesLoc = new Vector3[rows,columns];     
        for(int i=0; i<rows;i++){
            for(int j=0;j<columns;j++){
                boxesLoc[i,j]=boxesArray[i,j].transform.localPosition;
            }
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if(table.gMaze){AddReward(-0.0001f);}
        int index = 0;
        int movingPartsIndex = 0;
        onTarget = productClass.triggered;
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
        if (!onTarget){
            directionPoint = Vector3.Dot(productRigidbody.velocity.normalized, (target.transform.localPosition - product.transform.localPosition).normalized);
            float closeness = targetCloseness();
            heighpoint = Math.Abs(product.transform.localPosition.y - 6.1f);
            if(heighpoint<0.1f){heighpoint=0.1f;}
            float speed = productRigidbody.velocity.magnitude;
            if(directionPoint<0.4 && directionPoint>0 && !table.gMaze){directionPoint*=-1;}
            // else if (table.gMaze && directionPoint > -difficulty && directionPoint < 0){directionPoint*=-difficulty-directionPoint;}
            if(speed < 0.1f){speed = 0.1f;}
            if(closeness<1){closeness = 1f;}
            closeness = shrunk(closeness, min:1, max:40, newMin:1, newMax:10);
            float reward = ((directionPoint*speed*speed)-heighpoint)/closeness;
            AddReward(shrunk(reward, reward_state:true));
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
        gameStates.Enqueue(winState);
        if(gameStates.Count > 300){
            gameStates.Dequeue();
        }
        winState = 0;
        MaxStep = (int)(400f+freq);
        if (freq > 500){
            freq = 100;
        }
        else if(freq < 10){
            freq = 10;
        }
        if(difficulty > 0.6f){
            difficulty = 0.6f;
        }
        else if(difficulty < 0.01f){
            difficulty = 0.01f;
        }
        recorder.Add("Custom/Win Percentage",CalculatePercentageOfOnes(),StatAggregationMethod.Average);
        recorder.Add("Custom/Completed Episodes",CompletedEpisodes,StatAggregationMethod.Average);
        recorder.Add("Custom/Avg Step",lastStep,StatAggregationMethod.Average);
        recorder.Add("Custom/Win",state,StatAggregationMethod.Average);
        recorder.Add("Custom/Freq",freq,StatAggregationMethod.Average);
        recorder.Add("Custom/Wall Difficulty",difficulty,StatAggregationMethod.Average);
        recorder.Add("Custom/Avg Wall Number",table.getWallNumber,StatAggregationMethod.Average);
        recorder.Add("Custom/Wall Crash",trigger,StatAggregationMethod.Average);
        if(CompletedEpisodes%((int)freq) == 0 && table.gMaze){
            table.SetMaze(difficulty);
        }
        difficulty = table.ObjectPos(difficulty);
        product.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        productRigidbody.velocity = Vector3.zero;
        activeArray = new Transform[size*size-specifiedPoints.Count];
        GetActiveArray();
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
        state -= 1;
        difficulty = (float)Math.Pow(difficulty,multiplier) * difficulty * 0.9f;
        freq += 0.5f;
        trigger++;
        lose_streak++;
        if (lose_streak>freq*0.8f){
            if(table.gMaze){
                table.SetMaze(difficulty-0.01f);
            }
            difficulty = table.ObjectPos(difficulty);
            lose_streak = 0;
        }
        if(difficulty>0.5f){difficulty-=0.1f;}
        AddReward(-2f);
        EndEpisode();
    }
    
    public void winReset(){
        win++;
        winState = 1;
        freq -= freq*0.2f;
        state += 1;
        difficulty = (float)((float)Math.Pow(difficulty/multiplier,multiplier) * difficulty * 1.03);
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
        sensor.AddObservation(target.transform.localPosition);
        sensor.AddObservation(targetCloseness());
        sensor.AddObservation(productRigidbody.velocity.magnitude);
        // sensor.AddObservation(difficulty);
        RayCollect();
        sensor.AddObservation(observation);
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

    private void updateUI()
    {
        ui.text = "Product States\nAre we winning? "+CalculatePercentageOfOnes()+"\nFrequency: "+freq+" ("+(int)freq+") "+"\nDifficulty: "+difficulty+"\nBoard Size: "+rows+"x"+columns+"\nDirection: "+directionPoint+"\nSpeed: "+productRigidbody.velocity.magnitude+"\nPosition: "+product.transform.localPosition+"\nHeighpoint: "+heighpoint+"\nDistance to Target: "+targetCloseness()+"\nReward: "+GetCumulativeReward()+"\nAction Count: "+StepCount+"\nGame Count: "+CompletedEpisodes+"\nWin Count: "+win+"\nActive Parts Map: \n"+ActiveMap();
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

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        for (int i = 0; i < size*size-specifiedPoints.Count; i++)
        {
            continuousActions[i] = UnityEngine.Random.Range(-1f, 1f);
        }       
    }
}