using UnityEngine;
using DG.Tweening;

public class CashAnimation : MonoBehaviour
{
    public SpriteRenderer cash;

    public Ease ease1, ease2;
    public float duration1, duration2;
    public Vector3 scale = Vector3.one;

    private void OnEnable()
    {
        cash.transform.DOKill();
        cash.transform.DOScale(scale, duration1).SetEase(ease1);
        cash.transform.DOScale(0f, duration2).SetEase(ease2).OnComplete(DestroyMe).SetDelay(duration1);
    }

    void DestroyMe()
    {
        GameObject.Destroy(gameObject);
    }
}
