using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class rayRotation : MonoBehaviour
{

    private Quaternion _initialRotation = Quaternion.Euler(0f,0f,0f);

    
    private void Update(){
        transform.rotation = _initialRotation;
    }

}
