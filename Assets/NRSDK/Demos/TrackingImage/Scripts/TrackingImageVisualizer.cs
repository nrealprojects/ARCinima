﻿//-----------------------------------------------------------------------
// <copyright file="TrackingImageVisualizer.cs" company="Google">
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

namespace NRKernal.NRExamples
{
    using UnityEngine;

    /// <summary>
    /// Uses 4 frame corner objects to visualize an TrackingImage.
    /// </summary>
    public class TrackingImageVisualizer : MonoBehaviour
    {
        /// <summary>
        /// The TrackingImage to visualize.
        /// </summary>
        public NRTrackableImage Image;

        /// <summary>
        /// A model for the lower left corner of the frame to place when an image is detected.
        /// </summary>
        public GameObject FrameLowerLeft;

        /// <summary>
        /// A model for the lower right corner of the frame to place when an image is detected.
        /// </summary>
        public GameObject FrameLowerRight;

        /// <summary>
        /// A model for the upper left corner of the frame to place when an image is detected.
        /// </summary>
        public GameObject FrameUpperLeft;

        /// <summary>
        /// A model for the upper right corner of the frame to place when an image is detected.
        /// </summary>
        public GameObject FrameUpperRight;

        public GameObject Axis;

        /// <summary>
        /// The Unity Update method.
        /// </summary>
        public void Update()
        {
            if (Image == null || Image.GetTrackingState() != TrackingState.Tracking)
            {
                FrameLowerLeft.SetActive(false);
                FrameLowerRight.SetActive(false);
                FrameUpperLeft.SetActive(false);
                FrameUpperRight.SetActive(false);
                Axis.SetActive(false);
                return;
            }

            float halfWidth = Image.ExtentX / 2;
            float halfHeight = Image.ExtentZ / 2;
            FrameLowerLeft.transform.localPosition = (halfWidth * Vector3.left) + (halfHeight * Vector3.back);
            FrameLowerRight.transform.localPosition = (halfWidth * Vector3.right) + (halfHeight * Vector3.back);
            FrameUpperLeft.transform.localPosition = (halfWidth * Vector3.left) + (halfHeight * Vector3.forward);
            FrameUpperRight.transform.localPosition = (halfWidth * Vector3.right) + (halfHeight * Vector3.forward);

            var center = Image.GetCenterPose();
            transform.position = center.position;
            transform.rotation = center.rotation;

            FrameLowerLeft.SetActive(true);
            FrameLowerRight.SetActive(true);
            FrameUpperLeft.SetActive(true);
            FrameUpperRight.SetActive(true);
            Axis.SetActive(true);
        }
    }
}