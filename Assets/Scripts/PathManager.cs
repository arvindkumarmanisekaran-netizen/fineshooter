using UnityEngine;
using System;
using Random = UnityEngine.Random;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;

public class PathManager : MonoBehaviour
{
    public List<DOTweenPath> paths;

    public List<DOTweenPath> parkingPaths;

    public Transform routesParent;

    private Queue<int> previousReturnedPathsQueue = new Queue<int>(2);
    private Queue<int> previousReturnedParkingPathsQueue = new Queue<int>(2);

    private void Awake()
    {
        int numPaths = paths.Count;

        //for(int i = 0; i < numPaths; i++)
        //{
        //    GameObject reversePathObject = new GameObject(paths[i].name + "_Reverse");
        //    reversePathObject.transform.parent = routesParent;

        //    DOTweenPath reversePath = reversePathObject.AddComponent<DOTweenPath>();
        //    reversePath.wps = new List<Vector3>(paths[i].wps);
        //    reversePath.wps.Reverse();

        //    paths.Add(reversePath);
        //}

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

    public DOTweenPath GetRandomPath()
    {
        int randomPath = Random.Range(0, paths.Count);

        if (previousReturnedPathsQueue.Contains(randomPath))
            return GetRandomPath();

        if(previousReturnedPathsQueue.Count == 0)
        {
            previousReturnedPathsQueue.Enqueue(randomPath);
            previousReturnedPathsQueue.Enqueue(randomPath);
        }
        else 
        {
            previousReturnedPathsQueue.Dequeue();
            previousReturnedPathsQueue.Enqueue(randomPath);
        }

        return GetPath(randomPath);
    }

    public DOTweenPath GetParkingPath(int index)
    {
        return parkingPaths[index];
    }

    public DOTweenPath GetRandomParkingPath()
    {
        int randomPath = Random.Range(0, parkingPaths.Count);

        if (previousReturnedParkingPathsQueue.Contains(randomPath))
            return GetRandomParkingPath();

        if (previousReturnedParkingPathsQueue.Count == 0)
        {
            previousReturnedParkingPathsQueue.Enqueue(randomPath);
            previousReturnedParkingPathsQueue.Enqueue(randomPath);
        }
        else
        {
            previousReturnedParkingPathsQueue.Dequeue();
            previousReturnedParkingPathsQueue.Enqueue(randomPath);
        }

        return GetParkingPath(randomPath);
    }
}
