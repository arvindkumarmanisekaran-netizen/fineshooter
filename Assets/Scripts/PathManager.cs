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

    public UILineRenderer GetRandomPath(out int pathIndex)
    {
        int randomPath = Random.Range(0, paths.Length);

        if (randomPath == previousReturnedPath)
            return GetRandomPath(out pathIndex);

        previousReturnedPath = randomPath;

        pathIndex = randomPath;

        return GetPath(randomPath);
    }
}
