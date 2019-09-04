using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NREAL.AR;
using DG.Tweening;
using NREAL.AR.VideoPlayer;
using NRKernal;

public class OneScreen : MonoBehaviour
{
    public enum ScreenState
    {
        NotReady,
        Default,
        Selected,
        InList
    }

    private ScreenState m_CurScreenState = ScreenState.NotReady;
    private ScreensContainer m_Container;
    private Transform m_ARCam;
    private ARInteractiveItem m_InteractiveItem;

    private Vector3 m_OriginScale;
    private Vector3 m_OriginLocalPos;
    private Quaternion m_OriginRot;
    private Vector3 m_HoverScale; //= Vector3.one * 2.7f;  //hover时模型的大小
    private Vector3 m_InListScale = Vector3.one * 1.5f;
    private float m_HoverTime = 0.5f;  //hover动画时间
    private float m_HoverRadius = 12f;  //hover位置所在球体的半径

    private float m_ListTime = 0.7f; //形成列表的时间
    private float m_PlayDistance = 7f;
    private Vector3 m_PlayScale;
    private float m_MoveTime = 0.6f;
    private Collider m_Collider;
    private MeshRenderer m_Render;

    private bool m_IsHovering = false;
    private bool m_IsBigScreen = false;
    private float m_MinScaleFactor = 0.3f;
    private float m_MaxScaleFactor = 1.7f;
    private float m_Radius = 0.5f;  //这个是实际测得的球半径大小，允许少量误差
    public bool IsPhoneController
    {
        get
        {
            return NRInput.GetControllerType() == ControllerType.CONTROLLER_TYPE_PHONE;
        }
    }

    public ScreenState CurrentScreenState
    {
        get
        {
            return m_CurScreenState;
        }
    }

    public VideoInfo Info { get; private set; }

    void Awake()
    {
        m_Render = GetComponentInChildren<MeshRenderer>(true);
        m_Collider = GetComponent<Collider>();
        m_HoverRadius *= MultiScreenPlayerManager.Instance.GetGlobalScale();
    }

    void Update()
    {
        switch (m_CurScreenState)
        {
            case ScreenState.NotReady:
                SetColliderEnabled(false);
                SetVisible(IsPhoneController || !m_IsHovering);
                break;
            case ScreenState.Default:
                SetColliderEnabled(true);
                SetVisible(IsPhoneController || !m_IsHovering);
                LookCamera();
                SetScaleToDistance();
                break;
            case ScreenState.Selected:
                SetColliderEnabled(false);
                //LookCamera();
                break;
            case ScreenState.InList:
                SetColliderEnabled(true);
                SetVisible(IsPhoneController || !m_IsHovering);
                break;
        }
        //CheckClipScreen();
    }

    private void LookCamera()
    {
        transform.LookAt(transform.position * 2f - m_ARCam.position, m_ARCam.up);
    }

    private void SetVisible(bool isVisible)
    {
        m_Render.enabled = isVisible;
    }

    private void SetColliderEnabled(bool isEnabled)
    {
        m_Collider.enabled = isEnabled;
    }

    //scale = minscale + (maxscale - minsacle) * distance / 2 r
    private void SetScaleToDistance()
    {
        Vector3 containerToCam = m_Container.transform.position - m_ARCam.position;
        //float distance = containerToCam.magnitude;
        Vector3 projectVector = Vector3.Project(transform.position - m_ARCam.position, containerToCam);
        float distance = (containerToCam.magnitude + m_Radius) - projectVector.magnitude;
        distance = Mathf.Clamp(distance, 0f, 2 * m_Radius);
        float scaleFactor = m_MinScaleFactor + (m_MaxScaleFactor - m_MinScaleFactor) * distance * distance / (m_Radius * m_Radius * 4f);
        transform.localScale = m_OriginScale * scaleFactor;
    }

    private void OnHover()
    {
        switch (m_CurScreenState)
        {
            case ScreenState.Default:
                transform.DOKill();
                //Vector3 targetPos = mARCam.position + (transform.position - mARCam.position).normalized * mHoverRadius;
                //transform.DOMove(targetPos, mHoverTime).OnComplete(delegate
                //{
                //    SetVisible(false);
                //    MultiScreenPlayerManager.Instance.HoverPlayer.Bind(this);
                //});
                SetVisible(false);
                m_IsHovering = true;
                MultiScreenPlayerManager.Instance.HoverPlayer.Bind(this);
                //transform.DOScale(mHoverScale, mHoverTime).OnComplete(delegate
                //{
                //    SetVisible(false);
                //    MultiScreenPlayerManager.Instance.HoverPlayer.Bind(this);
                //});
                break;
            case ScreenState.InList:
                SetVisible(false);
                m_IsHovering = true;
                MultiScreenPlayerManager.Instance.HoverPlayer.Bind(this);
                break;
        }
    }

    private void OnOut()
    {
        switch (m_CurScreenState)
        {
            case ScreenState.Default:
                transform.DOKill();
                //Vector3 targetPos = transform.parent.TransformPoint(mOriginLocalPos);
                //transform.DOMove(targetPos, mHoverTime);
                //transform.DOScale(mOriginScale, mHoverTime);
                m_IsHovering = false;
                MultiScreenPlayerManager.Instance.HoverPlayer.UnBind();
                break;
            case ScreenState.InList:
                m_IsHovering = false;
                MultiScreenPlayerManager.Instance.HoverPlayer.UnBind();
                break;
        }
    }

