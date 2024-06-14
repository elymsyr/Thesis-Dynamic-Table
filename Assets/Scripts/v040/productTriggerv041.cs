using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class productTriggerv041 : MonoBehaviour
{
    [SerializeField]
    private bool stayWin = false;
    [SerializeField]
    private GameObject receiverObject;
    [SerializeField]
    private GameObject wall0;
    [SerializeField]
    private GameObject wall1;
    [SerializeField]
    private GameObject wall2;
    [SerializeField]
    private GameObject wall3;

    [SerializeField]
    private GameObject target;
    private float timer = 0f;
    [SerializeField]
    private float timeThreshold = 0f;

    private void OnTriggerEnter(Collider other)
    {
        if(stayWin)
        {
            timeThreshold = 5f;
        }
        if (other.gameObject == wall0)
        {
            receiverObject.SendMessage("triggerReset");
        }
        if (other.gameObject == wall1)
        {
            receiverObject.SendMessage("triggerReset");
        }
        if (other.gameObject == wall2)
        {
            receiverObject.SendMessage("triggerReset");
        }
        if (other.gameObject == wall3)
        {
            receiverObject.SendMessage("triggerReset");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == target)
        {
            timer += Time.deltaTime;
            if (timer >= timeThreshold)
            {
                timer = 0;
                receiverObject.SendMessage("winReset");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == target)
        {
            timer = 0f;
        }
    }
}