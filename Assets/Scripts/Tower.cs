using UnityEngine;
using DG.Tweening;

public class Tower : MonoBehaviour
{
    private SpriteRenderer tower;

    [System.Serializable]
    public class WASDMap
    {
        public string wasdMap;
        public Tower targetTower;
    }

    public WASDMap[] wasdMap;

    private Color colorTransparent = new Color(1f, 1f, 1f, 0f);

    public Sprite tower_On;
    public Sprite tower_Off;

    private bool inited = false;    

    void Init()
    {
        tower = GetComponent<SpriteRenderer>();
        inited = true;
    }

    public void SelectTower()
    {
        if(!inited)
            Init();

        tower.sprite = tower_On;
    }

    public void DeselectTower()
    {
        if (!inited)
            Init();

        tower.sprite = tower_Off;
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
