using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NREAL.AR{
public class CameraController : MonoBehaviour {
	public float speed;
	private Camera[] camera;
    
	// Use this for initialization
	void Start () {
		// 形变组件transform,与该脚本直接关联上的组件就是transform
		camera = GetComponentsInChildren<Camera> ();
		Screen.SetResolution (3840,1080,true);
	}

	// Update is called once per frame
	void Update () {
		// 得到鼠标当前位置
		float mouseX = Input.GetAxis ("Mouse X") * speed;
		float mouseY = Input.GetAxis ("Mouse Y") * speed;
		// 设置照相机和Player的旋转角度，X,Y值需要更具情况变化位置
		camera[0].transform.localRotation = camera[0].transform.localRotation * Quaternion.Euler ( -mouseY, mouseX, 0);
		camera[1].transform.localRotation = camera[1].transform.localRotation * Quaternion.Euler ( -mouseY, mouseX, 0);
//		transform.localRotation = transform.localRotation * Quaternion.Euler ( 0, mouseX, 0);
		if (Input.GetKeyDown(KeyCode.Q)) {
			Application.Quit ();
		}

	}
}
}