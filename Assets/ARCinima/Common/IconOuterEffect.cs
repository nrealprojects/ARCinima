using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconOuterEffect : MonoBehaviour {
    public Transform normalEffect;
    public Transform hoverEffect;

	// Use this for initialization
	void Start () {
        SetForNormal();
    }

    public void SetForHover()
    {
        normalEffect.gameObject.SetActive(false);
        hoverEffect.gameObject.SetActive(true);
    }

    public void SetForNormal()
    {
        normalEffect.gameObject.SetActive(true);
        hoverEffect.gameObject.SetActive(false);
    }
}
