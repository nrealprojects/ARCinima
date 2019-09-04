using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlwaysFaceCamera : MonoBehaviour {
    public bool isHorizontal = true;
    public bool useReverseFoward = false;  //是否用Z轴的反向朝向

    private Transform _camRoot;
    private Transform CameraRoot
    {
        get
        {
            if(_camRoot == null)
            {
                _camRoot = Camera.main.transform;
                if (_camRoot.parent != null)
                    _camRoot = _camRoot.parent;
            }
            return _camRoot;
        }
    }

    private void Awake()
    {
        if(Camera.main == null)
        {
            Debug.LogError("Please Set Main Camera!");
            Destroy(gameObject);
        }
    }

    void Update () {
        if (CameraRoot)
        {
            Vector3 targetPos = useReverseFoward ? (transform.position * 2f - CameraRoot.position) : CameraRoot.position;
            if (isHorizontal)
            {
                targetPos = new Vector3(targetPos.x, transform.position.y, targetPos.z);
                transform.LookAt(targetPos, Vector3.up);
            }
            else
            {
                transform.LookAt(targetPos);
            }
        }
    }
}
