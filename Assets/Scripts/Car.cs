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

            switch (carOrientation)
            {
                case eCarOrientation.Top:
                    car.sprite = car_up;
                    myRectTransform.localScale = new Vector2 (-Mathf.Abs(scale.x), Mathf.Abs(scale.y));
                    break;

                case eCarOrientation.Right:
                    car.sprite = car_down;
                    myRectTransform.localScale = new Vector2(Mathf.Abs(scale.x), Mathf.Abs(scale.y));
                    break;

                case eCarOrientation.Down:
                    car.sprite = car_down;
                    myRectTransform.localScale = new Vector2(-Mathf.Abs(scale.x), Mathf.Abs(scale.y));
                    break;

                case eCarOrientation.Left:
                    car.sprite = car_up;
                    myRectTransform.localScale = new Vector2(Mathf.Abs(scale.x), Mathf.Abs(scale.y));
                    break;
            }
        }
    }

    public void Update()
    {
        if (assignedPath != null)
        {
            currentPos = myRectTransform.anchoredPosition + moveDir * moveSpeed * Time.deltaTime;

            if((currentPos - nextPos).sqrMagnitude < 0.1f)
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
