using UnityEngine;
using System;
using Random = UnityEngine.Random;
using DG.Tweening;

public class PathManager : MonoBehaviour
{
    public DOTweenPath[] paths;

    private int previousReturnedPath = -1;

    public DOTweenPath GetPath(int index)
    {
        return paths[index];
    }

    public DOTweenPath GetRandomPath(out int pathIndex)
    {
        int randomPath = Random.Range(0, paths.Length);

        if (randomPath == previousReturnedPath)
            return GetRandomPath(out pathIndex);

        previousReturnedPath = randomPath;

        pathIndex = randomPath;

        return GetPath(randomPath);
    }
}
