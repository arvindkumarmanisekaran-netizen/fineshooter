using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public enum eVoilation
{
    PARKING_ILLEGAL,
    PARKING_HARD_SHOULDER,
    SPEEDING_GREATER_60,
    SPEEDING_BETWEEN_50_60,
    SPEEDING_BETWEEN_20_30,
    SIGNAL_RED_LIGHT,
    SIGNAL_PEDESTRIAN,
    OTHER_WRECKLESS,
    OTHER_MOBILE,
    OTHER_NO_SEAT_BELT
}

[System.Serializable]
public class LevelVoilation
{
    public eVoilation voilation;
    public int fine;
    public Color highlightColor = Color.white;
    public GameObject voilationPrefab;
}

[System.Serializable]
public class Level
{
    public eVoilation[] voilations;
}

public class LevelManager : MonoBehaviour
{
    public LevelVoilation[] levelVoilations;

    public Level[] levels;

    public delegate void LevelComplete();
    public static LevelComplete OnLevelComplete = null;

    public int GetFine(eVoilation voilation)
    {
        foreach (LevelVoilation levelVoilation in levelVoilations)
        {
            if(levelVoilation.voilation == voilation)
                return levelVoilation.fine;
        }

        return 0;
    }

    public Color GetFineColor(eVoilation voilation)
    {
        foreach (LevelVoilation levelVoilation in levelVoilations)
        {
            if (levelVoilation.voilation == voilation)
                return levelVoilation.highlightColor;
        }

        return Color.white;
    }

    public GameObject GetVoilationPrefab(eVoilation voilation)
    {
        foreach (LevelVoilation levelVoilation in levelVoilations)
        {
            if (levelVoilation.voilation == voilation)
                return levelVoilation.voilationPrefab;
        }

        return null;
    }
}
