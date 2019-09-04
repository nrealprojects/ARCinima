using UnityEngine;
using System.Collections;

[System.Reflection.Obfuscation(Exclude = true)]
public class DebugScreen : MonoBehaviour {

	GUIStyle mFpsFontStyle = null;

	// Use this for initialization
	void Start () 
	{
		mFpsFontStyle =new GUIStyle();
		mFpsFontStyle.normal.background = null;    //这是设置背景填充的
		mFpsFontStyle.normal.textColor=new Color(1,0,0);   //设置字体颜色的
		mFpsFontStyle.fontSize = 80;       //当然，这是字体大小
	}

	// Update is called once per frame
	void Update () 
	{
		UpdateTick();
	}

	void OnGUI()
	{
		DrawFps();
	}

	private void DrawFps()
	{
		if (mLastFps > 50)
		{
			mFpsFontStyle.normal.textColor=new Color(0,1,0);   //设置字体颜色的
		}
		else if (mLastFps > 40)
		{
			mFpsFontStyle.normal.textColor=new Color(1, 1, 0);   //设置字体颜色的
		}
		else
		{
			mFpsFontStyle.normal.textColor=new Color(1.0f, 0, 0);   //设置字体颜色的
		}

		GUI.Label(new Rect(50, 32, 128, 128), "fps: " + mLastFps, mFpsFontStyle);
	}

	private long mFrameCount = 0;
	private long mLastFrameTime = 0;
	static long mLastFps = 0;
	private void UpdateTick()
	{
		if (true)
		{
			mFrameCount++;
			long nCurTime = TickToMilliSec(System.DateTime.Now.Ticks);
			if (mLastFrameTime == 0)
			{
				mLastFrameTime = TickToMilliSec(System.DateTime.Now.Ticks);
			}

			if ((nCurTime - mLastFrameTime) >= 1000)
			{
				long fps = (long)(mFrameCount * 1.0f / ((nCurTime - mLastFrameTime) / 1000.0f));

				mLastFps = fps;

				mFrameCount = 0;

				mLastFrameTime = nCurTime;
			}
		}
	}

	public static long TickToMilliSec(long tick)
	{
		return tick / (10 * 1000);
	}
}
