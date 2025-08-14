using UnityEngine;
using System;
using DG.Tweening;
using Physics2D = RotaryHeart.Lib.PhysicsExtension.Physics2D;
using RotaryHeart.Lib.PhysicsExtension;

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

    private DOTweenPath assignedPath;

    private SpriteRenderer car;

    public float minMoveSpeed = 10f;
    public float maxMoveSpeed = 100f;
    private float moveSpeed = 2f;
    public float currentMoveSeed = 0f;

    public Sprite car_down;
    public Sprite car_up;

    private int currentIndex;
    private Vector3 currentPos;
    private Vector3 moveDir;
    private Vector3 nextPos;

    public eCarOrientation carOrientation;

    public Vector2[] orientations;

    private int assignedPathIndex = 0;

    private float spawnTime;

    private Collider2D myCollider;

    public TMPro.TMP_Text fineText;
    private int fineAssigned;

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
        car = GetComponentInChildren<SpriteRenderer>();

        myCollider = GetComponent<Collider2D>();
    }

    public void ParkedCar(DOTweenPath path, int fineAssigned)
    {
        currentIndex = 0;

        carState = eCarState.Parked;

        gameObject.SetActive(true);

        transform.position = path.wps[0];

        moveDir = (path.wps[1] - path.wps[0]).normalized;
        
        SetOrientation();
    }

    public void StartMoving(DOTweenPath path, int assignedPathIndex, float speed = -1f, int fineAssigned = -1)
    {
        spawnTime = Time.time;

        assignedPath = path;

        if(speed == -1)
        {
            moveSpeed = UnityEngine.Random.Range(minMoveSpeed, maxMoveSpeed);
        }
        else
        {
            moveSpeed = speed;
        }

        fineText.gameObject.SetActive(fineAssigned != -1);
        
        this.fineAssigned = fineAssigned;
        SetFineText();

        currentMoveSeed = moveSpeed;

        this.assignedPathIndex = assignedPathIndex;

        currentIndex = 0;

        carState = eCarState.Moving;

        gameObject.SetActive(true);

        currentPos = assignedPath.wps[0];
        transform.position = currentPos;

        SetCar();
    }

    void SetFineText()
    {
        fineText.text = "" + fineAssigned;
    }

    void FreeCar()
    {
        carState = eCarState.None;
        gameObject.SetActive(false);

        currentIndex = -1;

        FindShooterManager.CarFreed();
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

    void SetCar()
    {
        if (assignedPath != null)
        {
            if (currentIndex + 1 >= assignedPath.wps.Count)
            {
                FreeCar();

                return;
            }

            Vector3 startPos = assignedPath.wps[currentIndex];
            nextPos = assignedPath.wps[currentIndex + 1];

            moveDir = (nextPos - startPos).normalized;

            SetOrientation();
        }
    }

    void HitCars(Collider2D[] otherCars)
    {
        bool hitOtherCar = false;
        foreach(Collider2D collider in otherCars)
        {
            if (collider == myCollider)
                continue;

            Car otherCar = collider.GetComponent<Car>();

            float otherCarSpawnTime = otherCar.SpawnTime;
            eCarState otherCarState = otherCar.carState;
            float otherCapSpeed = otherCar.currentMoveSeed;
            int otherCarPath = otherCar.AssignedPathIndex;

            if (otherCarPath != assignedPathIndex)
            {
                if(otherCarState == eCarState.Paused)
                {
                    carState = otherCarState;
                    return;
                }
                else if (spawnTime < otherCarSpawnTime)
                {
                    carState = eCarState.Paused;
                    hitOtherCar = true;
                    return;
                }
            }
        }

        if (!hitOtherCar)
        {
            if (carState == eCarState.Paused)
            {
                currentMoveSeed = moveSpeed;
                carState = eCarState.Moving;
            }
        }
    }

    public void Update()
    {
        if (assignedPath != null)
        {
            if (carState == eCarState.Moving)
            {
                currentPos = transform.position + moveDir * currentMoveSeed * Time.deltaTime;

                if ((currentPos - nextPos).sqrMagnitude < 0.01f)
                {
                    transform.position = nextPos;
                    currentIndex += 1;
                    SetCar();
                }
                else
                {
                    transform.position = currentPos;
                }
            }

            //HitCars(Physics2D.OverlapCircleAll(transform.position, 0.4f, 1 << gameObject.layer, 
            //        PreviewCondition.Both, 0f, Color.red, Color.green));
        }
    }

    internal void BulletHit(int bulletValue)
    {
        fineAssigned -= bulletValue;

        if(fineAssigned <= 0 )
        {
            FreeCar();
        }
        else
        {
            SetFineText();
        }
    }
}
