using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NREAL.AR;
using NREAL.AR.VideoPlayer;
using NRToolkit.Sharing.Tools;
using NRKernal;

//视频列表的枚举类
public class VideoListType
{
    public int TypeID { get; private set; }
    public string Name { get; private set; }

    public VideoListType(int TypeID, string Name)
    {
        this.TypeID = TypeID;
        this.Name = Name;

        if (typeDict == null)
            typeDict = new Dictionary<int, VideoListType>();
        if (!typeDict.ContainsKey(TypeID))
            typeDict.Add(TypeID, this);
    }

    public static readonly VideoListType Favourate = new VideoListType(101, "猜你喜欢");
    public static readonly VideoListType Recommand = new VideoListType(102, "推荐");
    public static readonly VideoListType TV = new VideoListType(103, "电视剧");
    public static readonly VideoListType Movie = new VideoListType(104, "电影");

    public static Dictionary<int, VideoListType> typeDict;

    public static VideoListType GetListTypeByID(int id)
    {
        VideoListType listType;
        if (typeDict.TryGetValue(id, out listType))
        {
            return listType;
        }
        else
        {
            Debug.LogError("不存在这个列表类型：" + id);
            return null;
        }
    }
}


public class MultiScreenListInfo
{
    public VideoListType listType;
    public List<int> videoIDList;
    private int maxNum; //暂定一个数目上限

    public MultiScreenListInfo(VideoListType listType)
    {
        this.listType = listType;
        int maxNum = Random.Range(12, 21);
        videoIDList = new List<int>();
        //随机几个视频ID加入列表
        for (int i = 0; i < maxNum; i++)
        {
            int id = Random.Range(0, 500);
            if (!videoIDList.Contains(id))
                videoIDList.Add(id);
        }
    }
}

public class MultiScreenPlayerManager : NREAL.AR.Singleton<MultiScreenPlayerManager>
{
    [SerializeField]
    private ScreensContainer m_Container;
    [SerializeField]
    private VideoScreenPage m_ScreenPlayer;
    [SerializeField]
    private ScreenHoverPlayer m_HoverPlayer;
    [SerializeField]
    private WitMenuHelper m_WitMenuHelper;
    [SerializeField]
    private Transform m_TargetPos;
    [SerializeField]
    private Transform m_MiniVideoTarget;
    [SerializeField]
    private ScreensCenterEffect m_CenterEffect;

    private Dictionary<string, Texture> m_ThumbTexDict;
    private OneScreen m_CurrentScreen;
    private OneScreen[] m_ScreenArr;
    private Vector3 m_OriginScreenPlayerScale;
    private Dictionary<int, OneScreen> m_ScreenDict;
    private static VideoConfig videoConfig = null;

    private MultiScreenListInfo[] m_ListInfoArr;
    private MultiScreenListInfo m_CurrentListInfo = null;
    private List<OneScreen> m_CurrentScreenList = new List<OneScreen>();

    const float Row_Space = 0.94f;
    const float Col_Space = 1.6f;
    const int rowNum = 4;
    const int colNum = 5;

    public bool IsShowingAll { get; private set; }

    private Transform m_ARCamera;
    public Transform CameraTransform
    {
        get
        {
            if (m_ARCamera == null)
                m_ARCamera = Camera.main.transform.parent;
            return m_ARCamera;
        }
    }

    public ScreenHoverPlayer HoverPlayer
    {
        get
        {
            return m_HoverPlayer;
        }
    }

    public Transform GetTargetPos()
    {
        return m_TargetPos;
    }

    public Vector3 GetScreenPageScale()
    {
        return m_ScreenPlayer.transform.localScale;
    }

    void Start()
    {
        StartCoroutine(Init());
    }

    private void Update()
    {
        if (NRInput.GetButtonDown(ControllerButton.HOME))
        {
            OnCancel();
        }
    }

    protected override void OnDestroy()
    {
        StopAllCoroutines();
        base.OnDestroy();
    }

    public void ShowScreenList(int index)
    {
        if (m_CurrentListInfo != null)
            return;
        if (m_Container.IsRotating)
            return;
        if (index < 0 || index > m_ListInfoArr.Length - 1)
            return;
        m_CurrentListInfo = m_ListInfoArr[index];
        m_Container.Lock();
        m_CurrentScreenList.Clear();
        Vector3 centerPos = CameraTransform.forward * 10f;

        for (int i = 0; i < m_CurrentListInfo.videoIDList.Count; i++)
        {
            int videoID = m_CurrentListInfo.videoIDList[i];
            if (m_ScreenDict.ContainsKey(videoID))
            {
                m_CurrentScreenList.Add(m_ScreenDict[videoID]);
                m_ScreenDict[videoID].OnAddToList(GetScreenPositionInWorld(centerPos, i), GetScreenRotationInWorld());
            }
        }

        for (int i = 0; i < m_ScreenArr.Length; i++)
        {
            if (m_ScreenArr[i].CurrentScreenState != OneScreen.ScreenState.InList)
                m_ScreenArr[i].Hide();
        }

        if (m_WitMenuHelper)
            m_WitMenuHelper.Hide();
        if (m_CenterEffect)
            m_CenterEffect.Hide();
    }

