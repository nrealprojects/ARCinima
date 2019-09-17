/****************************************************************************
* Copyright 2019 Nreal Techonology Limited.All rights reserved.
*
* This file is part of NRSDK.
*
* https://www.nreal.ai/        
*
*****************************************************************************/

namespace NRKernal
{
    using UnityEngine;
    using UnityEditor;
    using System.IO;

    /// @cond EXCLUDE_FROM_DOXYGEN
    public class SDKUpdateCenter : EditorWindow
    {
        [MenuItem("NRWindows/SDKUpdateCenter")]
        static void Init()
        {
            SDKUpdateCenter myWindow = (SDKUpdateCenter)EditorWindow.GetWindow(typeof(SDKUpdateCenter), false, "SDKVersion", true);
            myWindow.Show();
        }

        public void OnGUI()
        {
            var resourcePath = GetResourcePath();
            var logo = AssetDatabase.LoadAssetAtPath<Texture2D>(resourcePath + "icon.png");
            var rect = GUILayoutUtility.GetRect(position.width, 150, GUI.skin.box);
            if (logo) GUI.DrawTexture(rect, logo, ScaleMode.ScaleToFit);

            var style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
            EditorGUILayout.LabelField(string.Format(" Wellcome to use NRSDK {0}", NRVersionInfo.GetVersion()), style);
        }

        string GetResourcePath()
        {
            var ms = MonoScript.FromScriptableObject(this);
            var path = AssetDatabase.GetAssetPath(ms);
            path = Path.GetDirectoryName(path);
            return path.Substring(0, path.Length - "Editor".Length - 1) + "Textures/";
        }
    }
    /// @endcond
}
