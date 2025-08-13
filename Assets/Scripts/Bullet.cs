using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float moveSpeed = 1000f;

    private Vector2 moveDir;

    private bool fired = false;

    private RectTransform myRectTransform;

    private Vector2 currentPos;

    public bool Fired
    {
        get { return fired; }
    }

    private void Awake()
    {
        myRectTransform = GetComponent<RectTransform>();
    }

    public void Fire(Vector2 position, Vector2 movePosition)
    {
        gameObject.SetActive(true);

        this.moveDir = (movePosition - position).normalized;

        myRectTransform.anchoredPosition = position;

        float angle = Vector2.SignedAngle(Vector2.right, moveDir);

        myRectTransform.rotation = Quaternion.Euler(0f, 0f, angle);

        fired = true;
    }

    public void Update()
    {
        if (fired)
        {
            currentPos = myRectTransform.anchoredPosition + moveDir * moveSpeed * Time.deltaTime;

            myRectTransform.anchoredPosition = currentPos;

            if( myRectTransform.anchoredPosition.x < -Screen.width / 2 ||
                myRectTransform.anchoredPosition.x > Screen.width / 2 ||
                myRectTransform.anchoredPosition.y < -Screen.height / 2 ||
                myRectTransform.anchoredPosition.y > Screen.height / 2 )
            {
                BulletRest();
            }
        }
    }

    public void BulletRest()
    {
        gameObject.SetActive(false);
        fired = false;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        BulletRest();
    }
}
