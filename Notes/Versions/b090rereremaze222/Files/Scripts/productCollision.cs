using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class productCollision090 : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private GameObject receiverObject;
    public bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == target)
        {
            receiverObject.SendMessage("winReset");
            triggered = true;
        }
        if (other.gameObject.CompareTag("Wall"))
        {
            receiverObject.SendMessage("triggerReset");
        }
    }

    private void OnTriggerExit(Collider other){
        if(other.gameObject == target){
            triggered = false;
        }
    }    
}
