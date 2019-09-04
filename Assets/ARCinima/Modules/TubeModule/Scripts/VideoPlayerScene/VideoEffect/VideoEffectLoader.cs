using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VideoEffectLoader : MonoBehaviour
{
    [SerializeField] VideoEffectEventConfig m_EffectsConfig1;
    [SerializeField] VideoEffectEventConfig m_EffectsConfig2;
    private List<GameObject> loadedEffects = new List<GameObject>();
    private Coroutine videoEffectCor = null;

    GameObject _movieScreen = null;
    GameObject movieScreen
    {
        get
        {
            if (_movieScreen == null)
            {
                _movieScreen = GameObject.Find("ScreenController/MovieScreen"); ;
            }
            return _movieScreen;
        }
    }

    private void OnDisable()
    {
        this.Clear();
    }

    public void RegistConfigEvent(int index = 1)
    {
        if (videoEffectCor != null)
        {
            StopCoroutine(videoEffectCor);
        }
        VideoEffectEventConfig config = null;
        if (index == 1)
        {
            config = m_EffectsConfig1;
        }
        else if (index == 2)
        {
            config = m_EffectsConfig2;
        }
        if (config != null)
        {
            for (int i = 0; i < config.effects.Length; i++)
            {
                RegistEffectEvent(config.effects[i]);
            }
        }
    }

    private GameObject Load(EffectEvent effect)
    {
        GameObject go = null;
        switch (effect.type)
        {
            case EffectType.Load:
                go = Instantiate(effect.data.effectObj, transform);
                go.transform.position = Camera.main.transform.position;
                break;
            case EffectType.ScreenChange:
                go = Instantiate(effect.data.effectObj, transform);
                go.transform.position = movieScreen.transform.position;
                movieScreen.SetActive(false);
                Invoke("ShowScreen", effect.end - effect.start);
                break;
            case EffectType.Poetry:
                go = Instantiate(effect.data.effectObj, transform);
                go.transform.position = movieScreen.transform.position;
                break;
            case EffectType.Flare:
                go = Instantiate(effect.data.effectObj);
                break;
            default:
                break;
        }
        loadedEffects.Add(go);
        return go;
    }

    private void ShowScreen()
    {
        movieScreen.SetActive(true);
    }

    private void RegistEffectEvent(EffectEvent effect)
    {
        videoEffectCor = StartCoroutine(PlayVideoEffct(effect));
    }

    public void Clear()
    {
        this.StopAllCoroutines();
        for (int i = 0; i < loadedEffects.Count; i++)
        {
            if (loadedEffects[i] != null)
            {
                DestroyImmediate(loadedEffects[i]);
            }
        }
        //for (int i = 0; i < transform.childCount; i++)
        //{
        //    DestroyImmediate(transform.GetChild(i));
        //}
        loadedEffects.Clear();
    }

    private IEnumerator PlayVideoEffct(EffectEvent effect)
    {
        if (effect != null)
        {
            float s;
            if (effect.start < 0f)
            {
                s = 0f;
            }
            else
            {
                s = effect.start;
            }
            yield return new WaitForSeconds(s);
            GameObject temp = this.Load(effect);
            if (effect.end > 0 && effect.end > effect.start)
            {
                yield return new WaitForSeconds(effect.end - effect.start);
                if (effect.type == EffectType.Poetry)
                {
                    temp.GetComponent<PoetryEffect>().FadeOut();
                }
                else
                {
                    Destroy(temp);
                }
            }
        }
    }
}