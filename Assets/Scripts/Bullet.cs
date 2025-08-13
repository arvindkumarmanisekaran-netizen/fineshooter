using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float moveSpeed = 1000f;

    private Vector3 moveDir;

    private bool fired = false;

    private Vector3 currentPos;

    private float limit = 6f;

    public bool Fired
    {
        get { return fired; }
    }

    public void Fire(Vector2 position, Vector2 movePosition)
    {
        gameObject.SetActive(true);

        this.moveDir = (movePosition - position).normalized;

        transform.position = position;

        float angle = Vector2.SignedAngle(Vector2.up, moveDir);

        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        fired = true;
    }

    public void Update()
    {
        if (fired)
        {
            currentPos = transform.position + moveDir * moveSpeed * Time.deltaTime;

            transform.position = currentPos;

            if( transform.position.x < -limit ||
                transform.position.x > limit ||
                transform.position.y < -limit ||
                transform.position.y > limit )
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
