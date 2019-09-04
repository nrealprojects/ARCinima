using UnityEngine;
using UnityEngine.UI;
using System;
using NREAL.AR;

public class MaskFader : SingleInstance<MaskFader>
{
    //[SerializeField]
    //Image mMask;
    [SerializeField]
    CanvasGroup mGroup;

    private bool _flag = false;
    private float _from = 0f;
    private float _to = 1f;
    private float _current = 0f;
    private float _delta = 0f;

    public float fadeinTime = 0.6f;
    public float fadeoutTime = 0.6f;

    private Action fadeInComplete;
    private Action fadeOutComplete;

    private enum MaskState
    {
        open,          // 遮罩开启状态，全黑
        closed        //遮罩关闭状态，完全透明
    }
    private MaskState _state = MaskState.closed;
    private MaskState _target = MaskState.closed;

    void FixedUpdate()
    {
        if (_flag)
        {
            if (_target == MaskState.closed)
            {
                if (_current < fadeinTime)
                {
                    _delta = 1 / (fadeinTime / Time.fixedDeltaTime);
                    _current += Time.fixedDeltaTime;
                    mGroup.alpha += -_delta;
                }
                else
                {
                    if (fadeInComplete != null)
                    {
                        fadeInComplete();
                    }
                    this.ClearState();
                }
            }
            else if (_target == MaskState.open)
            {
                if (_current < fadeoutTime)
                {
                    _delta = 1 / (fadeoutTime / Time.fixedDeltaTime);
                    _current += Time.fixedDeltaTime;
                    mGroup.alpha += _delta;
                }
                else
                {
                    if (fadeOutComplete != null)
                    {
                        fadeOutComplete();
                    }
                    this.ClearState();
                }
            }
        }
    }

    private void ClearState()
    {
        _state = _target;
        _current = 0f;
        _flag = false;
        mGroup.alpha = _to;
        fadeInComplete = null;
        fadeOutComplete = null;
    }

    public void FadeIn(Action complete = null)
    {
        if (_state == MaskState.open && !_flag)
        {
            _flag = true;
            _from = 1;
            _to = 0;
            mGroup.alpha = _from;
            _target = MaskState.closed;
            fadeInComplete = complete;
        }
    }

    public void FadeOut(Action complete = null)
    {
        if (_state == MaskState.closed && !_flag)
        {
            _flag = true;
            _from = 0;
            _to = 1;
            mGroup.alpha = _from;
            _target = MaskState.open;
            fadeOutComplete = complete;
        }
    }

    public void OpenMask()
    {
        _state = MaskState.open;
        mGroup.alpha = 0;
    }

    public void CloseMask()
    {
        _state = MaskState.closed;
        mGroup.alpha = 1;
    }
}