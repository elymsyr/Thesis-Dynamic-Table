using UnityEngine;
public class TargetMovement082 : MonoBehaviour
{
    private bool isMovementOn = false;
    private dynamicTable082 table;
    private float[] wallBorders = {14.2f,-15.2f,14.2f,-15.2f};
    private float X = 1;
    private float Z = 1;
    [SerializeField] private GameObject product;
    private float distance = 0;
    private bool run;

    private void Awake(){
        table = transform.parent.GetComponent<dynamicTable082>();
        isMovementOn = table.targetMovement;
    }

    void Update()
    {
        if (isMovementOn)
        {
            distance = Vector3.Distance(product.transform.localPosition, transform.localPosition);
            if (Input.anyKey)
            {
                float horizontalInput = Input.GetAxis("Horizontal");
                float verticalInput = Input.GetAxis("Vertical");
                float newXPosition = transform.localPosition.x + horizontalInput * table.getTargetSpeed * Time.deltaTime;
                newXPosition = Mathf.Clamp(newXPosition, wallBorders[1], wallBorders[0]);                
                float newZPosition = transform.localPosition.z + verticalInput * table.getTargetSpeed * Time.deltaTime;
                newZPosition = Mathf.Clamp(newZPosition, wallBorders[3], wallBorders[2]);
                transform.localPosition = new Vector3(newXPosition, transform.localPosition.y, newZPosition);
            }
            else
            {
                run = true;
                int border = Random.Range(4,15);
                if(border==14){border=9;}
                else if (border>11){border-=5;}
                else if(border>9){border-=4;}
                float horizontalInput = Random.Range(0f, 2f);
                float verticalInput = Random.Range(0f, 2f);
                float newXPosition = transform.localPosition.x + horizontalInput * Random.Range(2f,6f) * Time.deltaTime * X;
                float newZPosition = transform.localPosition.z + verticalInput * Random.Range(2f,6f) * Time.deltaTime * Z;
                if (newXPosition > wallBorders[0] - border) { X = -1 * System.Math.Abs(X); run = false; }
                else if (newXPosition < wallBorders[1] + border) { X = System.Math.Abs(X); run = false; }
                if(newZPosition > wallBorders[2]-border){ Z = -1 * System.Math.Abs(Z); run = false;}
                else if(newZPosition < wallBorders[3]+border){ Z = System.Math.Abs(Z); run = false; }
                transform.localPosition = new Vector3(newXPosition, transform.localPosition.y, newZPosition);
                if ((Vector3.Distance(product.transform.localPosition, transform.localPosition) < distance) && run){
                    float randomX = Random.Range(0f, 1f);
                    float randomZ = Random.Range(0f, 1f);
                    if (randomX < 0.3f){X = -X;}
                    if (randomZ < 0.3f){Z = -Z;}
                }
            }
        }
    }
}