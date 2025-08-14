using System;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;
using TMPro;
using PlasticGui.WorkspaceWindow.QueryViews;
using NUnit.Framework;
using System.Collections.Generic;

public class FindShooterManager : MonoBehaviour
{
    public PathManager pathManager;

    public Car[] cars;

    public Bullet[] bullets;

    public Tower[] towers;

    private Tower currentSelectedTower;

    private static int numCarsMoving = 0;

    public int maxCarsMoving = 10;

    public float spawnDelay = 2f;

    private float spawnTimer = 0f;

    private int numTries = 100;

    private Vector3 offset = new Vector3(-0.5f, -0.5f, 0f);

    public SpriteRenderer bk_glow, noise_1, noise_2;

    public Ease noise_1_ease;
    public float noise_1_duration;
    public Vector3 noise_1_moveby;
    public Ease noise_2_ease;
    public float noise_2_duration;
    public Vector3 noise_2_moveby;
    public Ease bk_glow_ease;
    public float bk_glow_duration;
    public float bk_glow_fade_amount;

    public TMP_InputField fineField;

    private int currentFine = 100;

    public int currentLevel = 1;

    public LevelManager levelManager;

    private List<Car> spawnedCars = new List<Car>();


    public void Awake()
    {
        int seed = DateTime.Now.Millisecond;

        Random.InitState(seed);

        spawnTimer = 0f;

        StaticAnimations();

        currentSelectedTower = towers[0];
        currentSelectedTower.SelectTower();

        SetCurrentFine();

        SetCurrentLevel();
    }

    private void OnEnable()
    {
        LevelManager.OnLevelComplete += LevelComplete;
    }

    private void OnDisable()
    {
        LevelManager.OnLevelComplete -= LevelComplete;
    }

    private void LevelComplete()
    {
        print("Level Complete: " + currentLevel);

        currentLevel += 1;  

        if(currentLevel <= levelManager.levels.Length)
        {
            SetCurrentLevel();
        }
    }

    void StaticAnimations()
    {
        DOTween.ToAlpha(() => bk_glow.color, x => bk_glow.color = x, bk_glow_fade_amount, bk_glow_duration).SetLoops(-1, LoopType.Yoyo);

        noise_1.DOKill();
        noise_1.transform.DOBlendableLocalMoveBy(noise_1_moveby, noise_1_duration).SetEase(noise_1_ease).SetLoops(-1, LoopType.Yoyo);

        noise_2.DOKill();
        noise_2.transform.DOBlendableLocalMoveBy(noise_2_moveby, noise_2_duration).SetEase(noise_2_ease).SetLoops(-1, LoopType.Yoyo);
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

    void Fire()
    {
        Bullet bullet = GetFreeBullet();

        if (bullet != null)
        {
            bullet.Fire(currentSelectedTower.transform.position, currentFine);
        }
    }

    void SetCurrentFine()
    {
        fineField.text = "" + currentFine;
    }

    void SetCurrentLevel()
    {
        Level level = levelManager.levels[currentLevel - 1];
        spawnedCars = new List<Car> { GetRandomCar() };
        int pathIndex = 0;
        Car car = null;
        DOTweenPath path = null;

        spawnedCars.Clear();
        foreach (eVoilation voilation in level.voilations)
        {
            switch (voilation)
            {
                case eVoilation.SPEEDING_BETWEEN_20_30:
                    car = GetRandomCar();
                    path = pathManager.GetRandomPath(out pathIndex);
                    car.StartMoving(path, pathIndex, 0.25f, levelManager.GetFine(voilation));
                    break;

                case eVoilation.SPEEDING_BETWEEN_50_60:
                    car = GetRandomCar();
                    path = pathManager.GetRandomPath(out pathIndex);
                    car.StartMoving(path, pathIndex, 0.4f, levelManager.GetFine(voilation));
                    break;

                case eVoilation.SPEEDING_GREATER_60:
                    car = GetRandomCar();
                    path = pathManager.GetRandomPath(out pathIndex);
                    car.StartMoving(path, pathIndex, 0.6f, levelManager.GetFine(voilation));
                    break;

                case eVoilation.SIGNAL_RED_LIGHT:
                    car = GetRandomCar();
                    path = pathManager.GetRandomPath(out pathIndex);
                    car.StartMoving(path, pathIndex, Random.Range(0.3f, 0.4f), levelManager.GetFine(voilation));
                    break;

                case eVoilation.SIGNAL_PEDESTRIAN:
                    car = GetRandomCar();
                    path = pathManager.GetRandomPath(out pathIndex);
                    car.StartMoving(path, pathIndex, Random.Range(0.3f, 0.45f), levelManager.GetFine(voilation));
                    break;

                case eVoilation.OTHER_MOBILE:
                    car = GetRandomCar();
                    path = pathManager.GetRandomPath(out pathIndex);
                    car.StartMoving(path, pathIndex, Random.Range(0.3f, 0.4f), levelManager.GetFine(voilation));
                    break;

                case eVoilation.OTHER_NO_SEAT_BELT:
                    car = GetRandomCar();
                    path = pathManager.GetRandomPath(out pathIndex);
                    car.StartMoving(path, pathIndex, Random.Range(0.3f, 0.4f), levelManager.GetFine(voilation));
                    break;

                case eVoilation.PARKING_ILLEGAL:
                    car = GetRandomCar();
                    path = pathManager.GetRandomParkingPath();
                    car.ParkedCar(path, levelManager.GetFine(voilation));
                    break;

                case eVoilation.PARKING_HARD_SHOULDER:
                    car = GetRandomCar();
                    path = pathManager.GetRandomParkingPath();
                    car.ParkedCar(path, levelManager.GetFine(voilation));
                    break;
            }
            
            spawnedCars.Add(car);
        }

        levelManager.ManageLevel(spawnedCars);
    }

    public void Update()
    {
        if(Input.anyKeyDown)
        {
            string inputString = Input.inputString.ToLower();

            switch(inputString)
            {
                case " ":
                    Fire();
                    break;

                case "":
                    break;

                case "1":
                    currentFine = 100;
                    break;

                case "2":
                    currentFine = 200;
                    break;

                case "3":
                    currentFine = 300;
                    break;

                case "4":
                    currentFine = 400;
                    break;

                case "5":
                    currentFine = 500;
                    break;

                default:
                    Tower newTower = currentSelectedTower.GetTower(inputString);
                    if (newTower != null)
                    {
                        currentSelectedTower.DeselectTower();
                        currentSelectedTower = newTower;
                        currentSelectedTower.SelectTower();
                    }
                    break;
            }

            SetCurrentFine();
        }

        //if(spawnTimer <= 0f && numCarsMoving < maxCarsMoving)
        //{
        //    SpawnCar();
        //    spawnTimer = spawnDelay;
        //}

        //spawnTimer -= Time.deltaTime;
    }
}
