/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// @cond EXCLUDE_FROM_DOXYGEN
    /**
    * @brief The class parses the touch position of smart phone to usable states by invoking parsing method every frame.
    */
    public class NRPhoneControllerStateParser : IControllerStateParser
    {
        private bool[] _buttons_down = new bool[3];
        private bool[] _buttons_up = new bool[3];
        private bool[] _buttons = new bool[3];
        private bool[] _down = new bool[3];
        private Vector2 _touch;
        private bool _touch_status;
        private bool _physical_button;
        private bool _physical_button_down;
        private int _current_down_btn = -1;
        private float _device_touch_x;
        private float _device_touch_y;
        private float _touch_area_x_min;
        private float _touch_area_x_max;
        private float _touch_area_y_min;
        private float _touch_area_y_max;
        private float _touch_area_y_center;
        private bool _screen_adapted = false;

        private const float Design_Phone_Screen_Width = 1080f;
        private const float Design_Phone_Screen_Height = 2340f;
        private const float Design_Touch_Area_Top = 1115;
        private const float Design_Touch_Area_Bottom = 2065;
        private const float PRECISION = 0.000001f;

        private void AdaptPhoneScreenSize()
        {
            AndroidJavaClass j = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = j.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject displayManager = currentActivity.Call<AndroidJavaObject>("getSystemService", new AndroidJavaObject("java.lang.String", "display"));
            AndroidJavaObject display = displayManager.Call<AndroidJavaObject>("getDisplay", 0);
            AndroidJavaObject outSize = new AndroidJavaObject("android.graphics.Point");
            display.Call("getRealSize", outSize);
            float _real_screen_width = outSize.Get<int>("x");
            float _real_screen_height = outSize.Get<int>("y");

            _touch_area_x_min = -0.93f;
            _touch_area_x_max = 0.93f;
            float _real_touch_area_top = _real_screen_height - (Design_Phone_Screen_Height - Design_Touch_Area_Top) * _real_screen_width / Design_Phone_Screen_Width;
            float _real_touch_area_bottom = _real_screen_height - (Design_Phone_Screen_Height - Design_Touch_Area_Bottom) * _real_screen_width / Design_Phone_Screen_Width;
            _touch_area_y_min = _real_touch_area_bottom / _real_screen_height;
            _touch_area_y_max = _real_touch_area_top / _real_screen_height;
            _touch_area_y_min = -(_touch_area_y_min - 0.5f) * 2f;  // value range maps to  -1f ~ 1f 
            _touch_area_y_max = -(_touch_area_y_max - 0.5f) * 2f;  // value range maps to  -1f ~ 1f 
            _touch_area_y_center = (_touch_area_y_min + _touch_area_y_max) / 2f;
            _screen_adapted = true;
        }

        public void ParserControllerState(ControllerState state)
        {
            if (!_screen_adapted)
                AdaptPhoneScreenSize();
            _device_touch_x = state.touchPos.x;
            _device_touch_y = state.touchPos.y;
            _physical_button_down = _physical_button;
            _physical_button = (Mathf.Abs(_device_touch_x) > PRECISION || Mathf.Abs(_device_touch_y) > PRECISION);

            if (_device_touch_x < _touch_area_x_min
                || _device_touch_x > _touch_area_x_max
                || _device_touch_y < _touch_area_y_min
                || _device_touch_y > _touch_area_y_max
                || !_physical_button)
            {
                _touch_status = false;
                _touch.x = 0f;
                _touch.y = 0f;
            }
            else
            {
                _touch_status = true;
                _touch.x = _device_touch_x / (_touch_area_x_max - _touch_area_x_min) * 2f;
                _touch.y = (_device_touch_y - _touch_area_y_center) / (_touch_area_y_max - _touch_area_y_min) * 2f;
            }

            lock (_buttons)
            {
                lock (_down)
                {
                    for (int i = 0; i < _buttons.Length; ++i)
                    {
                        _down[i] = _buttons[i];
                    }
                }

                if (_current_down_btn != -1)
                {
                    _buttons[_current_down_btn] = _physical_button;
                    if (!_buttons[_current_down_btn])
                        _current_down_btn = -1;
                }
                else
                {
                    _buttons[0] = false;  //Trigger
                    _buttons[1] = false;  //App
                    _buttons[2] = false;  //Home
                    bool _is_down = !_physical_button_down & _physical_button;
                    if (_touch_status)
                    {
                        _buttons[0] = _physical_button && _is_down;
                    }
                    else if (_device_touch_y > _touch_area_y_max)
                    {
                        _buttons[1] = _physical_button && _is_down;
                    }
                    else if (_device_touch_y < _touch_area_y_min)
                    {
                        _buttons[2] = _physical_button && _is_down;
                    }

                    _current_down_btn = -1;
                    for (int i = 0; i < 3; i++)
                    {
                        if (_buttons[i])
                        {
                            _current_down_btn = i;
                            break;
                        }
                    }
                }

                lock (_buttons_up)
                {
                    lock (_buttons_down)
                    {
                        for (int i = 0; i < _buttons.Length; ++i)
                        {
                            _buttons_up[i] = (_down[i] & !_buttons[i]);
                            _buttons_down[i] = (!_down[i] & _buttons[i]);
                        }
                    }
                }
            }

            state.isTouching = _touch_status;
            state.touchPos = _touch;
            state.buttonsState =
                (_buttons[0] ? ControllerButton.TRIGGER : 0)
                | (_buttons[1] ? ControllerButton.APP : 0)
                | (_buttons[2] ? ControllerButton.HOME : 0);
            state.buttonsDown =
                (_buttons_down[0] ? ControllerButton.TRIGGER : 0)
                | (_buttons_down[1] ? ControllerButton.APP : 0)
                | (_buttons_down[2] ? ControllerButton.HOME : 0);
            state.buttonsUp =
                (_buttons_up[0] ? ControllerButton.TRIGGER : 0)
                | (_buttons_up[1] ? ControllerButton.APP : 0)
                | (_buttons_up[2] ? ControllerButton.HOME : 0);

            if (_buttons_down[0] || _buttons_down[1] || _buttons_down[2])
                NRInput.TriggerHapticVibration(0.1f);
        }
    }
    /// @endcond
}
