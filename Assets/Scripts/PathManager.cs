using UnityEngine;
using Radishmouse;

public class PathManager : MonoBehaviour
{
    public UILineRenderer[] paths;
    
    public UILineRenderer GetPath(int index)
    {
        return paths[index];
    }
}
