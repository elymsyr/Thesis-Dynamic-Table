using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public float gCost = 999999f;
    public float hCost = 999999f;
    public bool wallFlag = false;
    public float exposure = 2.5f;
    public LayerMask layerMask;
    public float fCost => gCost + hCost;
    public List<Transform> neighbors;
    public Transform parent;

    public void CheckCollide(){
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
        };
        foreach (Vector3 direction in directions)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, direction, out hit, exposure, layerMask))
            {
                wallFlag = true;
                break;
            }
        }
    }

    public void resetNode(Transform target){
        wallFlag = false;
        CheckCollide();
        if(!wallFlag){
            hCost = Vector3.Distance(transform.localPosition, target.localPosition);
        }
        else{hCost = 999999f;}
        gCost = 999999f;
        parent = null;
    }

    public void initializeNeighbors(List<Transform> newNeighbors){
        neighbors = newNeighbors;
    }

    public float setgCost(Transform neighbor){
        gCost = Vector3.Distance(transform.localPosition, neighbor.localPosition);
        return Vector3.Distance(transform.localPosition, neighbor.localPosition);
    }

}