    private void OnClick()
    {
        if (!m_IsHovering)
            return;
        if (m_CurScreenState == ScreenState.Default || m_CurScreenState == ScreenState.InList)
            MultiScreenPlayerManager.Instance.SelectScreen(this);
    }

    public void Init(VideoInfo info, ScreensContainer container)
    {
        m_ARCam = MultiScreenPlayerManager.Instance.CameraTransform;
        m_Container = container;

        m_InteractiveItem = gameObject.AddComponent<ARInteractiveItem>();
        m_InteractiveItem.OnHover += OnHover;
        m_InteractiveItem.OnOut += OnOut;
        m_InteractiveItem.OnClick += OnClick;

        m_OriginScale = transform.localScale;
        m_OriginLocalPos = transform.localPosition;
        m_HoverScale = m_OriginScale * 1.5f;
        this.Info = info;

        //MoveablePhysicsObject moveObj = gameObject.AddComponent<MoveablePhysicsObject>();
        //moveObj.minDistance = 1f;
        //moveObj.maxDistance = 40f;
        //moveObj.distanceFromControllerMin = 5f;
        //moveObj.distanceFromControllerMax = 30f;
        //moveObj.maintainFacing = true;

        m_CurScreenState = ScreenState.Default;
    }

    public void SetThumbTexture(Texture tex)
    {
        m_Render.material.mainTexture = tex;
    }

    public void Reset()
    {
        transform.DOKill();
        transform.position = transform.parent.TransformPoint(m_OriginLocalPos);
        transform.localScale = m_OriginScale;
        SetVisible(true);
        m_IsHovering = false;
        m_IsBigScreen = false;
        m_CurScreenState = ScreenState.Default;
    }

    public void OnSelected()
    {
        m_CurScreenState = ScreenState.Selected;
        MultiScreenPlayerManager.Instance.HoverPlayer.UnBind();
        m_PlayScale = MultiScreenPlayerManager.Instance.GetScreenPageScale() * 6.6f; //视频播放器的模型和onescreen的模型大小不一致，所以有个校准的缩放比
        SetVisible(true);
        transform.DOKill();
        //transform.DOMove(mARCam.position + mARCam.forward * mPlayDistance, mMoveTime)
        transform.DORotate(MultiScreenPlayerManager.Instance.GetTargetPos().eulerAngles, m_MoveTime);
        transform.DOMove(MultiScreenPlayerManager.Instance.GetTargetPos().position, m_MoveTime)
            .OnComplete(delegate
            {
                SetVisible(false);
                m_IsBigScreen = true;
                MultiScreenPlayerManager.Instance.StartPlayVideo(this);
            });
        transform.DOScale(m_PlayScale, m_MoveTime);
    }

    public void OnDeselect()
    {
        SetVisible(true);
        m_IsBigScreen = false;
        m_IsHovering = false;
        transform.DOKill();
        Vector3 targetPos = transform.parent.TransformPoint(m_OriginLocalPos);
        transform.DOMove(targetPos, m_HoverTime);
        transform.DOScale(m_OriginScale, m_HoverTime)
            .OnComplete(delegate
            {
                m_CurScreenState = ScreenState.Default;
            });
    }

    public void OnAddToList(Vector3 pos, Quaternion rot)
    {
        m_CurScreenState = ScreenState.InList;
        m_OriginRot = transform.rotation;
        transform.DOKill();
        transform.DOMove(pos, m_ListTime);
        transform.DORotateQuaternion(rot, m_ListTime);
        transform.DOScale(m_InListScale, m_ListTime);
    }

    public void OnRemoveFromList()
    {
        transform.DOKill();
        Vector3 targetPos = transform.parent.TransformPoint(m_OriginLocalPos);
        transform.DOMove(targetPos, m_ListTime)
            .OnComplete(delegate
            {
                m_CurScreenState = ScreenState.Default;
            });
        transform.DOScale(m_OriginScale, m_ListTime);
        transform.DORotateQuaternion(m_OriginRot, m_ListTime);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public IEnumerator LoadThumnail()
    {
        if (Info == null || string.IsNullOrEmpty(Info.thumbnail))
            yield break;
        string path = NRealTool.GetNrealResPath() + "/" + Info.thumbnail + ".jpg";
        WWW www = new WWW(path);
        yield return www;
        if (www.isDone && string.IsNullOrEmpty(www.error))
        {
            m_Render.material.mainTexture = www.texture;
            www.Dispose();
            www = null;
        }
        else
        {
            Debug.LogError(www.error);
        }
    }

    //public float DistancePointToLine(Vector3 point, Vector3 linePoint1, Vector3 linePoint2)
    //{
    //    float fProj = Vector3.Dot(point - linePoint1, (linePoint1 - linePoint2).normalized);
    //    return Mathf.Sqrt((point - linePoint1).sqrMagnitude - fProj * fProj);
    //}

    ////靠近摄像机的半球保留，另外的半球隐藏
    //private bool ShouldClip()
    //{
    //    if (m_CurScreenState == ScreenState.Selected && !m_IsBigScreen)
    //        return false;
    //    Plane plane = new Plane(m_Container.transform.position - m_ARCam.position, m_ARCam.position);
    //    float distance = plane.GetDistanceToPoint(transform.position);
    //    return (distance > m_Container.DistanceFromCamera);
    //}

    //private void CheckClipScreen()
    //{
    //    if (ShouldClip())
    //    {
    //        m_Collider.enabled = false;
    //        m_Render.enabled = false;
    //    }
    //    else
    //    {
    //        m_Collider.enabled = true;
    //        m_Render.enabled = true;
    //    }
    //}
}
