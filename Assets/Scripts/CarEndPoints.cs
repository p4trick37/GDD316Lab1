using NUnit.Framework;
using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using System.Collections.Generic;

public class CarSpawnPoint : MonoBehaviour
{
    [SerializeField] private bool xPos;
    [SerializeField] private bool xNeg;
    [SerializeField] private bool zPos;
    [SerializeField] private bool zNeg;
    [SerializeField] private float timeToSpawn;
    [SerializeField] private GameObject carPrefab;
    [SerializeField] private float distanceToRight;

    [SerializeField] private bool hasSecondLane;

    private GameManager gm;

    private List<Vector3> possibleDirections = new List<Vector3>();

    private void Awake()
    {
        gm = FindAnyObjectByType<GameManager>();
    }

    private void Start()
    {
        StartCoroutine(Timer());
    }


    private IEnumerator Timer()
    {
        while(true)
        {
            yield return new WaitForSeconds(timeToSpawn);
            if(xPos == true)
            {
                GameObject car = Instantiate(carPrefab, transform.position, Quaternion.identity);
                car.GetComponent<Car>().SetDirection(new Vector3(1, 0, 0));
                car.GetComponent<Car>().SetLaneNumber(1);
                car.GetComponent<Car>().InitializeCar();
                car.GetComponent<Car>().SetSpeed(gm.NewCarSpeed());
                if (hasSecondLane == true)
                {
                    GameObject car2 = Instantiate(carPrefab, transform.position, Quaternion.identity);
                    car2.GetComponent<Car>().SetDirection(new Vector3(1, 0, 0));
                    car2.GetComponent<Car>().SetLaneNumber(2);
                    car2.GetComponent<Car>().InitializeCar();
                    car2.GetComponent<Car>().SetSpeed(gm.NewCarSpeed());
                }
            }

            if(xNeg == true)
            {
                GameObject car = Instantiate(carPrefab, transform.position, Quaternion.identity);
                car.GetComponent<Car>().SetDirection(new Vector3(-1, 0, 0));
                car.GetComponent<Car>().SetLaneNumber(1);
                car.GetComponent<Car>().InitializeCar();
                car.GetComponent<Car>().SetSpeed(gm.NewCarSpeed());
                if (hasSecondLane == true)
                {
                    GameObject car2 = Instantiate(carPrefab, transform.position, Quaternion.identity);
                    car2.GetComponent<Car>().SetDirection(new Vector3(-1, 0, 0));
                    car2.GetComponent<Car>().SetLaneNumber(2);
                    car2.GetComponent<Car>().InitializeCar();
                    car2.GetComponent<Car>().SetSpeed(gm.NewCarSpeed());
                }
            }

            if(zPos == true)
            {
                GameObject car = Instantiate(carPrefab, transform.position, Quaternion.identity);
                car.GetComponent<Car>().SetDirection(new Vector3(0, 0, 1));
                car.GetComponent<Car>().SetLaneNumber(1);
                car.GetComponent<Car>().InitializeCar();
                car.GetComponent<Car>().SetSpeed(gm.NewCarSpeed());
                if (hasSecondLane == true)
                {
                    GameObject car2 = Instantiate(carPrefab, transform.position, Quaternion.identity);
                    car2.GetComponent<Car>().SetDirection(new Vector3(0, 0, 1));
                    car2.GetComponent<Car>().SetLaneNumber(2);
                    car2.GetComponent<Car>().InitializeCar();
                    car2.GetComponent<Car>().SetSpeed(gm.NewCarSpeed());
                }
            }

            if(zNeg == true)
            {
                GameObject car = Instantiate(carPrefab, transform.position, Quaternion.identity);
                car.GetComponent<Car>().SetDirection(new Vector3(0, 0, -1));
                car.GetComponent<Car>().SetLaneNumber(1);
                car.GetComponent<Car>().InitializeCar();
                car.GetComponent<Car>().SetSpeed(gm.NewCarSpeed());
                if (hasSecondLane == true)
                {
                    GameObject car2 = Instantiate(carPrefab, transform.position, Quaternion.identity);
                    car2.GetComponent<Car>().SetDirection(new Vector3(0, 0, -1));
                    car2.GetComponent<Car>().SetLaneNumber(2);
                    car2.GetComponent<Car>().InitializeCar();
                    car2.GetComponent<Car>().SetSpeed(gm.NewCarSpeed());
                }
            }
            
        }
    }

    private Vector3 RandomDirection()
    {
        int randomIndex = Random.Range(0, possibleDirections.Count);
        return possibleDirections[randomIndex];
        
    }
    
    public void SetSpawnRate(float rate)
    {
        timeToSpawn = rate;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, 5);
    }
}
