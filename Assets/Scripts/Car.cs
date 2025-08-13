using UnityEngine;
using Radishmouse;
using UnityEngine.UI;


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

    private UILineRenderer assignedPath;

    private RectTransform myRectTransform;

    public float moveSpeed = 2f;

    public Sprite car_down;
    public Sprite car_up;

    public Image car;

    private int currentIndex;
    private Vector2 currentPos;
    private Vector2 moveDir;
    private Vector2 nextPos;

    public eCarOrientation carOrientation;

    public Vector2[] orientations;

    private void Awake()
    {
        myRectTransform = GetComponent<RectTransform>();
    }

    public void StartMoving(UILineRenderer path)
    {
        assignedPath = path;

        currentPos = assignedPath.points[0];
        myRectTransform.anchoredPosition = currentPos;

        SetCar();
    }

    void SetCar()
    {
        if (assignedPath != null)
        {
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
            currentPos = myRectTransform.anchoredPosition + moveDir * moveSpeed * Time.deltaTime;

            //print((currentPos - nextPos).sqrMagnitude);

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
