using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NREAL.AR.VideoPlayer
{
    public class VideoPlayerManager : MonoBehaviour
    {
        [SerializeField] RenderHeads.Media.AVProVideo.MediaPlayer videoPlayer;
        [SerializeField] ScreenController screenManager;
        [SerializeField] VideoEffectLoader effectLoader;

        private float timeClock = 0f;

        private void Awake()
        {
            screenManager.SetScreenType(ScreenController.ScreenType.MovieNormal);

            //videoPlayer.OnReady += () =>
            //{
            //    StartCoroutine(TimeClock());
            //    effectLoader.RegistConfigEvent();
            //};
        }

        private IEnumerator TimeClock()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                timeClock += 1;
            }
        }
    }
}