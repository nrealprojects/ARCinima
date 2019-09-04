using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutLine : MonoBehaviour
{
    [SerializeField] Transform mQuard;
    private Vector3 pointLT;
    private Vector3 pointLB;
    private Vector3 pointRT;
    private Vector3 pointRB;

    private Renderer[] mRenderList;

    void Start()
    {
        Vector3 posbottomLeft = new Vector3(-0.5f, -0.5f, 0);
        Vector3 posbottomright = new Vector3(0.5f, -0.5f, 0);
        Vector3 postopright = new Vector3(0.5f, 0.5f, 0);
        Vector3 postopleft = new Vector3(-0.5f, 0.5f, 0);

        pointLB = mQuard.TransformPoint(posbottomLeft);
        pointRB = mQuard.TransformPoint(posbottomright);
        pointRT = mQuard.TransformPoint(postopright);
        pointLT = mQuard.TransformPoint(postopleft);


        LineRenderer line = gameObject.GetComponent<LineRenderer>();
        line.positionCount = 6;
        line.startWidth = LineWrite.lineWidth;
        line.endWidth = LineWrite.lineWidth;
        line.SetPositions(new Vector3[6] { pointLB, pointRB, pointRT, pointLT, pointLB, pointRB });

        mRenderList = transform.parent.GetComponentsInChildren<Renderer>();
        StartCoroutine(ColorShader());
    }

    private IEnumerator ColorShader()
    {
        float current = 0.5f;
        bool revert = false;
        float step = 0.01f;
        while (true)
        {
            yield return new WaitForSeconds(0.05f);
            if (current > 1)
            {
                revert = !revert;
            }
            else if (current < 0.5f)
            {
                revert = !revert;
            }
            if (revert)
            {
                current += step;
            }
            else
            {
                current -= step;
            }
            Color c = Color.HSVToRGB(current, 0.5f, 1f);
            for (int i = 0; i < mRenderList.Length; i++)
            {
                mRenderList[i].material.SetColor("_color", c);
            }
        }
    }
}
