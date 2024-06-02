using UnityEngine;

public class productCollision077 : MonoBehaviour
{
    private GameObject target;
    private GameObject receiverObject;
    public bool triggered = false;

    public void InitializeProduct(GameObject setTarget, GameObject receiver){
        target = setTarget;
        receiverObject = receiver;
    } 

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