    public void HideScreenList()
    {
        for (int i = 0; i < m_CurrentScreenList.Count; i++)
        {
            m_CurrentScreenList[i].OnRemoveFromList();
        }
        m_CurrentScreenList.Clear();
        m_CurrentListInfo = null;

        for (int i = 0; i < m_ScreenArr.Length; i++)
        {
            m_ScreenArr[i].Show();
        }
        m_Container.Unlock();
        if (m_WitMenuHelper)
            m_WitMenuHelper.Show();
        if (m_CenterEffect)
            m_CenterEffect.Show();
    }

    private Vector3 GetScreenPositionInWorld(Vector3 centerPos, int index)
    {
        //假设是4排5列
        int rowIndex = index / colNum;
        int colIndex = index % colNum;
        Vector3 resultPos = centerPos
            + CameraTransform.right * Col_Space * (colIndex - (colNum - 1) / 2f)
            - CameraTransform.up * Row_Space * (rowIndex - (rowNum - 1) / 2f);
        return resultPos;
    }

    private Quaternion GetScreenRotationInWorld()
    {
        return CameraTransform.rotation;
    }

    public void OnCancel()
    {
        if (m_WitMenuHelper)
            m_WitMenuHelper.HideMenu();
        if (m_CurrentScreen)
            DeselectScreen();
        if (m_CurrentListInfo != null)
            HideScreenList();
    }

    IEnumerator Init()
    {
        m_Container.Init(CameraTransform);
        m_OriginScreenPlayerScale = m_ScreenPlayer.transform.localScale;
        m_ScreenPlayer.SetMiniSizeTarget(m_MiniVideoTarget);
        yield return StartCoroutine(LoadVideoList());
        yield return StartCoroutine(PreLoadThumbTexture());
        Transform ctf = m_Container.transform;
        m_ScreenArr = new OneScreen[ctf.childCount];
        m_ScreenDict = new Dictionary<int, OneScreen>();
        for (int i = 0; i < ctf.childCount; i++)
        {
            ChangeScale(ctf.GetChild(i));
            m_ScreenArr[i] = ctf.GetChild(i).gameObject.AddComponent<OneScreen>();
            VideoInfo info = GetRandomInfo(i);
            m_ScreenArr[i].Init(info, m_Container);
            if (m_ThumbTexDict.ContainsKey(info.thumbnail))
                m_ScreenArr[i].SetThumbTexture(m_ThumbTexDict[info.thumbnail]);
            m_ScreenDict.Add(m_ScreenArr[i].Info.videoID, m_ScreenArr[i]);
        }

        CreateRandomListInfo();
        m_ScreenPlayer.transform.parent = null;
        ////初始化缩略图
        //for (int i = 0; i < m_ScreenArr.Length; i++)
        //{
        //    yield return m_ScreenArr[i].LoadThumnail();
        //}

        ShowAll();
    }

    IEnumerator PreLoadThumbTexture()
    {
        var temp = transform.localScale;
        transform.localScale = Vector3.zero;
        if (videoConfig != null && videoConfig.videos != null && videoConfig.videos.Count > 0)
        {
            m_ThumbTexDict = new Dictionary<string, Texture>();
            for (int i = 0; i < videoConfig.videos.Count; i++)
            {
                if (string.IsNullOrEmpty(videoConfig.videos[i].thumbnail))
                {
                    Debug.LogError("thumbnail is null for video: " + videoConfig.videos[i].videoName);
                    continue;
                }
                yield return StartCoroutine(LoadThumbTexByIndex(i));
            }
            yield return new WaitForSeconds(0.2f);
            transform.localScale = temp;
        }
        else
        {
            yield break;
        }
    }

    IEnumerator LoadThumbTexByIndex(int index)
    {
        string path = NRealTool.GetNrealResPath() + "/" + videoConfig.videos[index].thumbnail + ".jpg";
        WWW www = new WWW(path);
        yield return www;
        if (www.isDone && string.IsNullOrEmpty(www.error))
        {
            m_ThumbTexDict.Add(videoConfig.videos[index].thumbnail, www.texture);
            www.Dispose();
            www = null;
        }
        else
        {
            Debug.LogError(www.error);
        }
    }

