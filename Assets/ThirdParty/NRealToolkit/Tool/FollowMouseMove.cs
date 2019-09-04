using UnityEngine;

public class FollowMouseMove : MonoBehaviour
{
    [SerializeField] Transform cameraTran;
    [SerializeField] float moveSpeed = 5.0f;
    private float mouseX, mouseY;
    private bool needUpdateRotation = false;
    private bool needUpdatePosition = false;

#if UNITY_EDITOR
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            needUpdateRotation = true;
        }
        if (Input.GetMouseButtonUp(1))
        {
            needUpdateRotation = false;
        }

        if (needUpdateRotation)
        {
            mouseX = Input.GetAxis("Mouse X") * moveSpeed;
            mouseY = Input.GetAxis("Mouse Y") * moveSpeed;

            cameraTran.localEulerAngles = cameraTran.localEulerAngles + new Vector3(-mouseY, mouseX, 0);
        }

        if (Input.GetMouseButtonDown(0))
        {
            needUpdatePosition = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            needUpdatePosition = false;
        }

        if (needUpdatePosition)
        {
            mouseX = Input.GetAxis("Mouse X");
            mouseY = Input.GetAxis("Mouse Y");

            cameraTran.localPosition = cameraTran.localPosition + cameraTran.forward * mouseY + cameraTran.right * mouseX;
        }
    }
#endif
}