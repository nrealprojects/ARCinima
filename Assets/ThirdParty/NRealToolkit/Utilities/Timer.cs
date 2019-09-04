using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Timer.
/// Author: Xingguang Yu
/// </summary>
[ExecuteInEditMode]
public class Timer:MonoBehaviour
{
	static Timer instance;

	static Timer Instance
	{
		get
		{
			if (instance == null)
			{
				instance = MonoDelegate.GetHiddenNonDestroyGameObject().AddComponent<Timer>();
			}
			return instance;
		}
	}

	public static void OutterUpdate(float deltaTime)
	{
		instance.ManuallyUpdate(deltaTime);
	}

	public static void AddCallback(Action callback, float periodInSecond, bool callOnce = true)
	{
		Instance._AddCallback(callback, periodInSecond, callOnce);
	}

	public static void Pause(Action callback)
	{

	}

	public static void Resume(Action callback)
	{

	}
	
	List<TimerInfo> timers = new List<TimerInfo>();
	List<TimerInfo> timersShouldBeRemoved = new List<TimerInfo>();
	
	public static void RemoveCallback(Action callback)
	{
		Instance._RemoveCallback(callback);
	}

//	public static void RemoveAllCallbackForTarget(object target)
//	{
//		Instance._RemoveAllCallbackForTarget (target);
//	}
	
	void _AddCallback(Action callback, float period, bool callOnce)
	{
		TimerInfo timerInfo = new TimerInfo();
		
		timerInfo.callback = callback;
		timerInfo.period = period;
		timerInfo.timeElapsed = 0.0f;
		timerInfo.callOnce = callOnce;
		
		timers.Add(timerInfo);
	}
	
	void _RemoveCallback(Action callback)
	{
		List<TimerInfo> timersToRemove = new List<TimerInfo>();
		for (int i = 0; i < timers.Count; i++)
		{
			if (timers [i].callback == callback)
			{
				timersToRemove.Add(timers [i]);
			}
		}
		for (int i = 0; i < timersToRemove.Count; i++)
		{
			timers.Remove(timersToRemove [i]);
		}
		timersToRemove.Clear();
		
	}

//	void _RemoveAllCallbackForTarget(object target)
//	{
//		List<TimerInfo> timersToRemove = new List<TimerInfo>();
//		for (int i = 0; i < timers.Count; i++)
//		{
//			if (timers [i].callback.Target == target)
//			{
//				timersToRemove.Add(timers [i]);
//			}
//		}
//		for (int i = 0; i < timersToRemove.Count; i++)
//		{
//			timers.Remove(timersToRemove [i]);
//		}
//		timersToRemove.Clear();
//	}

	TimerInfo _GetTimer(Action callback)
	{
		for (int i = 0; i < timers.Count; i++)
		{
			if (timers [i].callback == callback)
			{
				return timers [i];
			}
		}

		return null;
	}

	void _Pause(Action callback)
	{
		TimerInfo timerInfo = _GetTimer(callback);
		if (timerInfo == null)
		{
			return;
		}
		timerInfo.pause = true;
	}

	void _Resume(Action callback)
	{
		TimerInfo timerInfo = _GetTimer(callback);
		if (timerInfo == null)
		{
			return;
		}
		timerInfo.pause = false;
	}

	public void ManuallyUpdate(float deltaTime)
	{
		UpdateTime(deltaTime);
	}


	void Update()
	{
		UpdateTime(Time.deltaTime);
	}

	void UpdateTime(float deltaTime)
	{
		if (deltaTime <= 0f)
		{
			return;
		}

		for (int i = 0; i < timers.Count; i++)
		{
			TimerInfo timer = timers [i];
			if (timer.pause)
			{
				continue;
			}
			//Elapse time
			timer.timeElapsed += deltaTime;
			
			bool objThere = true;
			bool isExecuted = false;
			
			//If time is up
			if (timer.timeElapsed >= timer.period)
			{
				Action callback = timer.callback;
				try
				{
					callback();
				} catch (Exception e)
				{
					Debug.LogException(e);
				}
				isExecuted = true;
			}
			if (isExecuted)
			{
				//If it is executed and should only execute once, collects and removes it.
				if (timer.callOnce)
				{
					timersShouldBeRemoved.Add(timer);
				} else
				{
					timer.timeElapsed = 0.0f;
				}
			} else if (!objThere)
			{
				timersShouldBeRemoved.Add(timer);
			}
			
			
		}
		
		//Removes timers should be removed
		for (int i = 0; i < timersShouldBeRemoved.Count; i++)
		{
			timers.Remove(timersShouldBeRemoved [i]);
		}
		
		timersShouldBeRemoved.Clear();
		
	}
	
	
	
	
	
	class TimerInfo
	{
		public Action callback;
		public float period = 0.0f;
		public float timeElapsed = 0.0f;
		public bool callOnce = false;
		public bool pause = false;
	}
}

