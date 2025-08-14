using UnityEngine;
using System;
using Random = UnityEngine.Random;
using DG.Tweening;

public class PathManager : MonoBehaviour
{
    public DOTweenPath[] paths;

    public DOTweenPath[] parkingPaths;

    private int previousReturnedPath = -1;

    private int previousReturnedParkingPath = -1;

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

    public DOTweenPath GetParkingPath(int index)
    {
        return parkingPaths[index];
    }

    public DOTweenPath GetRandomParkingPath()
    {
        int randomPath = Random.Range(0, parkingPaths.Length);

        if (randomPath == previousReturnedParkingPath)
            return GetRandomParkingPath();

        previousReturnedParkingPath = randomPath;

        return GetParkingPath(randomPath);
    }
}
