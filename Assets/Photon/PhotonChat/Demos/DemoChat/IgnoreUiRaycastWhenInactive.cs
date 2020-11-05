using UnityEngine;


public class IgnoreUiRaycastWhenInactive : MonoBehaviour, ICanvasRaycastFilter
{
    public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
    {
        return gameObject.activeInHierarchy;
    }

    public bool IsRaycastLocationValid(Vector2 sp, UnityEngine.Camera eventCamera)
    {
        throw new System.NotImplementedException();
    }
}