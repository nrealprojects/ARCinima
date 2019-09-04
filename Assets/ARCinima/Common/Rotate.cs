using UnityEngine;

public class Rotate : MonoBehaviour
{
    private void Update()
    {
        transform.Rotate(new Vector3(0, Time.deltaTime * 10, 0));
    }
}