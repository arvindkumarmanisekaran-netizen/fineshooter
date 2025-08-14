using System;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;
using TMPro;
using System.Collections.Generic;
using Image = UnityEngine.UI.Image;
using System.Collections;
using log4net.Core;
using System.Runtime.ConstrainedExecution;

public class FindShooterManager : MonoBehaviour
{
    public int currentLevel = 1;

    private PathManager pathManager;
    private LevelManager levelManager;

    public Car[] cars;

    public Bullet[] bullets;

    public Tower[] towers;

    private Tower currentSelectedTower;

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

    private int currentFine = 100;

    public LineRenderer rayFromTower;

    public Texture2D cursorTexture;
    private Vector2 cursorOffset = new Vector2(16f, 16f);

    public TMP_Text fineText;
    public Image fineSelector;
    private Vector2 selectorStart = new Vector2(-70f, 66f);
    private Vector2 selectorMoveOffset = new Vector2(70f, -58f);

    private List<Car> spawnedCars = new List<Car> ();
    private bool spawningLevel = false;
    private bool gameOver = false;

    private bool smalliPadShown = false;
    public Image smalliPad;
    public Image bigiPad;

    public Image crackle;


    private eVoilation[] currentLevelVoilations;

    public void Awake()
    {
        int seed = DateTime.Now.Millisecond;

        pathManager = GetComponentInChildren<PathManager>();
        levelManager = GetComponentInChildren<LevelManager>();

        Random.InitState(seed);

        fineSelector.GetComponent<RectTransform>().anchoredPosition = selectorStart;

        Cursor.SetCursor(cursorTexture, cursorOffset, CursorMode.ForceSoftware);

        StaticAnimations();

        currentSelectedTower = towers[0];
        currentSelectedTower.SelectTower();

        SetCurrentFine();

        SetCurrentLevel();

        ToggleiPAD();
    }

    void ToggleiPAD()
    {
        smalliPadShown = !smalliPadShown;

        smalliPad.gameObject.SetActive(smalliPadShown);
        bigiPad.gameObject.SetActive(!smalliPadShown);
    }

    private void LevelComplete()
    {
        print("Level Complete: " + currentLevel);

        currentLevel += 1;  

        if(currentLevel <= levelManager.levels.Length)
        {
            SetCurrentLevel();
        }
        else
        {
            gameOver = true;
        }
    }

    void StaticAnimations()
    {
        bk_glow.DOKill();
        bk_glow.DOFade(bk_glow_fade_amount, bk_glow_duration).SetEase(bk_glow_ease);

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
        fineText.text = "" + currentFine;
    }

    IEnumerator SpawnVoilations()
    {
        int pathIndex = 0;
        Car car = null;
        DOTweenPath path = null;
        spawningLevel = true;

        foreach (eVoilation voilation in currentLevelVoilations)
        {
            switch (voilation)
            {
                case eVoilation.SPEEDING_BETWEEN_20_30:
                    car = GetRandomCar();
                    path = pathManager.GetRandomPath(out pathIndex);
                    car.StartMoving(path, pathIndex, 0.25f, levelManager.GetFine(voilation), levelManager.GetFineColor(voilation));
                    break;

                case eVoilation.SPEEDING_BETWEEN_50_60:
                    car = GetRandomCar();
                    path = pathManager.GetRandomPath(out pathIndex);
                    car.StartMoving(path, pathIndex, 0.4f, levelManager.GetFine(voilation), levelManager.GetFineColor(voilation));
                    break;

                case eVoilation.SPEEDING_GREATER_60:
                    car = GetRandomCar();
                    path = pathManager.GetRandomPath(out pathIndex);
                    car.StartMoving(path, pathIndex, 0.6f, levelManager.GetFine(voilation), levelManager.GetFineColor(voilation));
                    break;

                case eVoilation.SIGNAL_RED_LIGHT:
                    car = GetRandomCar();
                    path = pathManager.GetRandomPath(out pathIndex);
                    car.StartMoving(path, pathIndex, Random.Range(0.3f, 0.4f), levelManager.GetFine(voilation), levelManager.GetFineColor(voilation));
                    break;

                case eVoilation.SIGNAL_PEDESTRIAN:
                    car = GetRandomCar();
                    path = pathManager.GetRandomPath(out pathIndex);
                    car.StartMoving(path, pathIndex, Random.Range(0.3f, 0.45f), levelManager.GetFine(voilation), levelManager.GetFineColor(voilation));
                    break;

                case eVoilation.OTHER_MOBILE:
                    car = GetRandomCar();
                    path = pathManager.GetRandomPath(out pathIndex);
                    car.StartMoving(path, pathIndex, Random.Range(0.3f, 0.4f), levelManager.GetFine(voilation), levelManager.GetFineColor(voilation));
                    break;

                case eVoilation.OTHER_NO_SEAT_BELT:
                    car = GetRandomCar();
                    path = pathManager.GetRandomPath(out pathIndex);
                    car.StartMoving(path, pathIndex, Random.Range(0.3f, 0.4f), levelManager.GetFine(voilation), levelManager.GetFineColor(voilation));
                    break;

                case eVoilation.PARKING_ILLEGAL:
                    car = GetRandomCar();
                    path = pathManager.GetRandomParkingPath();
                    car.ParkedCar(path, levelManager.GetFine(voilation), levelManager.GetFineColor(voilation));
                    break;

                case eVoilation.PARKING_HARD_SHOULDER:
                    car = GetRandomCar();
                    path = pathManager.GetRandomParkingPath();
                    car.ParkedCar(path, levelManager.GetFine(voilation), levelManager.GetFineColor(voilation));
                    break;
            }

            spawnedCars.Add(car);

            crackle.transform.DOKill();
            crackle.transform.DOBlendableScaleBy(new Vector3(0f, -0.5f, 0f), 0.1f).SetEase(Ease.OutBounce)
                .SetLoops(4, LoopType.Yoyo);

            yield return new WaitForSeconds(1f);
        }

        spawningLevel = false;
    }

    void SetCurrentLevel()
    {
        Level level = levelManager.levels[currentLevel - 1];
        currentLevelVoilations = level.voilations;
        spawnedCars.Clear();

        StartCoroutine(SpawnVoilations());
    }

    void DrawRayFromTower()
    {
        rayFromTower.SetPosition(0, currentSelectedTower.transform.position);
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0f;
        rayFromTower.SetPosition(1, pos);
    }

    void ManageLevel()
    {
        foreach(Car car in spawnedCars)
        {
            if(!car.isFree)
            {
                return;
            }
        }

        LevelComplete();
    }

    public void Update()
    {
        if (!spawningLevel && !gameOver)
        {
            ManageLevel();
        }

        DrawRayFromTower();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            ToggleiPAD();
        }

        if (Input.anyKeyDown)
        {
            string inputString = Input.inputString.ToLower();

            int value;
            if(int.TryParse(inputString, out value))
            {
                currentFine = value * 100;
                currentFine = currentFine == 0 ? 1000 : currentFine;

                int x = 1;
                int y = 3;
                if (value > 0)
                {
                    x = (value - 1) % 3;
                    y = (value - 1) / 3;
                }

                Vector2 moveOffset = selectorMoveOffset;
                moveOffset.x *= x;
                moveOffset.y *= y;

                Vector2 currentPos = selectorStart + moveOffset;

                fineSelector.GetComponent<RectTransform>().anchoredPosition = currentPos;
            }
            else
            {
                switch (inputString)
                {
                    case " ":
                        Fire();
                        break;

                    case "":
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
            }

            SetCurrentFine();
        }
    }
}
