using System.Linq;
using UnityEngine;

public class Car : MonoBehaviour
{
    [SerializeField] private float speed;
    private Vector3 direction;
    private Vector3 startPosition;
    [SerializeField] private CarSpawnPoint targetEndPoint;
    [SerializeField] private float distanceToRight;

    private int laneNumber;

    private float invincibleTimer = 0;
    private bool canKill = false;

    public void InitializeCar()
    {
        transform.rotation = Quaternion.LookRotation(direction);
        transform.position = transform.position + (distanceToRight * transform.right * Mathf.Pow(4, laneNumber - 1));
        RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.forward);
        for (int i = hits.Length - 1; i >= 0; i--)
        {
            CarSpawnPoint endPoint = hits[i].transform.GetComponent<CarSpawnPoint>();
            if (endPoint != null)
            {
                targetEndPoint = endPoint;
                break;
            }
        }
    }


    private void Update()
    {
        transform.position += speed * transform.forward * Time.deltaTime;
        if(invincibleTimer >= 1)
        {
            canKill = true;
        }
        else
        {
            invincibleTimer += Time.deltaTime;
        }
    }

    public void SetDirection(Vector3 spawnedDirection)
    {
        direction = spawnedDirection;
    }

    public void SetStartPosition(Vector3 spawnPosition)
    {
        startPosition = spawnPosition + (distanceToRight * transform.right);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<CarSpawnPoint>() != null && other.gameObject.GetComponent<CarSpawnPoint>() == targetEndPoint)
        {
            Destroy(gameObject);
        }
    }

    public void SetLaneNumber(int number)
    {
        laneNumber = number;
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    public bool GetKillStatus()
    {
        return canKill;
    }

}
