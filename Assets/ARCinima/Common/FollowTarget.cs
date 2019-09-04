using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [SerializeField] Transform target;

    void Update()
    {
        if (target != null)
        {
            transform.position = target.position;
        }
    }
}