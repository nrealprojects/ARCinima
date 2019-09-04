using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Nreal/Create VideoEffectEventts Config ")]
public class VideoEffectEventConfig : ScriptableObject
{
    [SerializeField]
    public EffectEvent[] effects;
}

[Serializable]
public class EffectEvent
{
    public EffectType type;
    public float start;
    public float end;
    public EffectData data;
}

[Serializable]
public class EffectData
{
    public GameObject effectObj;
}

public enum EffectType
{
    Load,
    ScreenChange,
    Poetry,
    Flare,
}