using UnityEngine;

public class productCollision074 : MonoBehaviour
{
    private GameObject wall1;
    private GameObject wall2;
    private GameObject wall3;
    private GameObject wall4;
    private GameObject target;
    private GameObject receiverObject;
    public bool triggered = false;

    public void InitializeProduct(GameObject setWall1,GameObject setWall2,GameObject setWall3,GameObject setWall4,GameObject setTarget, GameObject receiver){
        wall1 = setWall1;
        wall2 = setWall2;
        wall3 = setWall3;
        wall4 = setWall4;
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
        if (other.gameObject == wall4)
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
    // private void OnTriggerStay(Collider other){
    //     if(other.gameObject == target){
    //         receiverObject.SendMessage("StayReward");
    //     }
    // }

    private void OnTriggerExit(Collider other){
        if(other.gameObject == target){
            triggered = false;
        }
    }    
}