    private void CreateRandomListInfo()
    {
        m_ListInfoArr = new MultiScreenListInfo[4];
        m_ListInfoArr[0] = new MultiScreenListInfo(VideoListType.Favourate);
        m_ListInfoArr[1] = new MultiScreenListInfo(VideoListType.Recommand);
        m_ListInfoArr[2] = new MultiScreenListInfo(VideoListType.TV);
        m_ListInfoArr[3] = new MultiScreenListInfo(VideoListType.Movie);
    }

    public void ShowAll()
    {
        IsShowingAll = true;
        if (m_WitMenuHelper)
            m_WitMenuHelper.Show();
        if (m_CenterEffect)
            m_CenterEffect.Show();
        m_Container.Unlock();
        for (int i = 0; i < m_ScreenArr.Length; i++)
        {
            if (m_ScreenArr[i] != m_CurrentScreen)
                m_ScreenArr[i].Show();
        }
    }

    public void HideAll()
    {
        IsShowingAll = false;
        if (m_WitMenuHelper)
            m_WitMenuHelper.Hide();
        if (m_CenterEffect)
            m_CenterEffect.Hide();
        m_Container.Lock();
        for (int i = 0; i < m_ScreenArr.Length; i++)
        {
            if (m_ScreenArr[i] != m_CurrentScreen)
                m_ScreenArr[i].Hide();
        }
    }

    public void SelectScreen(OneScreen screenPlayer)
    {
        m_CurrentScreen = screenPlayer;
        m_CurrentScreen.OnSelected();
        HideAll();
    }

    public void StartPlayVideo(OneScreen oneScreen)
    {
        if (videoConfig == null)
            return;
        m_ScreenPlayer.transform.position = oneScreen.transform.position;
        m_ScreenPlayer.transform.rotation = oneScreen.transform.rotation;
        m_ScreenPlayer.gameObject.SetActive(true);
        m_ScreenPlayer.PlayNormalVideo(oneScreen.Info);
    }

    public void DeselectScreen()
    {
        if (m_CurrentScreen)
        {
            ShowAll();
            m_CurrentScreen.transform.position = m_ScreenPlayer.transform.position;
            m_CurrentScreen.OnDeselect();
            m_CurrentScreen = null;

            m_ScreenPlayer.DisableFollow();
            m_ScreenPlayer.Stop();
            m_ScreenPlayer.ResetState();
            m_ScreenPlayer.gameObject.SetActive(false);
            m_ScreenPlayer.transform.localScale = m_OriginScreenPlayerScale;
        }
    }

    public VideoInfo GetRandomInfo(int index)
    {
        VideoInfo videoInfo = videoConfig.videos[Random.Range(0, videoConfig.videos.Count)].Copy();
        //videoInfo.thumbnail = "VideoList/MultiScreen/Thumbs/" + Random.Range(0, 10);
        videoInfo.videoID = index;
        return videoInfo;
    }

    private IEnumerator LoadVideoList()
    {
        string path = NRealTool.GetNrealResPath() + "/MultiScreenVideoList/MultiScreenVideoConfig.xml";
        WWW www = new WWW(path);
        yield return www;
        if (www.isDone && string.IsNullOrEmpty(www.error))
        {
            videoConfig = XmlSerializeHelper.DeSerialize<VideoConfig>(www.text);
            www.Dispose();
            www = null;
        }
        else
        {
            Debug.LogError(www.error);
        }
    }

    public void Close()
    {
        if (m_CurrentScreen)
        {
            m_CurrentScreen.Reset();
            m_CurrentScreen = null;
        }
        m_ScreenPlayer.DisableFollow();
        m_ScreenPlayer.Stop();
        m_ScreenPlayer.gameObject.SetActive(false);
        m_ScreenPlayer.transform.localScale = m_OriginScreenPlayerScale;
        ShowAll();
        EventCenter.Instance.DispatchEvent(TubeNoti.CLOSE);
    }

    private float childScreenScale = 0.4f;
    private void ChangeScale(Transform tf)
    {
        tf.localPosition *= childScreenScale;
        tf.localScale *= childScreenScale;
    }

    public float GetGlobalScale()
    {
        return childScreenScale;
    }

    public void OnShow()
    {
        if (m_ScreenPlayer.gameObject.activeSelf)
            m_ScreenPlayer.ChangeVideoPageState(VideoScreenPage.VideoPageState.Normal);
    }

    public void OnHide()
    {
        if (m_ScreenPlayer.gameObject.activeSelf)
            m_ScreenPlayer.ChangeVideoPageState(VideoScreenPage.VideoPageState.MiniSize);
    }
}
