using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Highlighter : MonoBehaviour
{
    public Color highlightColor = Color.white;
    public int loops = 2;
    public float duration = 0.2f;
    public float delay = 2f;

    Image uiImage;

    private void Awake()
    {
        uiImage = GetComponent<Image>();

        Highlight();
    }

    void Highlight()
    {
        uiImage.DOColor(highlightColor, duration).SetDelay(delay).SetEase(Ease.InOutQuad)
            .SetLoops(loops, LoopType.Yoyo).OnComplete(Highlight);
    }

    private void OnDestroy()
    {
        if(uiImage != null)
        {
            uiImage.DOKill();
        }
    }
}
