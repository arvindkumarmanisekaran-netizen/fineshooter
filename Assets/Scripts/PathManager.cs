using UnityEngine;
using Radishmouse;
using System;
using Random = UnityEngine.Random;

public class PathManager : MonoBehaviour
{
    public UILineRenderer[] paths;

    public UILineRenderer GetPath(int index)
    {
        return paths[index];
    }

    private int previousReturnedPath = -1;

    public UILineRenderer GetRandomPath()
    {
        int randomPath = Random.Range(0, paths.Length);

        if (randomPath == previousReturnedPath)
            return GetRandomPath();

        previousReturnedPath = randomPath;

        return GetPath(randomPath);
    }
}
