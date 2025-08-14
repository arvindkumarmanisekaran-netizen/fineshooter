using UnityEngine;
using System;
using Random = UnityEngine.Random;
using DG.Tweening;
using System.Collections.Generic;

public class PathManager : MonoBehaviour
{
    public List<DOTweenPath> paths;

    public List<DOTweenPath> parkingPaths;

    private int previousReturnedPath = -1;

    private int previousReturnedParkingPath = -1;

    public Transform routesParent;

    private void Awake()
    {
        int numPaths = paths.Count;

        for(int i = 0; i < numPaths; i++)
        {
            GameObject reversePathObject = new GameObject(paths[i].name + "_Reverse");
            reversePathObject.transform.parent = routesParent;

            DOTweenPath reversePath = reversePathObject.AddComponent<DOTweenPath>();
            reversePath.wps = new List<Vector3>(paths[i].wps);
            reversePath.wps.Reverse();

            paths.Add(reversePath);
        }

        numPaths = parkingPaths.Count;

        for (int i = 0; i < numPaths; i++)
        {
            GameObject reversePathObject = new GameObject(parkingPaths[i].name + "_Reverse");
            reversePathObject.transform.parent = routesParent;

            DOTweenPath reversePath = reversePathObject.AddComponent<DOTweenPath>();
            reversePath.wps = new List<Vector3>(parkingPaths[i].wps);
            reversePath.wps.Reverse();

            parkingPaths.Add(reversePath);
        }
    }

    public DOTweenPath GetPath(int index)
    {
        return paths[index];
    }

    public DOTweenPath GetRandomPath(out int pathIndex)
    {
        int randomPath = Random.Range(0, paths.Count);

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
        int randomPath = Random.Range(0, parkingPaths.Count);

        if (randomPath == previousReturnedParkingPath)
            return GetRandomParkingPath();

        previousReturnedParkingPath = randomPath;

        return GetParkingPath(randomPath);
    }
}
