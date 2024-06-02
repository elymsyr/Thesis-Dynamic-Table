using UnityEngine;

public class rayRotation082 : MonoBehaviour
{

    private Quaternion _initialRotation = Quaternion.Euler(0f,0f,0f);

    
    private void Update(){
        transform.rotation = _initialRotation;
    }

}
