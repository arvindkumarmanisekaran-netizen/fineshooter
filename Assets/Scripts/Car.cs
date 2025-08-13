using UnityEngine;
using System;
using DG.Tweening;

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
        car = GetComponent<SpriteRenderer>();
    }

    public void StartMoving(DOTweenPath path, int assignedPathIndex)
    {
        spawnTime = Time.time;

        assignedPath = path;

        moveSpeed = UnityEngine.Random.Range(minMoveSpeed, maxMoveSpeed);
        currentMoveSeed = moveSpeed;

        this.assignedPathIndex = assignedPathIndex;

        currentIndex = 0;

        carState = eCarState.Moving;

        gameObject.SetActive(true);

        currentPos = assignedPath.wps[0];
        transform.position = currentPos;

        SetCar();
    }

    void FreeCar()
    {
        carState = eCarState.None;
        gameObject.SetActive(false);

        currentIndex = -1;

        FindShooterManager.CarFreed();
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

            Vector2 scale = transform.localScale;

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

            transform.localScale = new Vector2(currentOrientation.x * Mathf.Abs(scale.x),
                                                        currentOrientation.y * Mathf.Abs(scale.y));
        }
    }

    public void Update()
    {
        if (assignedPath != null)
        {
            if (carState == eCarState.Moving)
            {
                currentPos = transform.position + moveDir * currentMoveSeed * Time.deltaTime;

                if ((currentPos - nextPos).sqrMagnitude < 0.1f)
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
        }
    }
}
