using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Image = UnityEngine.UI.Image;

public class LaserModeOnOff : MonoBehaviour, IPointerDownHandler
{
    private Image laser;
    public Sprite laser_On;
    public Sprite laser_Off;
    private Action laserModeAutoClickCallback = null;

    private void Awake()
    {
        laser = GetComponent<Image>();
    }

    public void Setup(Action laserModeAutoClickCallback, bool laserModeOn)
    {
        this.laserModeAutoClickCallback = laserModeAutoClickCallback;

        SetSprite(laserModeOn);
    }

    public void SetSprite(bool laserModeOn)
    {
        laser.sprite = laserModeOn ? laser_On : laser_Off;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (laserModeAutoClickCallback != null)
        {
            laserModeAutoClickCallback.Invoke();
        }
    }
}
