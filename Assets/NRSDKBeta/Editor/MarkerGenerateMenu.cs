/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          

* NRSDK is distributed in the hope that it will be usefull                                                              

* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
     Justification = "Internal")]
    public static class MarkerGenerateMenu
    {
        private const string k_SupportedImageFormatListMessage = "PNG and JPEG";

        private static readonly string MarkerDataOutPath = Application.streamingAssetsPath + "/Cocoa/";

        private static BackgroundJobExecutor s_QualityBackgroundExecutor = new BackgroundJobExecutor();

        private static readonly List<string> k_SupportedImageExtensions = new List<string>()
        {
            ".png", ".jpg", ".jpeg"
        };

        private static readonly List<string> k_UnsupportedImageExtensions = new List<string>()
        {
            ".psd", ".tiff", ".tga", ".gif", ".bmp", ".iff", ".pict"
        };

        [MenuItem("Assets/Create/NRInternal/GenMarkerData", false, 2)]
        private static void AddAssetsToNewTrackingImageDatabase()
        {
            var selectedImagePaths = new List<string>();
            bool unsupportedImagesSelected = false;

            selectedImagePaths = GetSelectedImagePaths(out unsupportedImagesSelected);
            if (unsupportedImagesSelected)
            {
                var message = string.Format("Some selected images could not be added to the TrackingImageDatabase because " +
                    "they are not in a supported format.  Supported image formats are {0}.",
                    k_SupportedImageFormatListMessage);
                Debug.LogWarningFormat(message);
                EditorUtility.DisplayDialog("Unsupported Images Selected", message, "Ok");
            }

            if (!Directory.Exists(MarkerDataOutPath))
            {
                NRDebugger.Log("Directory is not exist, create a new one...");
                Directory.CreateDirectory(MarkerDataOutPath);
            }

            AssetDatabase.Refresh();

            string out_path = MarkerDataOutPath + System.Guid.NewGuid().ToString();

            if (!Directory.Exists(out_path))
            {
                NRDebugger.Log("Directory is not exist, create a new one...");
                Directory.CreateDirectory(out_path);
            }

            string binary_path;
            if (!NRTrackingImageDatabase.FindCliBinaryPath(out binary_path))
            {
                return;
            }

            foreach (var item in selectedImagePaths)
            {
                string image_path = Application.dataPath.Substring(0, Application.dataPath.Length - 6) + item;
                s_QualityBackgroundExecutor.PushJob(() =>
                {
                    string param = string.Format("-image_path={0} -save_dir={1} -width={2}",
                 image_path, out_path, 400).Trim();
                    string result = string.Empty;
                    string error = string.Empty;

                    ShellHelper.RunCommand(binary_path, param, out result, out error);
                    Debug.Log(string.Format("result : {0} error : {1}", result, error));
                });
            }

            AssetDatabase.Refresh();
        }

        private static List<string> GetSelectedImagePaths(out bool unsupportedImagesSelected)
        {
            var selectedImagePaths = new List<string>();

            unsupportedImagesSelected = false;
            foreach (var GUID in Selection.assetGUIDs)
            {
                var path = AssetDatabase.GUIDToAssetPath(GUID);
                var extension = Path.GetExtension(path).ToLower();

                if (k_SupportedImageExtensions.Contains(extension))
                {
                    selectedImagePaths.Add(AssetDatabase.GUIDToAssetPath(GUID));
                }
                else if (k_UnsupportedImageExtensions.Contains(extension))
                {
                    unsupportedImagesSelected = true;
                }
            }

            return selectedImagePaths;
        }
    }
}
