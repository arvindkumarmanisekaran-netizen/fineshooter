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
        Parked,
        Paused,
    }

    public eCarState carState = eCarState.None;

    public DOTweenPath assignedPath;

    private SpriteRenderer car;

    private float moveSpeed = 2f;

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

    private bool inited = false;

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

    public void ParkedCar(DOTweenPath path, int fineAssigned, Color fineColor)
    {
        if (!inited)
            Init();

        currentIndex = 0;

        carState = eCarState.Parked;

        transform.DOKill();

        assignedPath = path;

        car.DOKill();
        car.DOColor(fineColor, 0.5f).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);

        gameObject.SetActive(true);

        fineText.gameObject.SetActive(fineAssigned != -1);

        this.fineAssigned = fineAssigned;
        SetFineText();

        transform.position = path.wps[0];

        moveDir = (path.wps[1] - path.wps[0]).normalized;
        
        SetOrientation();
    }

    public void StartMoving(DOTweenPath path, int assignedPathIndex, float speed, int fineAssigned, Color fineColor)
    {
        if (!inited)
            Init();

        spawnTime = Time.time;

        assignedPath = path;

        moveSpeed = speed;

        car.DOKill();
        car.DOColor(fineColor, 0.5f).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);

        fineText.gameObject.SetActive(fineAssigned != -1);
        
        this.fineAssigned = fineAssigned;
        SetFineText();

        this.assignedPathIndex = assignedPathIndex;

        currentIndex = 0;

        carState = eCarState.Moving;

        gameObject.SetActive(true);

        currentPos = assignedPath.wps[0];
        transform.position = currentPos;

        SetCar();

        transform.DOKill();
        transform.DOPath(assignedPath.wps.ToArray(), moveSpeed).SetSpeedBased(true).SetEase(Ease.Linear)
            .OnStepComplete(FreeCar).OnWaypointChange(WayPointChanged);
    }

    void WayPointChanged(int index)
    {
        moveDir = (assignedPath.wps[index + 1] - assignedPath.wps[index]).normalized;

        SetOrientation();
    }

    void SetFineText()
    {
        fineText.text = "" + fineAssigned;
    }

    void FreeCar()
    {
        transform.DOKill();
        car.DOKill();

        carState = eCarState.None;
        gameObject.SetActive(false);

        currentIndex = -1;
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
