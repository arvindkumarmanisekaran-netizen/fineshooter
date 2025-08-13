using UnityEngine;
//using Physics2D = RotaryHeart.Lib.PhysicsExtension.Physics2D;

public class Test : MonoBehaviour
{
    private RectTransform myRectTransform;

    public LayerMask carLayer;

    private void Awake()
    {
        //myRectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        //Collider2D[] otherCars = Physics2D.OverlapCircleAll(myRectTransform.anchoredPosition, 140f, 1 << carLayer, RotaryHeart.Lib.PhysicsExtension.PreviewCondition.Both, 0f, Color.red, Color.green);

        //print(otherCars.Length);

        RaycastHit2D[] castHit =  Physics2D.RaycastAll(transform.position, -Vector2.right, 10000f);

        print(castHit.Length);
        foreach (RaycastHit2D hit in castHit)
        {
            print(hit.collider.name);
        }
    }
}
