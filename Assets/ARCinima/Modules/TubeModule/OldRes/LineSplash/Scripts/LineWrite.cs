using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineWrite : MonoBehaviour
{
    public Transform m_Quard;
    public LineRenderer m_LineRender1;
    public LineRenderer m_LineRender2;
    private Vector3 pointLT;
    private Vector3 pointLB;
    private Vector3 pointRT;
    private Vector3 pointRB;

    //public Transform headPoint;
    //public Transform middlePoint;
    //public Transform tailPoint;
    public bool isShrink = false;

    private float minX, maxX, minY, maxY;
    private Vector3 head, middle, tail;
    private Vector3[] points1 = new Vector3[2];
    private Vector3[] points2 = new Vector3[2];
    public float step = 0.15f;
    private float lineLength = 3f;
    private bool revert = true;
    public static float lineWidth = 0.06f;

    private enum PointLocation
    {
        top,
        leftTop,
        left,
        leftbottom,
        bottom,
        rightbottom,
        right,
        rightTop
    }

    public enum StartPoint
    {
        LeftTop,
        LeftBottom,
        RightBottom,
        RightTop
    }
    public StartPoint startPoint = StartPoint.LeftTop;

    // Use this for initialization
    void Start()
    {
        this.Init();
    }

    private void Init()
    {
        Vector3 posbottomLeft = new Vector3(-0.5f, -0.5f, 0);
        Vector3 posbottomright = new Vector3(0.5f, -0.5f, 0);
        Vector3 postopright = new Vector3(0.5f, 0.5f, 0);
        Vector3 postopleft = new Vector3(-0.5f, 0.5f, 0);

        pointLB = m_Quard.TransformPoint(posbottomLeft);
        pointRB = m_Quard.TransformPoint(posbottomright);
        pointRT = m_Quard.TransformPoint(postopright);
        pointLT = m_Quard.TransformPoint(postopleft);

        minX = pointLB.x;
        maxX = pointRB.x;
        minY = pointLB.y;
        maxY = pointLT.y;

        //init start point
        switch (startPoint)
        {
            case StartPoint.LeftTop:
                head = pointLT;
                tail = new Vector3(head.x + lineLength, pointLT.y, pointLT.z); ;
                middle = 0.5f * (head + tail);
                break;
            case StartPoint.LeftBottom:
                head = pointLB;
                tail = new Vector3(pointLB.x, pointLT.y + lineLength, pointLB.z); ;
                middle = 0.5f * (head + tail);
                break;
            case StartPoint.RightBottom:
                head = pointRB;
                tail = new Vector3(pointRB.x - lineLength, pointRB.y, pointRB.z); ;
                middle = 0.5f * (head + tail);
                break;
            case StartPoint.RightTop:
                head = pointRT;
                tail = new Vector3(pointRT.x, pointRT.y - lineLength, pointRT.z); ;
                middle = 0.5f * (head + tail);
                break;
            default:
                break;
        }
        StartCoroutine(PlaySplashEffect());
    }

    private IEnumerator PlaySplashEffect()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            WriteLine();
        }
    }

    private void WriteLine()
    {
        UpdateLinePoint();
        if (revert)
        {
            points1[0] = tail;
            points1[1] = middle;
            points2[0] = middle;
            points2[1] = head;
        }
        else
        {
            points1[0] = tail;
            points1[1] = middle;
            points2[0] = middle;
            points2[1] = head;
        }
        m_LineRender1.positionCount= points1.Length;
        m_LineRender1.SetPositions(points1);
        m_LineRender1.SetWidth(lineWidth * (isShrink ? 0.3f : 1f), lineWidth);

        m_LineRender2.positionCount = points2.Length;
        m_LineRender2.SetPositions(points2);
        m_LineRender2.SetWidth(lineWidth, lineWidth);

        //headPoint.position = head;
        //middlePoint.position = middle;
        //tailPoint.position = tail;
    }

    private void UpdateLinePoint()
    {
        //Move head
        head = MovePoint(head);
        //Move tail
        tail = MovePoint(tail);
        //get middle point
        middle = GetMiddle(head, tail);
    }

    private Vector3 MovePoint(Vector3 point)
    {
        Vector3 temp;
        PointLocation pl = GetPointLocation(point);
        switch (pl)
        {
            case PointLocation.top:
                if (revert)
                {
                    if (MoveLeft(point, out temp))
                    {
                        return temp;
                    }
                    else
                    {
                        return pointLT;
                    }
                }
                else
                {
                    if (MoveRight(point, out temp))
                    {
                        return temp;
                    }
                    else
                    {
                        return pointRT;
                    }
                }
            case PointLocation.leftTop:
                if (revert)
                {
                    if (MoveDown(point, out temp))
                    {
                        return temp;
                    }
                    else
                    {
                        return pointLB;
                    }
                }
                else
                {
                    if (MoveRight(point, out temp))
                    {
                        return temp;
                    }
                    else
                    {
                        return pointRT;
                    }
                }
            case PointLocation.left:
                if (revert)
                {
                    if (MoveDown(point, out temp))
                    {
                        return temp;
                    }
                    else
                    {
                        return pointLB;
                    }
                }
                else
                {
                    if (MoveUp(point, out temp))
                    {
                        return temp;
                    }
                    else
                    {
                        return pointLT;
                    }
                }
            case PointLocation.leftbottom:
                if (revert)
                {
                    if (MoveRight(point, out temp))
                    {
                        return temp;
                    }
                    else
                    {
                        return pointRB;
                    }
                }
                else
                {
                    if (MoveUp(point, out temp))
                    {
                        return temp;
                    }
                    else
                    {
                        return pointLT;
                    }
                }
            case PointLocation.bottom:
                if (revert)
                {
                    if (MoveRight(point, out temp))
                    {
                        return temp;
                    }
                    else
                    {
                        return pointRB;
                    }
                }
                else
                {
                    if (MoveLeft(point, out temp))
                    {
                        return temp;
                    }
                    else
                    {
                        return pointLB;
                    }
                }
            case PointLocation.rightbottom:
                if (revert)
                {
                    if (MoveUp(point, out temp))
                    {
                        return temp;
                    }
                    else
                    {
                        return pointRT;
                    }
                }
                else
                {
                    if (MoveLeft(point, out temp))
                    {
                        return temp;
                    }
                    else
                    {
                        return pointLB;
                    }
                }
            case PointLocation.right:
                if (revert)
                {
                    if (MoveUp(point, out temp))
                    {
                        return temp;
                    }
                    else
                    {
                        return pointRT;
                    }
                }
                else
                {
                    if (MoveDown(point, out temp))
                    {
                        return temp;
                    }
                    else
                    {
                        return pointRB;
                    }
                }
            case PointLocation.rightTop:
                if (revert)
                {
                    if (MoveLeft(point, out temp))
                    {
                        return temp;
                    }
                    else
                    {
                        return pointLT;
                    }
                }
                else
                {
                    if (MoveDown(point, out temp))
                    {
                        return temp;
                    }
                    else
                    {
                        return pointRB;
                    }
                }
            default:
                return Vector3.zero;
        }
    }

    private Vector3 GetMiddle(Vector3 h, Vector3 t)
    {
        Vector3 pos = Vector3.zero;
        PointLocation plh = GetPointLocation(h);
        PointLocation plt = GetPointLocation(t);
        if (Mathf.Abs((int)plh - (int)plt) == 0 || Mathf.Abs((int)plh - (int)plt) == 1 || Mathf.Abs((int)plh - (int)plt) == 7)
        {
            return (h + t) * 0.5f;
        }
        else if ((plh == PointLocation.leftTop && plt == PointLocation.rightTop)
            || (plh == PointLocation.rightTop && plt == PointLocation.leftTop)
            || (plh == PointLocation.leftTop && plt == PointLocation.leftbottom)
            || (plh == PointLocation.leftbottom && plt == PointLocation.leftTop)
            || (plh == PointLocation.leftbottom && plt == PointLocation.rightbottom)
            || (plh == PointLocation.rightbottom && plt == PointLocation.leftbottom)
            || (plh == PointLocation.rightbottom && plt == PointLocation.rightTop)
            || (plh == PointLocation.rightTop && plt == PointLocation.rightbottom))
        {
            return (h + t) * 0.5f;
        }
        else
        {
            if (revert)
            {
                if (plh == PointLocation.left)
                {
                    return new Vector3(pointLT.x, pointLT.y + 0.05f, pointLT.z);
                }
                if (plh == PointLocation.bottom)
                {
                    return pointLB;
                }
                if (plh == PointLocation.right)
                {
                    return pointRB;
                }
                if (plh == PointLocation.top)
                {
                    return pointRT;
                }
            }
            else
            {
                if (plh == PointLocation.left)
                {
                    return pointLB;
                }
                if (plh == PointLocation.bottom)
                {
                    return pointRB;
                }
                if (plh == PointLocation.right)
                {
                    return pointRT;
                }
                if (plh == PointLocation.top)
                {
                    return pointLT;
                }
            }
        }
        return h;
    }

    private PointLocation GetPointLocation(Vector3 point)
    {
        PointLocation pl = PointLocation.top;
        if (Mathf.Abs(point.y - maxY) < float.Epsilon)
        {
            if (Mathf.Abs(point.x - pointLT.x) < float.Epsilon)
            {
                return PointLocation.leftTop;
            }
            if (Mathf.Abs(point.x - pointRT.x) < float.Epsilon)
            {
                return PointLocation.rightTop;
            }
            return PointLocation.top;
        }
        if (Mathf.Abs(point.y - minY) < float.Epsilon)
        {
            if (Mathf.Abs(point.x - pointLB.x) < float.Epsilon)
            {
                return PointLocation.leftbottom;
            }
            if (Mathf.Abs(point.x - pointRB.x) < float.Epsilon)
            {
                return PointLocation.rightbottom;
            }
            return PointLocation.bottom;
        }
        if (Mathf.Abs(point.x - minX) < float.Epsilon)
        {
            if (Mathf.Abs(point.y - pointLT.y) < float.Epsilon)
            {
                return PointLocation.leftTop;
            }
            if (Mathf.Abs(point.y - pointLB.y) < float.Epsilon)
            {
                return PointLocation.leftbottom;
            }
            return PointLocation.left;
        }
        if (Mathf.Abs(point.x - maxX) < float.Epsilon)
        {
            if (Mathf.Abs(point.y - pointRT.y) < float.Epsilon)
            {
                return PointLocation.rightTop;
            }
            if (Mathf.Abs(point.y - pointRB.y) < float.Epsilon)
            {
                return PointLocation.rightbottom;
            }
            return PointLocation.right;
        }
        return pl;
    }

    private bool MoveLeft(Vector3 pos, out Vector3 tem, float dlta = 0)
    {
        if (dlta < float.Epsilon)
        {
            dlta = step;
        }
        if (pos.x - dlta < minX)
        {
            if (revert)
            {
                MoveDown(pointLT, out tem, step - (pos.x - minX));
                return true;
            }
            else
            {
                MoveUp(pointLB, out tem, step - (pos.x - minX));
                return true;
            }
        }
        else
        {
            tem = new Vector3(pos.x - dlta, pos.y, pos.z);
            return true;
        }
    }

    private bool MoveRight(Vector3 pos, out Vector3 tem, float dlta = 0)
    {
        if (dlta < float.Epsilon)
        {
            dlta = step;
        }
        if (pos.x + dlta > maxX)
        {
            if (revert)
            {
                MoveUp(pointRB, out tem, step - (maxX - pos.x));
                return true;
            }
            else
            {
                MoveDown(pointRT, out tem, step - (maxX - pos.x));
                return true;
            }
        }
        else
        {
            tem = new Vector3(pos.x + dlta, pos.y, pos.z);
            return true;
        }
    }

    private bool MoveUp(Vector3 pos, out Vector3 tem, float dlta = 0)
    {
        if (dlta < float.Epsilon)
        {
            dlta = step;
        }
        if (pos.y + dlta > maxY)
        {
            if (revert)
            {
                MoveLeft(pointRT, out tem, step - (maxY - pos.y));
                return true;
            }
            else
            {
                MoveRight(pointLT, out tem, step - (maxY - pos.y));
                return true;
            }
        }
        else
        {
            tem = new Vector3(pos.x, pos.y + dlta, pos.z);
            return true;
        }
    }

    private bool MoveDown(Vector3 pos, out Vector3 tem, float dlta = 0)
    {
        if (dlta <= float.Epsilon)
        {
            dlta = step;
        }
        if (pos.y - dlta < minY)
        {
            if (revert)
            {
                MoveRight(pointLB, out tem, step - (pos.y - minY));
                return true;
            }
            else
            {
                MoveLeft(pointRB, out tem, step - (pos.y - minY));
                return true;
            }
        }
        else
        {
            tem = new Vector3(pos.x, pos.y - dlta, pos.z);
            return true;
        }
    }
}