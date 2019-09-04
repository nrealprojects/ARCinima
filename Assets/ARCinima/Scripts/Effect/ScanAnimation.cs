using UnityEngine;

public class ScanAnimation : MonoBehaviour
{
    public float speed = 1;
    private Material material;
    private float value = 0;

    private void Awake()
    {
        material = gameObject.GetComponent<MeshRenderer>().material;
    }

    private void Update()
    {
        value += Time.deltaTime * speed;
        if (value > 1)
        {
            value = 0f;
        }
        material.SetFloat("_Stealth", value);
    }
}