using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class productTriggerv040 : MonoBehaviour
{
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

    private void OnTriggerEnter(Collider other)
    {
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

                                     
        if (other.gameObject == target)
        {
            receiverObject.SendMessage("winReset");
        }
    }
}
