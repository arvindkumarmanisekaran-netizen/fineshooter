using UnityEngine;

public class FindShooterManager : MonoBehaviour
{
    public PathManager pathManager;

    public Car car;

    public void Awake()
    {
        car.StartMoving(pathManager.GetPath(0));
    }

}
