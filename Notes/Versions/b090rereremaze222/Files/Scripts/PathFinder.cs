using System.Collections.Generic;
using UnityEngine;

public class AStar090 : MonoBehaviour
{
    [SerializeField] private bool TEST = true;
    public bool TestProperty {
        get { return TEST; }
        set { TEST = value; }
    }
    public Material selectedRoad;
    public Material pathObjects;
    public Material wallDetected;
    public Material test;
    public float mean = 26;
    public float scale = 0.5f;
    [Range(1,10)] public int chooseNext = 6; 
    private int rowColumns => (int)(mean/scale);
    public Transform[,] nodes;
    public Transform product;
    public Vector3 lastPos = new Vector3(0f,0f,0f);
    public bool pathFinder = true;
    private List<Transform> path;

    public void initPathFinder(){
        nodes = new Transform[rowColumns, rowColumns];
        path = new List<Transform>();
        createPathObjects();
        initializeNeighborsAll();
    }

    public Transform PathFinder(Transform startnode, Transform targetNode){

        path = FindPath(startnode, targetNode);
        colorRoad();
        if(path.Count>chooseNext && path[chooseNext] != null){return path[chooseNext];}
        else{return null;}
    }

    public void colorRoad(){
        if(TEST){
            foreach(Transform node in path){
                changeMaterial(node, selectedRoad);
            }
        }
    }

    public void changeMaterial(Transform node, Material selected){
        if(TEST){
            Renderer renderer = node.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = selected;
        }
        }
    }

    private void createPathObjects(){
        for (int x = 0; x < rowColumns; x++)
        {
            for (int y = 0; y < rowColumns; y++)
            {
                nodes[x, y] = transform.GetChild(x * rowColumns + y);
            }
        }
    }

    private List<Transform> GetSurroundingTransforms(int x, int y)
    {
        List<Transform> surroundingTransforms = new List<Transform>();
        int[] offsetX = { -1,  0,  1, -1, 1, -1, 0, 1 };
        int[] offsetY = { -1, -1, -1,  0, 0,  1, 1, 1 };
        for (int i = 0; i < offsetX.Length; i++)
        {
            int newX = x + offsetX[i];
            int newY = y + offsetY[i];

            if (newX >= 0 && newX < rowColumns && newY >= 0 && newY < rowColumns)
            {
                surroundingTransforms.Add(nodes[newX, newY]);
            }
        }

        return surroundingTransforms;
    }

    private void initializeNeighborsAll(){
        for (int x = 0; x < rowColumns; x++)
        {
            for (int y = 0; y < rowColumns; y++)
            {
                nodes[x,y].GetComponent<Node>().initializeNeighbors(GetSurroundingTransforms(x,y));
            }
        }
    }

    public void resetNodeAll(Transform startNode){
        for (int x = 0; x < rowColumns; x++)
        {
            for (int y = 0; y < rowColumns; y++)
            {
                nodes[x,y].GetComponent<Node>().resetNode(startNode);
                changeMaterial(nodes[x,y], pathObjects);
            }
        }
    }

    private List<Transform> FindPath(Transform start, Transform goal)
    {
        List<Transform> openSet = new List<Transform>();
        HashSet<Transform> closedSet = new HashSet<Transform>();
        openSet.Add(start);
        start.GetComponent<Node>().gCost = 0;
        foreach(Transform node in nodes){
            if(node.GetComponent<Node>().wallFlag){
                changeMaterial(node, wallDetected);
            }
            else{changeMaterial(node, pathObjects);}
        }

        while (openSet.Count > 0)
        {
            Transform currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if(!openSet[i].GetComponent<Node>().wallFlag){
                    if (openSet[i].GetComponent<Node>().fCost < currentNode.GetComponent<Node>().fCost)
                    {
                        currentNode = openSet[i];
                    }
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == goal)
            {
                return RetracePath(start, goal);
            }

            foreach (Transform neighbor in currentNode.GetComponent<Node>().neighbors)
            {
                if(neighbor != null){
                    if (closedSet.Contains(neighbor))
                    {
                        continue;
                    }

                    float newMovementCostToNeighbor = currentNode.GetComponent<Node>().gCost + GetDistance(currentNode, neighbor);

                    if (newMovementCostToNeighbor < neighbor.GetComponent<Node>().gCost || !openSet.Contains(neighbor))
                    {
                        neighbor.GetComponent<Node>().gCost = newMovementCostToNeighbor;
                        neighbor.GetComponent<Node>().parent = currentNode;

                        if (!openSet.Contains(neighbor))
                        {
                            openSet.Add(neighbor);
                        }
                    }
                }
            }
        }

        return null;
    }

    private List<Transform> RetracePath(Transform startNode, Transform endNode)
    {
        List<Transform> path = new List<Transform>();
        Transform currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.GetComponent<Node>().parent;
        }
        path.Reverse();
        int n = 3;
        if (n >= path.Count)
        {
            path.Clear();
        }
        else
        {
            int startIndex = path.Count - n;
            path.RemoveRange(startIndex, n);
        }        
        return path;
    }

    private float GetDistance(Transform nodeA, Transform nodeB)
    {
        return Vector3.Distance(gameObject.transform.InverseTransformPoint(nodeA.position), gameObject.transform.InverseTransformPoint(nodeB.position));
    }  

    public Transform getPoint(Transform point){
        float distance = 1000f;
        Transform pointNode = null;
        foreach(Transform node in nodes){
            if(!node.GetComponent<Node>().wallFlag){
                float newDist = GetDistance(node, point);
                if(newDist<distance){
                    pointNode = node;
                    distance = newDist;
                }
            }
        }
        return pointNode;
    }

}
