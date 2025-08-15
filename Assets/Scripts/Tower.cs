using UnityEngine;
using DG.Tweening;

public class Tower : MonoBehaviour
{
    public SpriteRenderer tower_flash;

    [System.Serializable]
    public class WASDMap
    {
        public string wasdMap;
        public Tower targetTower;
    }

    public WASDMap[] wasdMap;

    private Color colorTransparent = new Color(1f, 0f, 0f, 0f);

    public AudioClip fireAudioClip;

    public void SelectTower()
    {
        tower_flash.DOKill();
        tower_flash.color = colorTransparent;
        tower_flash.DOFade(0.24f, 0.06f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutExpo);
    }

    public void DeselectTower()
    {
        tower_flash.DOKill();
        tower_flash.DOFade(0f, 0.2f);
    }

    public Tower GetTower(string key)
    {
        foreach (WASDMap map in wasdMap)
        {
            if (map.wasdMap == key)
                return map.targetTower;
        }

        return null;
    }

    public void Fire()
    {
        AudioManager.instance.PlaySound(fireAudioClip);

        tower_flash.DOKill();
        tower_flash.DOColor(new Color(1f, 1f, 1f, 0.6f), 0.1f).OnComplete(SelectTower);
    }
}
