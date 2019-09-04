using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//脚本拖到父物体上，找到inspector面板点击EditorMenuBatchSetName组件右上角的齿轮图标，执行最下方的方法“BatchResetChildrenName”。完毕后删除此脚本。
public class EditorMenuBatchSetName : MonoBehaviour {
    [Header("命名前缀")]
    public string namePrefix = "";

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    [ContextMenu("BatchResetChildrenName")]
    public void BatchResetChildrenName()
    {
        if (transform.childCount == 0)
            return;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).name = namePrefix + i;
        }
    }
}
