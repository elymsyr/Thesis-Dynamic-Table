public class ChildObjectManagerv071 : Agent
{
    [SerializeField]
    [Tooltip("Observation Space = size*size+11\nAction Space = size*size")]
    [Range(4, 12)]
    private int size = 7;
    [SerializeField]
    private bool showUI = false;  
    [SerializeField]
    [Range(0f, 20f)]
    private float move_speed = 8f;  
    [SerializeField]
    [Range(0, 2000)]
    private int actionLimit = 800;
    [SerializeField]
    private GameObject env;
    [SerializeField]
    private GameObject product;
    [SerializeField]
    private GameObject target;
    [SerializeField]
    private GameObject text;
    [SerializeField]
    private float[] wallBorders = {-41.75f,-24.65f,16.35f,-1.15f};
    [SerializeField]
    [Range(0f, 1f)]
    private float directionFactor = 1f;
    [SerializeField]
    [Range(0f, 1f)]
    private float heightFactor = 0.2f;     
    private Transform[] activeArray;
    private float scale = 4.3f;
    private GameObject parentObject;
    private TextMeshPro ui;
    private int rows = 15;
    private int columns = 15;
    private Vector3 target_start;
    private Vector3 product_start;
    private float minY = 0f;
    private float maxY = 3.1f;
    private Transform[,] childArray;
    private Rigidbody productRigidbody;
    private int actionCount = 0;
    private Vector3[,] tableLoc;
    private int gameCount = 0;
    private float legalY;
    private int win = 0;
    private float directionPoint = 0;
    private void Awake()
    {
        activeArray = new Transform[size*size];
        if (text != null)
        {
            ui = text.GetComponent<TextMeshPro>();
        }
        productRigidbody = product.GetComponent<Rigidbody>();
        legalY = product.transform.localPosition.y;
        parentObject = transform.gameObject;
        if (parentObject != null)
        {
            GetChildObjects();
        }
        else
        {
            UnityEngine.Debug.LogError("Parent object not assigned!");
        }
        GetActiveArray();
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
    private void updateUI()
    {
        ui.text = "Product States\nDirection: "+directionPoint+"\nPosition: "+product.transform.localPosition+"\nDistance to Target: "+targetCloseness()+"\nReward: "+GetCumulativeReward()+"\nAction Count: "+actionCount+"\nGame Count: "+gameCount+"\nWin Count: "+win+"\nActive Parts Map: \n"+ActiveMap();
    }
    public void triggerReset(){
        AddReward(-1f);
        EndEpisode();
    }
    public void winReset(){
        win++;
        AddReward(1f);
        EndEpisode();
    }
    private float targetCloseness()
    {
        return Vector3.Distance(product.transform.localPosition, target.transform.localPosition);
    }
    private float GetDistanceToChild(Transform child)
    {
        float distance = Vector3.Distance(env.transform.InverseTransformPoint(new Vector3(child.position.x, 0f, child.position.z)), env.transform.InverseTransformPoint(new Vector3(product.transform.position.x, 0f, product.transform.position.z)));
        return distance;
    }    
    private int[] FindClosestTransform()
    {
        int[] closestPosition = new int[2];
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                Transform transform = childArray[i, j];
                if (transform != null)
                {
                    float distance = GetDistanceToChild(transform);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestPosition[0] = i;
                        closestPosition[1] = j;
                    }
                }
            }
        }

        return closestPosition;
    }
    private void GetActiveArray()
    {
        int[] centerPoint = FindClosestTransform();

        int startX = centerPoint[0] - size / 2;
        int startY = centerPoint[1] - size / 2;
        int index = 0;
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if(IsIndexValid(startX + i, startY + j) && childArray[startX + i, startY + j] != null){
                    activeArray[index] = childArray[startX + i, startY + j];
                }
                else{
                    activeArray[index] = null;
                }
                index++;
            }
        }
    }
    private bool IsIndexValid(int rowIndex, int colIndex)
    {
        return rowIndex >= 0 && rowIndex < rows && colIndex >= 0 && colIndex < columns;
    }
    private string ActiveMap(){
        // GetCircularArray(radius);
        string arrayString = "";
        for (int i = 0; i < size*size; i++)
        {
            if (activeArray[i] != null){arrayString += "O ";}
            else{arrayString += "X ";}
            if ((i+1)%size == 0){arrayString += "\n";}
        }
        return arrayString;       
    }    
    public override void OnActionReceived(ActionBuffers actions)
    {
        if (actionLimit < actionCount)
        {
            AddReward(-1f);
            EndEpisode();
        }
        actionCount++;
        int index = 0;
        int movingPartsIndex = 0;

        GetActiveArray();

        foreach (Transform child in childArray)
        {
            if (child != null)
            {
                if (Array.IndexOf(activeArray, child) != -1){
                    float newYPosition = child.localPosition.y + actions.ContinuousActions[movingPartsIndex] * move_speed * Time.deltaTime;
                    newYPosition = Mathf.Clamp(newYPosition, minY, maxY);
                    child.localPosition = new Vector3(child.localPosition.x, newYPosition, child.localPosition.z);
                    movingPartsIndex++;
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
                UnityEngine.Debug.Log("Null child founded!");
            }
        }

        directionPoint = Vector3.Dot(productRigidbody.velocity.normalized, (target.transform.localPosition - product.transform.localPosition).normalized);
        float reward = 0.001f * (directionPoint*directionFactor + (product.transform.localPosition.y-40)*heightFactor);
        AddReward(reward);

        if (showUI)
        {
            updateUI();
        }
    }
    public override void OnEpisodeBegin()
    {
        productRigidbody.velocity = Vector3.zero;
        do {
            target_start = randomPos();
            product_start = randomPos();
        } while (Vector3.Distance(target_start, product_start) < 5);

        target.transform.localPosition = target_start;
        product.transform.localPosition = product_start;

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
        foreach (Transform child in activeArray)
        {
            if (child != null)
            {
                sensor.AddObservation(child.localPosition.y);
            }
            else
            {
                sensor.AddObservation(0);
            }
        }

        sensor.AddObservation(product.transform.localPosition);
        sensor.AddObservation(target.transform.localPosition);
        sensor.AddObservation(targetCloseness());
        sensor.AddObservation(wallBorders[0]+scale/2);
        sensor.AddObservation(wallBorders[1]-scale/2);
        sensor.AddObservation(wallBorders[2]-scale/2);
        sensor.AddObservation(wallBorders[3]+scale/2);
    }
    private Vector3 randomPos(){
        return new Vector3(UnityEngine.Random.Range(wallBorders[0]+scale/2+0.1f, wallBorders[1]-scale/2-0.1f), legalY, UnityEngine.Random.Range(wallBorders[3]+scale/2+0.1f, wallBorders[2]-scale/2-0.1f));
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        for (int i = 0; i < size*size; i++)
        {
            continuousActions[i] = UnityEngine.Random.Range(-1f, 1f);
        }       
    }
}

