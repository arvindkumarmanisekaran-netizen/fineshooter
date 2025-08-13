using UnityEngine;

public class CarPhysicsHandler : MonoBehaviour
{
    private Car attachedCar;

    private void Awake()
    {
        attachedCar = GetComponentInParent<Car>();
    }

    public void OnTriggerStay2D(Collider2D collider)
    {
        Car otherCar = collider.GetComponent<Car>();

        attachedCar.EncounteredCar(otherCar);
    }
}
