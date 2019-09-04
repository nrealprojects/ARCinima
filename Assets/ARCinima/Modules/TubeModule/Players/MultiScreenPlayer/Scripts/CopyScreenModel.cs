using UnityEngine;

public class CopyScreenModel : MonoBehaviour
{
    [Header("要复制的屏幕父物体")]
    public Transform screenRoot;

    [Header("子屏幕网格")]
    public Mesh screenMesh;

    [Header("子屏幕材质")]
    public Material screenMaterial;

    [Header("子屏幕名字")]
    public string screenName = "VideoPlayer";

    public float Width = 16f;
    public float Height = 9f;
    public float BoxColliderScale = 0.38f;


    [ContextMenu("CopyScreen")]
    public void CopyScreen()
    {
        Vector3 screenScale = new Vector3(1f, Height / Width, 0.01f);
        if (screenRoot == null)
        {
            Debug.LogError("请设置要复制的对象");
            return;
        }
        //清除当前所有子物体
        int total = transform.childCount;
        if (total > 0)
        {
            for (int i = 0; i < total; i++)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
        }

        for (int i = 0; i < screenRoot.childCount; i++)
        {
            GameObject childObj = new GameObject(i.ToString());
            childObj.transform.parent = transform;
            childObj.transform.position = screenRoot.GetChild(i).position;
            childObj.transform.rotation = screenRoot.GetChild(i).rotation;
            childObj.transform.localScale = screenRoot.GetChild(i).localScale;
            childObj.transform.Rotate(new Vector3(0f, 180f, 0f), Space.Self);
            childObj.AddComponent<BoxCollider>().size = screenScale * BoxColliderScale;

            GameObject screen = GameObject.CreatePrimitive(PrimitiveType.Quad);
            screen.name = screenName;
            screen.transform.parent = childObj.transform;
            screen.transform.localPosition = Vector3.zero;
            screen.transform.localRotation = Quaternion.identity;
            screen.transform.localScale = screenScale;
            if (screenMaterial)
                screen.GetComponent<MeshRenderer>().material = screenMaterial;

            if (screenMesh != null)
                screen.GetComponent<MeshFilter>().mesh = screenMesh;
            DestroyImmediate(screen.GetComponent<MeshCollider>());
        }
    }
}
