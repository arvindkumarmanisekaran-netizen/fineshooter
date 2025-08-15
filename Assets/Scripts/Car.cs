using UnityEngine;
using System;
using DG.Tweening;
using UnityEngine.Events;

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
        Parked,
        Paused,
    }

    public eCarState carState = eCarState.None;

    private Vector3[] assignedPath;

    private SpriteRenderer car;

    private float moveSpeed = 2f;

    public Sprite car_down;
    public Sprite car_up;

    private Vector3 moveDir;

    public eCarOrientation carOrientation;

    public Vector2[] orientations;

    private float spawnTime;

    private int fineAssigned;

    public float SpawnTime
    {
        get { return spawnTime; }
    }

    public bool isFree
    {
        get { return carState == eCarState.None; }
    }

    public eCarState CarState
    {
        get { return carState; }
    }

    private bool inited = false;

    private Action<int, Car> carMessageFunction = null;

    private Color fineColor;
   
    void Init()
    {
        car = GetComponentInChildren<SpriteRenderer>();

        inited = true;
    }

    private void Awake()
    {
        if (!inited)
            Init();
    }

    public void ParkedCar(DOTweenPath path, int fineAssigned, Color fineColor, Action<int, Car> carMessageFunction)
    {
        if (!inited)
            Init();

        carState = eCarState.Parked;

        this.carMessageFunction = carMessageFunction;

        transform.DOKill();

        assignedPath = path.wps.ToArray();

        car.color = Color.white;

        this.fineColor = fineColor;

        gameObject.SetActive(true);

        this.fineAssigned = fineAssigned;

        transform.position = assignedPath[0];

        moveDir = (assignedPath[1] - assignedPath[0]).normalized;

        SetOrientation();

        InitialCarBlinker();
    }
    
    void InitialCarBlinker()
    {
        car.DOKill();
        car.color = Color.white;
        car.DOColor(fineColor, 0.5f).SetEase(Ease.InOutQuad).SetLoops(6, LoopType.Yoyo).SetDelay(1f).OnComplete(CarBlinker);
    }

    void CarBlinker()
    {
        car.DOKill();
        car.color = Color.white;
        car.DOColor(fineColor, 0.5f).SetEase(Ease.InOutQuad).SetLoops(6, LoopType.Yoyo).SetDelay(6f).OnComplete(CarBlinker);
    }

    public void StartMoving(DOTweenPath path, float speed, int fineAssigned, Color fineColor, Action<int, Car> carMessageFunction)
    {
        if (!inited)
            Init();

        spawnTime = Time.time;

        assignedPath = path.wps.ToArray();

        moveSpeed = speed;

        this.carMessageFunction = carMessageFunction;

        this.fineAssigned = fineAssigned;

        this.fineColor = fineColor;

        carState = eCarState.Moving;

        gameObject.SetActive(true);

        transform.position = assignedPath[0];

        InitialCarBlinker();

        MoveAlongPath(assignedPath);
    }

    void MoveAlongPath(Vector3[] pathArray)
    {
        transform.DOKill();
        transform.DOPath(pathArray, moveSpeed).SetSpeedBased(true).SetEase(Ease.Linear)
            .OnWaypointChange(WayPointChanged).OnStepComplete(PathComplete);
    }

    void PathComplete()
    {
        if(this.carMessageFunction != null)
        {
            this.carMessageFunction(2, this);
        }
    }

    void WayPointChanged(int index)
    {
        if(index + 1 < assignedPath.Length)
        {
            moveDir = (assignedPath[index + 1] - assignedPath[index]).normalized;

            SetOrientation();
        }
    }

    void FreeCar()
    {
        transform.DOKill();
        car.DOKill();
        car.color = Color.white;

        carState = eCarState.None;
        gameObject.SetActive(false);

        if (carMessageFunction != null)
        {
            carMessageFunction.Invoke(1, this);
        }

        carMessageFunction = null;
    }

    private void SetOrientation()
    {
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

        Vector2 scale = car.transform.localScale;

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

        car.transform.localScale = new Vector2(currentOrientation.x * Mathf.Abs(scale.x),
                                                    currentOrientation.y * Mathf.Abs(scale.y));
    }

    internal void BulletHit(int bulletValue)
    {
        fineAssigned -= bulletValue;

        if(fineAssigned <= 0)
        {
            FreeCar();
        }
        else
        {
            transform.DOShakePosition(0.05f, 0.1f, 8);
        }
    }

    public void AssignPath(DOTweenPath path)
    {
        assignedPath = path.wps.ToArray();
        transform.position = assignedPath[0];

        MoveAlongPath(assignedPath);
    }
}
