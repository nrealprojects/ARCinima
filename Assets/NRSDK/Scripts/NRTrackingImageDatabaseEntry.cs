//-----------------------------------------------------------------------
// <copyright file="TrackingImageDatabaseEntry.cs" company="Google">
//
// Copyright 2018 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

namespace NRKernal
{
    using System;
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
    using System.IO;
#endif

    /**
     * @brief Hold the total infomation of a image data base item.
     */
    [Serializable]
    public struct NRTrackingImageDatabaseEntry
    {
        /// <summary>
        /// The name assigned to the tracked image.
        /// </summary>
        public string Name;

        /// <summary>
        /// The width of the image in meters.
        /// </summary>
        public float Width;

        /// <summary>
        /// The height of the image in meters.
        /// </summary>
        public float Height;

        /// <summary>
        /// The quality of the image.
        /// </summary>
        public string Quality;

        /// <summary>
        /// The Unity GUID for this entry.
        /// </summary>
        public string TextureGUID;

        /// <summary>
        /// Contructs a new Augmented Image database entry.
        /// </summary>
        /// <param name="name">The image name.</param>
        /// <param name="width">The image width in meters or 0 if the width is unknown.</param>
        public NRTrackingImageDatabaseEntry(string name, float width, float height)
        {
            Name = name;
            TextureGUID = string.Empty;
            Width = width;
            Height = height;
            Quality = string.Empty;
            TextureGUID = string.Empty;
        }

#if UNITY_EDITOR
        /// @cond EXCLUDE_FROM_DOXYGEN
        public NRTrackingImageDatabaseEntry(string name, Texture2D texture, float width, float height)
        {
            Name = name;
            TextureGUID = string.Empty;
            Width = width;
            Quality = string.Empty;
            Height = height;
            Texture = texture;
        }

        public NRTrackingImageDatabaseEntry(string name, Texture2D texture)
        {
            Name = name;
            TextureGUID = string.Empty;
            Width = 0;
            Quality = string.Empty;
            Height = 0;
            Texture = texture;
        }

        public NRTrackingImageDatabaseEntry(Texture2D texture)
        {
            Name = "Unnamed";
            TextureGUID = string.Empty;
            Width = 0;
            Quality = string.Empty;
            Height = 0;
            Texture = texture;
        }

        public Texture2D Texture
        {
            get
            {
                return AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(TextureGUID));
            }
            set
            {
                string path = AssetDatabase.GetAssetPath(value);
                TextureGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(value));
                var fileName = Path.GetFileName(path);
                Name = fileName.Replace(Path.GetExtension(fileName), string.Empty);
            }
        }

        public override string ToString()
        {
            return string.Format("Name:{0} Quality:{1}", Name, Quality);
        }
        /// @endcond
#endif
    }

}
