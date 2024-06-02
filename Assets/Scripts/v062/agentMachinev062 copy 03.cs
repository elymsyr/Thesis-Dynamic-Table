using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.Barracuda;
using System.Diagnostics;
using UnityEngine.UIElements;
using System;

public class ChildObjectManagerv062__03 : Agent
{
    private float scale = 4.3f;
    [SerializeField]
    private bool switchBehavior = false;
    [SerializeField]
    private bool showUI = false;
    private GameObject parentObject;
    private TextMeshPro ui;
    private int rows = 15;
    private int columns = 15;
    private Vector3 target_start;
    private Vector3 product_start;
    private float minY = 0f;
    private float maxY = 3.1f;
    [SerializeField]
    private float move_speed = 8f;
    [SerializeField]
    private float distance_lim = 4.4f;
    private Transform[,] childArray;
    private Rigidbody productRigidbody;
    [SerializeField]
    private int actionLimit = 800;
    private int actionCount = 0;
    private Vector3[,] tableLoc;
    private int gameCount = 0;
    private float legalY;
    private float startDistance;
    [SerializeField]
    private GameObject env;
    [SerializeField]
    private GameObject product;
    [SerializeField]
    private GameObject target;
    private int win = 0;
    [SerializeField]
    private GameObject text;
    [SerializeField]
    private float[] wallBorders = {-41.75f,-24.65f,16.35f,-1.15f};
    [SerializeField]
    private float directionFactor = 1f;
    [SerializeField]
    private float heightFactor = 0.2f;
    // private float currentSpeed = 0;
    private void Awake()
    {

        if (text != null)
        {
            ui = text.GetComponent<TextMeshPro>();
        }
        productRigidbody = product.GetComponent<Rigidbody>();
        legalY = product.transform.localPosition.y;
        parentObject = transform.gameObject;
        if (parentObject != null)
        {
            GetChildObjects();
        }
        else
        {
            UnityEngine.Debug.LogError("Parent object not assigned!");
        }
    }
    private void printPeward()
    {
        UnityEngine.Debug.Log(GetCumulativeReward());
    }
    private void GetChildObjects()
    {
        childArray = new Transform[rows, columns];
        tableLoc = new Vector3[rows, columns];
        int index = 0;

        foreach (Transform child in parentObject.transform)
        {
            int i = index / rows;
            int j = index % columns;
            childArray[i, j] = child;
            tableLoc[i, j] = child.transform.localPosition;
            index++;
        }
    }
    
    private void updateUI()
    {
        ui.text = "Product States\nPosition: "+product.transform.position+"\nDistance to Target: "+targetCloseness()+"\nReward: "+GetCumulativeReward()+"\nAction Count: "+actionCount+"\nGame Count: "+gameCount+"\nWin Count: "+win;
    }
    public void triggerReset(){
        AddReward(-2f);
        printPeward();
        EndEpisode();
    }
    public void winReset(){
        win++;
        AddReward(2f);
        // AddReward((actionLimit-actionCount)/(startDistance*5));
        printPeward();
        EndEpisode();
    }
    private float targetCloseness()
    {
        float distance = Vector3.Distance(product.transform.localPosition, target.transform.localPosition);
        return (float)Math.Round(distance - (int)distance, 2) + (int)distance;
    }
    private bool GetDistanceToChild(Transform child)
    {
        float distance = Vector3.Distance(env.transform.InverseTransformPoint(new Vector3(child.position.x, 0f, child.position.z)), env.transform.InverseTransformPoint(new Vector3(product.transform.position.x, 0f, product.transform.position.z)));
        return distance < distance_lim;
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        if (actionLimit < actionCount)
        {
            AddReward(-2f);
            printPeward();
            EndEpisode();
        }
        actionCount++;
        int index = 0;
        foreach (Transform child in childArray)
        {
            if (child != null)
            {
                if (switchBehavior || GetDistanceToChild(child)){
                    float newYPosition = child.localPosition.y + actions.ContinuousActions[index] * move_speed * Time.deltaTime;
                    newYPosition = Mathf.Clamp(newYPosition, minY, maxY);
                    child.localPosition = new Vector3(child.localPosition.x, newYPosition, child.localPosition.z);
                }
                else
                {
                    int i = index / rows;
                    int j = index % columns;
                    child.transform.localPosition = tableLoc[i, j];
                }

                index++;
            }
            else{
                UnityEngine.Debug.Log("Null child founded!");
            }
        }
        if (showUI)
        {
            updateUI();
        }

        // // Reward for movement direction and high ground
        float directionPoint = Vector3.Dot(productRigidbody.velocity.normalized, (target.transform.localPosition - product.transform.localPosition).normalized);
        // if (directionPoint > 0.75f && product.transform.localPosition.y > 39.9 && 43 > product.transform.localPosition.y)
        // {
        //     AddReward(0.0001f * Math.Abs(directionPoint) * product.transform.localPosition.y);
        // }
        // else
        // {
        //     AddReward(-0.00001f * Math.Abs(directionPoint) * product.transform.localPosition.y);
        // }
        AddReward(0.001f * (directionPoint*directionFactor + product.transform.localPosition.y*heightFactor));
    }
    public override void OnEpisodeBegin()
    {
        productRigidbody.velocity = Vector3.zero;
        do {
            target_start = randomPos();
            product_start = randomPos();
        } while (Vector3.Distance(target_start, product_start) < 5);

        target.transform.localPosition = target_start;
        product.transform.localPosition = product_start;
        startDistance = Vector3.Distance(target_start, product_start);

        actionCount = 0;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                childArray[i,j].transform.localPosition = tableLoc[i,j];
            }
        }
        gameCount++;
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        foreach (Transform child in childArray)
        {
            if (child != null)
            {
                sensor.AddObservation(child.localPosition.y);
            }
        }

        sensor.AddObservation(product.transform.localPosition);
        sensor.AddObservation(target.transform.localPosition);
        sensor.AddObservation(targetCloseness());
        sensor.AddObservation(wallBorders[0]+scale/2);
        sensor.AddObservation(wallBorders[1]-scale/2);
        sensor.AddObservation(wallBorders[2]-scale/2);
        sensor.AddObservation(wallBorders[3]+scale/2);
    }
    private Vector3 randomPos(){
        return new Vector3(UnityEngine.Random.Range(wallBorders[0]+2f, wallBorders[1]-2f), legalY, UnityEngine.Random.Range(wallBorders[3]+2f, wallBorders[2]-2f));
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        for (int i = 0; i < columns*rows; i++)
        {
            continuousActions[i] = UnityEngine.Random.Range(-1f, 1f);
        }
    }

}
