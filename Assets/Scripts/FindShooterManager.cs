using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class FindShooterManager : MonoBehaviour
{
    public PathManager pathManager;

    public Car[] cars;

    public Bullet[] bullets;

    public Transform tower;

    private static int numCarsMoving = 0;

    public int maxCarsMoving = 10;

    public float spawnDelay = 2f;

    private float spawnTimer = 0f;

    private int numTries = 100;

    public void Awake()
    {
        int seed = DateTime.Now.Millisecond;

        Random.InitState(seed);

        spawnTimer = 0f;
    }

    Bullet GetFreeBullet()
    {
        foreach (Bullet bullet in bullets)
        {
            if(!bullet.Fired)
            {
                return bullet;
            }
        }

        return null;
    }

    public static void CarFreed()
    {
        numCarsMoving -= 1;
    }

    Car GetCar(int index)
    {
        return cars[index];
    }

    Car GetRandomCar()
    {
        for (int i = 0; i < numTries; i++)
        {
            int randomIndex = Random.Range(0, cars.Length);

            if (cars[randomIndex].isFree)
                return cars[randomIndex];
        }

        return null;
    }

    void SpawnCar()
    {
        Car car = GetRandomCar();

        if (car != null)
        {
            int pathIndex = 0;
            var path = pathManager.GetRandomPath(out pathIndex);

            if (path != null)
            {
                car.StartMoving(path, pathIndex);
                
                numCarsMoving += 1;
            }
        }
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 clickPosition = Input.mousePosition;
            clickPosition.x = clickPosition.x - Screen.width / 2;
            clickPosition.y = clickPosition.y - Screen.height / 2;

            Bullet bullet = GetFreeBullet();

            if (bullet != null)
            {
                bullet.Fire(tower.position, clickPosition);
            }
        }

        if(spawnTimer <= 0f && numCarsMoving < maxCarsMoving)
        {
            SpawnCar();
            spawnTimer = spawnDelay;
        }

        spawnTimer -= Time.deltaTime;
    }
}
