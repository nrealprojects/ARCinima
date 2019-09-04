using UnityEngine;

public class MoveWithCamera : MonoBehaviour
{
    private Vector3 origin;
    private float distance = -1;
    public enum FollowMode
    {
        None,
        Position,
        Rotation,
        All
    }

    public FollowMode mode = FollowMode.All;
    private Transform cameraRoot = null;
    public Vector3 offset;

    private void Start()
    {
        if (Camera.main != null)
        {
            cameraRoot = Camera.main.transform.parent;
        }
        if (Mathf.Abs(distance + 1) < float.Epsilon && cameraRoot != null)
        {
            origin = transform.position;
            distance = (cameraRoot.position - origin).magnitude;
        }
    }

    public void SetParam(float dis, Vector3 offset_pos, FollowMode m = FollowMode.All)
    {
        distance = dis;
        mode = m;
        offset = offset_pos;
    }

    public void AdjustDistance(float d)
    {
        distance = d;
    }

    void Update()
    {
        if (cameraRoot != null)
        {
            FollowCamera();
        }
    }

    void FollowCamera()
    {
#if UNITY_EDITOR
        offset = Vector3.zero;
#endif
        switch (mode)
        {
            case FollowMode.None:
                break;
            case FollowMode.Position:
                transform.position = cameraRoot.forward.normalized * distance + cameraRoot.position + offset;
                break;
            case FollowMode.Rotation:
                transform.rotation = cameraRoot.rotation;
                break;
            case FollowMode.All:
                transform.position = cameraRoot.forward.normalized * distance + cameraRoot.position + offset;
                transform.rotation = cameraRoot.rotation;
                break;
            default:
                break;
        }
    }
}