using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DrawCurveTest : MonoBehaviour
{
    public Material lineMat;
    public int segment = 20;
    public Transform[] poss = new Transform[3];
    List<Vector3> m_points3 = new List<Vector3>();
    public LineRenderer lineRenderer;
    public Color c1 = Color.yellow;
    public Color c2 = Color.red;

    void Start()
    {
        m_points3.AddRange(BezierCurveTool.CreateCurve(poss[0].position, poss[1].position, poss[2].position, poss[2].position, segment));
        CreateLine();
       transform.DOPath(m_points3.ToArray(), 3f, PathType.Linear);
    }

    void CreateLine()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.material = lineMat;
        lineRenderer.SetColors(c1, c2);
        lineRenderer.SetWidth(0.5F, 0.5F);
        lineRenderer.positionCount = segment;
        int i = 0;
        while (i < m_points3.Count)
        {
            lineRenderer.SetPosition(i, m_points3[i]);
            i++;
        }
    }
}