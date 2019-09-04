using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NREAL.AR;

public class ToolUITest : MonoBehaviour {
    public Transform cam;
    public GameObject toolScreen;
    public GameObject screenRoot;

    // Use this for initialization
    void Start()
    {
        if (toolScreen)
            toolScreen.SetActive(false);
        GetComponentInChildren<ARInteractiveItem>().OnClick += OnClick;
    }

    // Update is called once per frame
    void Update()
    {
        if (cam)
            transform.LookAt(cam, cam.up);
    }

    private void OnClick()
    {
        if(toolScreen)
            toolScreen.SetActive(true);
        if (screenRoot)
            screenRoot.SetActive(false);
    }
}
