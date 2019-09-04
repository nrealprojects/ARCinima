using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OldMoatGames;
using UnityEngine.UI;
using RenderHeads.Media.AVProVideo;
using NREAL.AR;

public class ScreenHoverPlayer : MonoBehaviour {
    public Text videoNameTxt;
    public Canvas cvs;

    private OneScreen m_CurrentBindPlayer;
    private MediaPlayer m_VideoPlayer;
    private Vector3 m_OriginScale;

    // Use this for initialization
    void Start () {
        GetComponent<MeshRenderer>().enabled = false;
        m_VideoPlayer = GetComponent<MediaPlayer>();
        m_OriginScale = transform.localScale;
    }
	
	// Update is called once per frame
	void Update () {
        UpdateTransform();
    }

    public void Bind(OneScreen mScreenPlayer)
    {
        m_CurrentBindPlayer = mScreenPlayer;
        if(cvs)
            cvs.gameObject.SetActive(true);
        if (videoNameTxt)
            videoNameTxt.text = m_CurrentBindPlayer.Info.videoName;
        UpdateTransform();
        SoundManager.Instance.PlaySoundEffect(SoundClipEnum.ItemHover, m_CurrentBindPlayer.transform.position);
        if (!m_CurrentBindPlayer.IsPhoneController)
        {
            GetComponent<MeshRenderer>().enabled = true;
            PlayHoverVideo();
        }
    }

    private void PlayHoverVideo()
    {
        string hoverPath = "";
        try
        {
            hoverPath = NRealTool.GetNrealResPath() + "/" + m_CurrentBindPlayer.Info.videoPath;
            hoverPath = hoverPath.Replace(@"/Videos/", @"/HoverVideos/").Replace(".mp4", "_hover.mp4");
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.ToString());
            hoverPath = "";
        }
        if (!string.IsNullOrEmpty(hoverPath))
        {
            m_VideoPlayer.OpenVideoFromFile(MediaPlayer.FileLocation.AbsolutePathOrURL, hoverPath);
            m_VideoPlayer.Play();
        }
    }

    private void UpdateTransform()
    {
        if (m_CurrentBindPlayer)
        {
            transform.position = m_CurrentBindPlayer.transform.position - m_CurrentBindPlayer.transform.forward *0.01f;
            transform.rotation = m_CurrentBindPlayer.transform.rotation;
            transform.localScale = m_OriginScale * (m_CurrentBindPlayer.transform.localScale.x / m_OriginScale.x);
        }
    }

    public void UnBind()
    {
        if (m_CurrentBindPlayer && !m_CurrentBindPlayer.IsPhoneController)
        {
            m_VideoPlayer.Pause();
            GetComponent<MeshRenderer>().enabled = false;
        }
        m_CurrentBindPlayer = null;
        if (cvs)
            cvs.gameObject.SetActive(false);
    }

}
