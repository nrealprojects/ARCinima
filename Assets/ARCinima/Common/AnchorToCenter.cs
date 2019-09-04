using UnityEngine;

public class AnchorToCenter : MonoBehaviour
{
    private float distance;
    private void Awake()
    {
        distance = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 4.0f, Screen.height / 2.0f, distance));
        transform.rotation = Camera.main.transform.parent.root.rotation;
    }
}
