using UnityEngine;
using NREAL.AR;

public enum SoundClipEnum
{
    Hover,
    ItemHover,
    ModuleStart
}

public class SoundManager : Singleton<SoundManager>
{
    public AudioClip[] clips;

    public void PlaySoundEffect(SoundClipEnum clipEnum, Vector3 pos)
    {
        AudioClip clip = GetClip(clipEnum);
        if (clip)
            AudioSource.PlayClipAtPoint(clip, pos);
    }

    private AudioClip GetClip(SoundClipEnum clipEnum)
    {
        try
        {
            switch (clipEnum)
            {
                case SoundClipEnum.Hover:
                    return clips[0];
                case SoundClipEnum.ItemHover:
                    return clips[1];
                case SoundClipEnum.ModuleStart:
                    return clips[2];
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.ToString());
        }
        return null;
    }
}