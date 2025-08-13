using UnityEngine;
using Radishmouse;
using UnityEngine.UI;
using System;
//using Physics2D = RotaryHeart.Lib.PhysicsExtension.Physics2D;


public class Car : MonoBehaviour
{
    [System.Serializable]
    public enum eCarOrientation
    {
        Left,
        Right,
        Top,
        Down
    }

    public enum eCarState
    {
        None,
        Moving,
        Paused
    }

    public eCarState carState = eCarState.None;

    private UILineRenderer assignedPath;

    private RectTransform myRectTransform;

    public float minMoveSpeed = 10f;
    public float maxMoveSpeed = 100f;
    private float moveSpeed = 2f;
    public float currentMoveSeed = 0f;

    public Sprite car_down;
    public Sprite car_up;

    public Image car;

    private int currentIndex;
    private Vector2 currentPos;
    private Vector2 moveDir;
    private Vector2 nextPos;

    public eCarOrientation carOrientation;

    public Vector2[] orientations;

    private int assignedPathIndex = 0;

    private float spawnTime;

    public float SpawnTime
    {
        get { return spawnTime; }
    }

    public int AssignedPathIndex
    {
        get { return assignedPathIndex; }
    }

    public bool isFree
    {
        get { return carState == eCarState.None; }
    }


    public eCarState CarState
    {
        get { return carState; }
    }

    private void Awake()
    {
        myRectTransform = GetComponent<RectTransform>();
    }

    public void StartMoving(UILineRenderer path, int assignedPathIndex)
    {
        spawnTime = Time.time;

        assignedPath = path;

        moveSpeed = UnityEngine.Random.Range(minMoveSpeed, maxMoveSpeed);
        currentMoveSeed = moveSpeed;

        this.assignedPathIndex = assignedPathIndex;

        carState = eCarState.Moving;

        gameObject.SetActive(true);

        currentPos = assignedPath.points[0];
        myRectTransform.anchoredPosition = currentPos;

        SetCar();
    }

    void SetCar()
    {
        if (assignedPath != null)
        {
            if (currentIndex + 1 >= assignedPath.points.Length)
            {
                carState = eCarState.None;
                gameObject.SetActive(false);

                FindShooterManager.CarFreed();

                return;
            }

            Vector2 startPos = assignedPath.points[currentIndex];
            nextPos = assignedPath.points[currentIndex + 1];

            moveDir = (nextPos - startPos).normalized;

            if (moveDir.x > 0f && moveDir.y > 0f)
            {
                carOrientation = eCarOrientation.Top;
            }
            else if (moveDir.x > 0f && moveDir.y < 0f)
            {
                carOrientation = eCarOrientation.Right;
            }
            else if (moveDir.x < 0f && moveDir.y < 0f)
            {
                carOrientation = eCarOrientation.Down;
            }
            else if (moveDir.x < 0f && moveDir.y > 0f)
            {
                carOrientation = eCarOrientation.Left;
            }

            Vector2 scale = myRectTransform.localScale;

            Vector2 currentOrientation = orientations[(int)carOrientation];

            switch (carOrientation)
            {
                case eCarOrientation.Top:
                    car.sprite = car_up;
                    break;

                case eCarOrientation.Right:
                    car.sprite = car_down;
                    break;

                case eCarOrientation.Down:
                    car.sprite = car_down;
                    break;

                case eCarOrientation.Left:
                    car.sprite = car_up;
                    break;
            }

            myRectTransform.localScale = new Vector2(   currentOrientation.x * Mathf.Abs(scale.x),
                                                        currentOrientation.y * Mathf.Abs(scale.y));
        }
    }

    public void Update()
    {
        if (assignedPath != null)
        {
            if(carState == eCarState.Paused)
            {
                float timeElapsedOtherCar = Time.time - otherCarEncounterTime;
                float timeElapsedSameRouteCar = Time.time - sameRouteCarEncouterTime;

                if (timeElapsedOtherCar > 2f)
                {
                    SetCarState(eCarState.Moving);
                }
                else if (timeElapsedSameRouteCar > 2f)
                {
                    SetCarState(eCarState.Moving);
                }
            }

            if (carState == eCarState.Moving)
            {
                currentPos = myRectTransform.anchoredPosition + moveDir * currentMoveSeed * Time.deltaTime;

                if ((currentPos - nextPos).sqrMagnitude < 100f)
                {
                    myRectTransform.anchoredPosition = nextPos;
                    currentIndex += 1;
                    SetCar();
                }
                else
                {
                    myRectTransform.anchoredPosition = currentPos;
                }
            }
        }
    }

    private float carPauseTime = 0f;
    private float otherCarEncounterTime = 0f;
    private float sameRouteCarEncouterTime = 0f;
    public void SetCarState(eCarState state)
    {
        carState = state;

        switch (carState)
        {
            case eCarState.Paused:
                carPauseTime = 0f;
                break;
        }
    }

    public void EncounteredCar(Car otherCar)
    {
        int otherCarRoute = otherCar.AssignedPathIndex;
        float otherCarSpawnTime = otherCar.SpawnTime;
        eCarState otherCarState = otherCar.CarState;

        if(otherCarRoute == assignedPathIndex)
        {
            if (spawnTime > otherCarSpawnTime)
            {
                if(otherCarState == eCarState.Paused)
                {
                    SetCarState(eCarState.Paused);

                    sameRouteCarEncouterTime = Time.time;
                }

                currentMoveSeed = otherCar.moveSpeed;
            }
        }
        else
        {
            if (spawnTime > otherCarSpawnTime)
            {
                otherCarEncounterTime = Time.time;

                SetCarState(eCarState.Paused);
            }
        }
    }
}
