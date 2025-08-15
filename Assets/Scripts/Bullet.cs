using UnityEngine;
using DG.Tweening;

public class Bullet : MonoBehaviour
{
    public float moveSpeed = 1000f;

    private Vector3 moveDir;

    private bool fired = false;

    private Vector3 currentPos;

    private float limit = 6f;

    private int bulletValue = 0;

    public AudioClip carHitClip;
    public AudioClip wrongHitClip;
    public AudioClip buildingHitClip;
    public AudioClip waterHitClip;

    public bool Fired
    {
        get { return fired; }
    }

    private Vector3 offset = new Vector3(-0.5f, -0.5f, 0f);

    public void Fire(Vector3 towerPosition, int bulletValue)
    {
        Vector3 pos1 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos1.z = 0f;
        gameObject.SetActive(true);

        transform.position = towerPosition;
        this.moveDir = (pos1 - towerPosition).normalized;

        float angle = Vector2.SignedAngle(Vector2.up, moveDir);
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        this.bulletValue = bulletValue;

        transform.DOKill();
        transform.DOBlendableMoveBy(moveDir * 15f, moveSpeed).SetSpeedBased(true).SetEase(Ease.OutExpo);

        fired = true;
    }

    public void Update()
    {
        if (fired)
        {
            //currentPos = transform.position + moveDir * moveSpeed * Time.deltaTime;

            //transform.position = currentPos;

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
        transform.DOKill();
        gameObject.SetActive(false);
        fired = false;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.layer == LayerMask.NameToLayer("Car"))
        {
            Car car = collider.gameObject.GetComponent<Car>();

            if(bulletValue == car.FineAssigned || (car.FineAssigned >= 1000 && bulletValue == 1000))
            {
                car.BulletHit(bulletValue);
                AudioManager.instance.PlaySound(carHitClip);
            }
            else
            {
                AudioManager.instance.PlaySound(wrongHitClip);
            }
        }
        else if(collider.gameObject.layer == LayerMask.NameToLayer("Buildings"))
        {
            AudioManager.instance.PlaySound(buildingHitClip);
        }
        else if(collider.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            AudioManager.instance.PlaySound(waterHitClip);
        }

        BulletRest();
    }
}
