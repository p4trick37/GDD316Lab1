using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    [SerializeField] private List<House> houses;
    [SerializeField] private Player player;
    [SerializeField] private Shop shop;
    [SerializeField] private GameObject beacon;
    [Header("UI")]
    [SerializeField] private TMP_Text housesText;
    [SerializeField] private TMP_Text drunkLevelText;
    [SerializeField] private GameObject deathMenu;
    [SerializeField] private TMP_InputField inputField;

    [Header("Speeds")]
    [SerializeField] private bool setToSlowSpeed;
    [SerializeField] private float fastSpawnRate;
    [SerializeField] private float fastCarSpeed;
    [SerializeField] private float slowSpawnRate;
    [SerializeField] private float slowCarSpeed;
    [SerializeField] private float timeToSlowDown;
    private int housesDelivered;
    private House currentHouse;
    private void Awake()
    {
        player = FindAnyObjectByType<Player>();
        FindAllHouses();
    }

    private void Start()
    {
        StartCoroutine(StartCars());
    }

    private void Update()
    {
        currentHouse = player.GetTargetHouse();
        if(currentHouse != null)
        {
            beacon.transform.position = new Vector3(currentHouse.transform.position.x, currentHouse.transform.position.y + 50, currentHouse.transform.position.z);
        }
        else
        {
            beacon.transform.position = new Vector3(shop.transform.position.x, shop.transform.position.y + 50, shop.transform.position.z);
        }

        housesText.text = "Deliveries: " + housesDelivered;
        float drunkLevel = player.GetDrunkLevel();
        if(drunkLevel >= 1)
        {
            drunkLevelText.text = "Drunk Level: Max";
        }
        else
        {
            drunkLevelText.text = "Drunk Level: " + drunkLevel;
        }
        
    }

    private void FindAllHouses()
    {
        House[] allHouses = FindObjectsByType<House>();
        foreach (House house in allHouses)
        {
            houses.Add(house);
        }
    }

    public House PickRandomHouse()
    {
        if(houses.Count == 0)
        {
            FindAllHouses();
        }

        int randomIndex = Random.Range(0, houses.Count);
        House removedHouse = houses[randomIndex];
        houses.RemoveAt(randomIndex);
        return removedHouse;
    }

    private IEnumerator StartCars()
    {
        SetAllSpawnersSpawnRate(fastSpawnRate);
        yield return new WaitForSeconds(timeToSlowDown);
        SetAllSpawnersSpawnRate(slowSpawnRate);
        SetAllCarSpeeds(slowCarSpeed);
        setToSlowSpeed = true;
        yield return null;
    }

    public void IncreaseHousesDelivered()
    {
        housesDelivered++;
    }

    public void OnDeath()
    {
        deathMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        player.SetSpeed(0);
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    private void SetAllCarSpeeds(float speed)
    {
        Car[] allSpawnedCars = FindObjectsByType<Car>();
        foreach(Car car in allSpawnedCars)
        {
            car.SetSpeed(speed);
        }
    }

    private void SetAllSpawnersSpawnRate(float rate)
    {
        CarSpawnPoint[] spawnpoints = FindObjectsByType<CarSpawnPoint>();
        foreach(CarSpawnPoint point in spawnpoints)
        {
            point.SetSpawnRate(rate);
        }
    }

    public float NewCarSpeed()
    {
        if(setToSlowSpeed == false)
        {
            return fastCarSpeed;
        }
        else
        {
            return slowCarSpeed;
        }
    }

    public void SubmittedText()
    {
        string input = inputField.text;
        if(float.TryParse(input, out float result))
        {
            player.SetDrunkLevel(result);
        }
        else
        {
            player.SetDrunkLevel(0);
        }
        player.ExitTextField();
    }

}
