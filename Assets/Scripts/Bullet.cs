using UnityEngine;
using UnityEngine.UI;

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

    private Vector3 offset = new Vector3(-0.5f, -0.5f, 0f);

    public void Fire(Vector3 towerPosition)
    {
        Vector3 pos1 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos1.z = 0f;
        gameObject.SetActive(true);

        transform.position = towerPosition;
        this.moveDir = (pos1 - towerPosition).normalized;

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
