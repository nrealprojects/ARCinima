using UnityEngine;
using System.Collections;
using DG.Tweening;

public class StayInDisplayArea : MonoBehaviour
{
    public float areaWidth = 3;            //锚点区域宽
    public float areaheight = 2;            //锚点区域宽
    public float safetyAreaWidth = 3;         //安全区域宽
    public float safetyAreaheight = 2;         //安全区域宽
    public float displayAreaScale = 1.5f;    //相机视口区域放缩尺度   
    private float displayAreaWidth;         //相机视口区域宽
    private float displayAreaHeigh;          //相机视口区域高
    private float distance = 0;
    private Camera mainCamera;
    private Vector3[] diplayAreaCorners = null;
    private Vector3[] safetyAreaCorners = null;
    private Vector3 displaycornerLT, displaycornerRT, displaycornerLB, displaycornerRB;//视口区域四个角点
    private Vector3[] anchorAreaCorners = null;
    private Vector3 anchorcornerLT, anchorcornerRT, anchorcornerLB, anchorcornerRB; //锚点区域四个角点
    private Vector3 anchorCenter = Vector3.zero;           //锚点区域中心点
    private Vector3 displayCenter = Vector3.zero;           //相机视口中心点
    private float dt, dr, db, dl;               //锚点区域中心点距离视口区域的四条边的距离
    private bool needUpdateTran;        //是否需要更新锚点的transform
    private bool isCameraMove = false;
    public float offsetY = 0.1f;                 //anchor点初始的偏移量
    private bool isMoving = false;

    private void Start()
    {
        mainCamera = Camera.main;
        StartCoroutine(CheckCameraMove());
    }

    void Update()
    {
        CalculateAnchorPoint();
        AdjustAnchorPos();
        UpdateTransform();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        CalculateAnchorPoint();
        Debug.DrawLine(anchorAreaCorners[0], anchorAreaCorners[1], Color.green); // UpperLeft -> UpperRight  
        Debug.DrawLine(anchorAreaCorners[1], anchorAreaCorners[3], Color.green); // UpperRight -> LowerRight  
        Debug.DrawLine(anchorAreaCorners[3], anchorAreaCorners[2], Color.green); // LowerRight -> LowerLeft  
        Debug.DrawLine(anchorAreaCorners[2], anchorAreaCorners[0], Color.green); // LowerLeft -> UpperLeft  

        Debug.DrawLine(safetyAreaCorners[0], safetyAreaCorners[1], Color.yellow); // UpperLeft -> UpperRight  
        Debug.DrawLine(safetyAreaCorners[1], safetyAreaCorners[3], Color.yellow); // UpperRight -> LowerRight  
        Debug.DrawLine(safetyAreaCorners[3], safetyAreaCorners[2], Color.yellow); // LowerRight -> LowerLeft  
        Debug.DrawLine(safetyAreaCorners[2], safetyAreaCorners[0], Color.yellow); // LowerLeft -> UpperLeft  

        Debug.DrawLine(diplayAreaCorners[0], diplayAreaCorners[1], Color.red); // UpperLeft -> UpperRight  
        Debug.DrawLine(diplayAreaCorners[1], diplayAreaCorners[3], Color.red); // UpperRight -> LowerRight  
        Debug.DrawLine(diplayAreaCorners[3], diplayAreaCorners[2], Color.red); // LowerRight -> LowerLeft  
        Debug.DrawLine(diplayAreaCorners[2], diplayAreaCorners[0], Color.red); // LowerLeft -> UpperLeft  
    }
#endif

