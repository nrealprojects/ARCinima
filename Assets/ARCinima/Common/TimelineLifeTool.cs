using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineLifeTool : MonoBehaviour {
    public bool playOnlyOnce = true;
    public PlayableDirector playableDirector;

    private void Awake()
    {
        if (playOnlyOnce)
            playableDirector.extrapolationMode = DirectorWrapMode.Hold;
    }

    private void OnDisable()
    {
        if (playOnlyOnce)
        {
            if (playableDirector && playableDirector.time >= playableDirector.duration)
            {
                playableDirector.enabled = false;
            }
        }
    }
}
