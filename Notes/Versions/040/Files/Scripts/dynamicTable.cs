move_speed: 8
actionLimit: 1500


public class ChildObjectManagerv040 : Agent
{
    [SerializeField]
    private bool switchBehavior = false;
    private GameObject parentObject;
    private int rows = 16;
    private int columns = 16;
    private Vector3 target_start;
    private Vector3 product_start;
    private float minY = 0f;
    private float maxY = 2.9f;
    [SerializeField]
    private float move_speed = 8f;
    [SerializeField]
    private int distance_lim = 8;
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
    private float last_reward = 0; 
    private float new_reward; 

    private void Awake()
    {
        productRigidbody = product.GetComponent<Rigidbody>();
        legalY = target.transform.localPosition.y;
        parentObject = transform.gameObject;
        if (parentObject != null)
        {
            GetChildObjects();
        }
        else
        {
            Debug.LogError("Parent object not assigned!");
        }
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
    public void triggerReset(){
        AddReward(-10f);
        EndEpisode();
    }
    public void winReset(){
            AddReward((actionLimit-actionCount)/(startDistance*5)+10f);
            EndEpisode();
    }
    private float targetCloseness()
    {
        float distance = Vector3.Distance(product.transform.localPosition, target.transform.localPosition);
        return distance;
    }
    private bool GetDistanceToChild(Transform child)
    {
        float distance = Vector3.Distance(env.transform.InverseTransformPoint(child.position), env.transform.InverseTransformPoint(product.transform.position));
        return distance < distance_lim;
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        new_reward = (startDistance-targetCloseness())*10/startDistance;
        var collected_reward = new_reward - last_reward;
        last_reward = new_reward;
        AddReward(collected_reward);
        if (actionLimit < actionCount)
        {
            AddReward(-10f);
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
                Debug.Log("Null child founded!");
            }
        }
    }
    public override void OnEpisodeBegin()
    {
        productRigidbody.velocity = Vector3.zero;
        last_reward = 0;
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
        sensor.AddObservation(targetCloseness());
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        for (int i = 0; i < columns*rows; i++)
        {
            continuousActions[i] = Random.Range(-1f, 1f);
        }
    }
    private Vector3 randomPos(){
        return new Vector3(Random.Range(-39.31f, -26.42f), legalY, Random.Range(1.5f, 15f));
    }
}

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