    private void CalculateAnchorPoint()
    {
        if (diplayAreaCorners == null)
        {
            diplayAreaCorners = new Vector3[4];
            anchorAreaCorners = new Vector3[4];
            safetyAreaCorners = new Vector3[4];

            mainCamera = Camera.main;
            Gizmos.color = Color.yellow;
            anchorCenter = transform.position;
            anchorAreaCorners[0] = new Vector3(anchorCenter.x + areaWidth * 0.5f, anchorCenter.y + areaheight * 0.5f, anchorCenter.z);
            anchorAreaCorners[1] = new Vector3(anchorCenter.x + areaWidth * 0.5f, anchorCenter.y - areaheight * 0.5f, anchorCenter.z);
            anchorAreaCorners[2] = new Vector3(anchorCenter.x - areaWidth * 0.5f, anchorCenter.y + areaheight * 0.5f, anchorCenter.z);
            anchorAreaCorners[3] = new Vector3(anchorCenter.x - areaWidth * 0.5f, anchorCenter.y - areaheight * 0.5f, anchorCenter.z);

            anchorcornerLT = mainCamera.transform.InverseTransformPoint(anchorAreaCorners[0]);
            anchorcornerRT = mainCamera.transform.InverseTransformPoint(anchorAreaCorners[1]);
            anchorcornerLB = mainCamera.transform.InverseTransformPoint(anchorAreaCorners[2]);
            anchorcornerRB = mainCamera.transform.InverseTransformPoint(anchorAreaCorners[3]);

            distance = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);
            Gizmos.color = Color.red;
            float camera_field_degree = mainCamera.fieldOfView * Mathf.PI * 0.5f / 180;
            displayAreaHeigh = 2 * Mathf.Tan(camera_field_degree) * distance * displayAreaScale;
            displayAreaWidth = displayAreaHeigh * mainCamera.aspect;
            Vector3 pos = mainCamera.transform.position + mainCamera.transform.forward * distance;
            diplayAreaCorners[0] = new Vector3(pos.x + displayAreaWidth * 0.5f, pos.y + displayAreaHeigh * 0.5f, pos.z);
            diplayAreaCorners[1] = new Vector3(pos.x + displayAreaWidth * 0.5f, pos.y - displayAreaHeigh * 0.5f, pos.z);
            diplayAreaCorners[2] = new Vector3(pos.x - displayAreaWidth * 0.5f, pos.y + displayAreaHeigh * 0.5f, pos.z);
            diplayAreaCorners[3] = new Vector3(pos.x - displayAreaWidth * 0.5f, pos.y - displayAreaHeigh * 0.5f, pos.z);
            Vector3 horizontal = (diplayAreaCorners[0] - diplayAreaCorners[2]).normalized;
            Vector3 vertical = (diplayAreaCorners[3] - diplayAreaCorners[2]).normalized;
            displayCenter = diplayAreaCorners[2] + (horizontal * displayAreaWidth * 0.5f + vertical * displayAreaHeigh * 0.5f);

            displaycornerRT = mainCamera.transform.InverseTransformPoint(diplayAreaCorners[0]);
            displaycornerRB = mainCamera.transform.InverseTransformPoint(diplayAreaCorners[1]);
            displaycornerLT = mainCamera.transform.InverseTransformPoint(diplayAreaCorners[2]);
            displaycornerLB = mainCamera.transform.InverseTransformPoint(diplayAreaCorners[3]);
        }
        else
        {
            //更新可视区域
            diplayAreaCorners[0] = mainCamera.transform.TransformPoint(displaycornerRT);
            diplayAreaCorners[1] = mainCamera.transform.TransformPoint(displaycornerRB);
            diplayAreaCorners[2] = mainCamera.transform.TransformPoint(displaycornerLT);
            diplayAreaCorners[3] = mainCamera.transform.TransformPoint(displaycornerLB);

            UpdateAnchorPos();
        }
    }

    private void UpdateTransform()
    {
        if (isMoving)
        {
            return;
        }
        transform.position = anchorCenter;
        transform.rotation = mainCamera.transform.rotation;
    }

    private void UpdateAnchorPos()
    {
        needUpdateTran = false;
        dt = DisPoint2Line(anchorCenter, diplayAreaCorners[0], diplayAreaCorners[2]);
        dr = DisPoint2Line(anchorCenter, diplayAreaCorners[0], diplayAreaCorners[1]);
        db = DisPoint2Line(anchorCenter, diplayAreaCorners[1], diplayAreaCorners[3]);
        dl = DisPoint2Line(anchorCenter, diplayAreaCorners[2], diplayAreaCorners[3]);

        if (dt < areaheight * 0.5f)
        {
            dt = areaheight * 0.5f;
            db = displayAreaHeigh - dt;
            needUpdateTran = true;
        }
        if (dr < areaWidth * 0.5)
        {
            dr = areaWidth * 0.5f;
            dl = displayAreaWidth - dr;
            needUpdateTran = true;
        }
        if (db < areaheight * 0.5)
        {
            db = areaheight * 0.5f;
            dt = displayAreaHeigh - db;
            needUpdateTran = true;
        }
        if (dl < areaWidth * 0.5)
        {
            dl = areaWidth * 0.5f;
            dr = displayAreaWidth - dl;
            needUpdateTran = true;
        }

        Vector3 horizontal = (diplayAreaCorners[0] - diplayAreaCorners[2]).normalized;
        Vector3 vertical = (diplayAreaCorners[3] - diplayAreaCorners[2]).normalized;
        Vector3 toward = horizontal * dl + vertical * dt;
        anchorCenter = diplayAreaCorners[2] + toward;
        displayCenter = diplayAreaCorners[2] + (horizontal * displayAreaWidth * 0.5f + vertical * displayAreaHeigh * 0.5f);

        //更新锚点区域
        toward = -horizontal * areaWidth * 0.5f - vertical * areaheight * 0.5f;
        anchorAreaCorners[0] = anchorCenter + toward;
        toward = horizontal * areaWidth * 0.5f - vertical * areaheight * 0.5f;
        anchorAreaCorners[1] = anchorCenter + toward;
        toward = -horizontal * areaWidth * 0.5f + vertical * areaheight * 0.5f;
        anchorAreaCorners[2] = anchorCenter + toward;
        toward = horizontal * areaWidth * 0.5f + vertical * areaheight * 0.5f;
        anchorAreaCorners[3] = anchorCenter + toward;

        //更新安全区域
        toward = -horizontal * safetyAreaWidth * 0.5f - vertical * safetyAreaheight * 0.5f;
        safetyAreaCorners[0] = displayCenter + toward;
        toward = horizontal * safetyAreaWidth * 0.5f - vertical * safetyAreaheight * 0.5f;
        safetyAreaCorners[1] = displayCenter + toward;
        toward = -horizontal * safetyAreaWidth * 0.5f + vertical * safetyAreaheight * 0.5f;
        safetyAreaCorners[2] = displayCenter + toward;
        toward = horizontal * safetyAreaWidth * 0.5f + vertical * safetyAreaheight * 0.5f;
        safetyAreaCorners[3] = displayCenter + toward;
    }

    private bool CheckAnchorInSafetyArea()
    {
        float dt_s, dr_s, db_s, dl_s;
        dl_s = DisPoint2Line(anchorCenter, safetyAreaCorners[0], safetyAreaCorners[2]);
        dt_s = DisPoint2Line(anchorCenter, safetyAreaCorners[0], safetyAreaCorners[1]);
        dr_s = DisPoint2Line(anchorCenter, safetyAreaCorners[1], safetyAreaCorners[3]);
        db_s = DisPoint2Line(anchorCenter, safetyAreaCorners[2], safetyAreaCorners[3]);
        if (dt_s > safetyAreaheight || db_s > safetyAreaheight || dl_s > safetyAreaWidth || dr_s > safetyAreaWidth)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// 根据相机目前是否移动以及锚点是否在安全区域内决定是否需要移动anchor的位置
    /// </summary>
    private void AdjustAnchorPos()
    {
        if (!isCameraMove && !CheckAnchorInSafetyArea())
        {
            MoveAnchorToCenter();
        }
    }

    private void MoveAnchorToCenter()
    {
        if (!isMoving)
        {
            Vector3 vertical = (diplayAreaCorners[3] - diplayAreaCorners[2]).normalized;
            Vector3 tartget = displayCenter + vertical * offsetY;
            isMoving = true;
            transform.DOMove(tartget, 0.4f);
            transform.DORotate(mainCamera.transform.rotation.eulerAngles, 0.4f).OnComplete(() =>
            {
                isMoving = false;
                anchorCenter = tartget;
            });
        }
    }

    /// <summary>
    /// 检测相机在30帧内是否移动
    /// </summary>
    /// <returns></returns>
    private IEnumerator CheckCameraMove()
    {
        int index = 1;
        int check_num = 15;
        Vector3 start, end;
        start = end = Vector3.zero;
        while (true)
        {
            if (index == 1)
            {
                start = displayCenter;
            }
            else if (index == check_num)
            {
                end = displayCenter;
                isCameraMove = (end - start).magnitude > (displayAreaWidth * 0.01f);
            }
            index++;
            if (index > check_num)
            {
                index = 1;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    /// <summary>
    /// 获取一个点到一条线的距离
    /// </summary>
    /// <param name="point">点</param>
    /// <param name="linePoint1">线上某点</param>
    /// <param name="linePoint2">线上某点</param>
    /// <returns></returns>
    public static float DisPoint2Line(Vector3 point, Vector3 linePoint1, Vector3 linePoint2)
    {
        Vector3 vec1 = point - linePoint1;
        Vector3 vec2 = linePoint2 - linePoint1;
        Vector3 vecProj = Vector3.Project(vec1, vec2);
        float dis = Mathf.Sqrt(Mathf.Pow(Vector3.Magnitude(vec1), 2) - Mathf.Pow(Vector3.Magnitude(vecProj), 2));
        return dis;
    }
}