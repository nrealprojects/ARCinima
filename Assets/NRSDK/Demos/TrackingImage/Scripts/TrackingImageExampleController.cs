//-----------------------------------------------------------------------
// <copyright file="TrackingImageExampleController.cs" company="Google">
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
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Controller for TrackingImage example.
    /// </summary>
    public class TrackingImageExampleController : MonoBehaviour
    {
        /// <summary>
        /// A prefab for visualizing an TrackingImage.
        /// </summary>
        public TrackingImageVisualizer TrackingImageVisualizerPrefab;

        /// <summary>
        /// The overlay containing the fit to scan user guide.
        /// </summary>
        public GameObject FitToScanOverlay;

        private Dictionary<int, TrackingImageVisualizer> m_Visualizers
            = new Dictionary<int, TrackingImageVisualizer>();

        private List<NRTrackableImage> m_TempTrackingImages = new List<NRTrackableImage>();

        /// <summary>
        /// The Unity Update method.
        /// </summary>
        public void Update()
        {
#if !UNITY_DEITOR
            // Check that motion tracking is tracking.
            if (NRFrame.SessionStatus != SessionState.Tracking)
            {
                return;
            }
#endif
            // Get updated augmented images for this frame.
            NRFrame.GetTrackables<NRTrackableImage>(m_TempTrackingImages, NRTrackableQueryFilter.New);

            // Create visualizers and anchors for updated augmented images that are tracking and do not previously
            // have a visualizer. Remove visualizers for stopped images.
            foreach (var image in m_TempTrackingImages)
            {
                TrackingImageVisualizer visualizer = null;
                m_Visualizers.TryGetValue(image.GetDataBaseIndex(), out visualizer);
                if (image.GetTrackingState() == TrackingState.Tracking && visualizer == null)
                {
                    NRDebug.Log("Create new TrackingImageVisualizer!");
                    // Create an anchor to ensure that NRSDK keeps tracking this augmented image.
                    visualizer = (TrackingImageVisualizer)Instantiate(TrackingImageVisualizerPrefab, image.GetCenterPose().position, image.GetCenterPose().rotation);
                    visualizer.Image = image;
                    visualizer.transform.parent = transform;
                    m_Visualizers.Add(image.GetDataBaseIndex(), visualizer);
                }
                else if (image.GetTrackingState() == TrackingState.Stopped && visualizer != null)
                {
                    m_Visualizers.Remove(image.GetDataBaseIndex());
                    Destroy(visualizer.gameObject);
                }

                FitToScanOverlay.SetActive(false);
            }

        }
    }
}
