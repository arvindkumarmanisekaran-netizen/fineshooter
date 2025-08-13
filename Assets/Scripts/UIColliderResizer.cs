using UnityEngine;
using UnityEngine.UI;

public class UIColliderResizer : MonoBehaviour
{
    private RectTransform rectTransform;
    private BoxCollider2D boxCollider;

    public bool runOnce = true;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        boxCollider = GetComponent<BoxCollider2D>();
        

        if (runOnce)
            Run();
    }

    void Run()
    {
        // Get the size of the RectTransform
        Vector2 rectSize = rectTransform.sizeDelta;

        // Apply the size to the BoxCollider2D
        boxCollider.size = rectSize;
    }

    void Update()
    {
        if(!runOnce)
        {
            Run();
        }
    }
}