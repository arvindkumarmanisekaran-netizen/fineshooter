using UnityEngine;
using DG.Tweening;

public class Tower : MonoBehaviour
{
    public SpriteRenderer glow;

    [System.Serializable]
    public class WASDMap
    {
        public string wasdMap;
        public Tower targetTower;
    }

    public WASDMap[] wasdMap;

    private Color colorTransparent = new Color(1f, 1f, 1f, 0f);

    public void SelectTower()
    {
        glow.DOKill();
        glow.DOFade(1f, 0.5f);
    }

    public void DeselectTower()
    {
        glow.DOKill();
        glow.DOFade(0f, 0.3f);
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
}
