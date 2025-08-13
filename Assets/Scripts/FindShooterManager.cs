using UnityEngine;

public class FindShooterManager : MonoBehaviour
{
    public PathManager pathManager;

    public Car car;

    public Bullet[] bullets;

    public RectTransform tower;

    public void Awake()
    {
        car.StartMoving(pathManager.GetPath(0));
    }

    Bullet GetFreeBullet()
    {
        foreach (Bullet bullet in bullets)
        {
            if(!bullet.Fired)
            {
                return bullet;
            }
        }

        return null;
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 clickPosition = Input.mousePosition;
            clickPosition.x = clickPosition.x - Screen.width / 2;
            clickPosition.y = clickPosition.y - Screen.height / 2;

            Bullet bullet = GetFreeBullet();

            if (bullet != null)
            {
                bullet.Fire(tower.anchoredPosition, clickPosition);
            }
        }
    }
}
