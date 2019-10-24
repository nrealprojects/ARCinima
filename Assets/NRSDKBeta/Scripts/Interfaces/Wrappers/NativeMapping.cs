/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          

* https://www.nreal.ai/         
* 
*****************************************************************************/

namespace NRKernal
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    /**
    * @brief Session Native API.
    */
    internal partial class NativeMapping
    {
        private UInt64 m_DatabaseHandle;

        private NativeInterface m_NativeInterface;

        public NativeMapping(NativeInterface nativeInterface)
        {
            m_NativeInterface = nativeInterface;
        }

        public bool CreateDataBase()
        {
            Debug.Log("Start to create worldmap...");
            var result = NativeApi.NRWorldMapDatabaseCreate(m_NativeInterface.TrackingHandle, ref m_DatabaseHandle);
            Debug.Log("End to create worldmap :" + result);
            return result == NativeResult.Success;
        }

        public bool DestroyDataBase()
        {
            Debug.Log("Start to destroy worldmap...");
            var result = NativeApi.NRWorldMapDatabaseDestroy(m_NativeInterface.TrackingHandle, m_DatabaseHandle);
            Debug.Log("End to destroy worldmap :" + result);
            return result == NativeResult.Success;
        }

        public bool LoadMap(string path)
        {
            Debug.Log("Start to load worldmap...");
            var result = NativeApi.NRWorldMapDatabaseLoadFile(m_NativeInterface.TrackingHandle, m_DatabaseHandle, path);
            Debug.Log("End to load worldmap :" + result);
            return result == NativeResult.Success;
        }

        public bool SaveMap(string path)
        {
            Debug.Log("Start to create worldmap...");
            var result = NativeApi.NRWorldMapDatabaseSaveFile(m_NativeInterface.TrackingHandle, m_DatabaseHandle, path);
            Debug.Log("End to create worldmap :" + result);
            return result == NativeResult.Success;
        }

        public UInt64 AddAnchor(Pose pose)
        {
            Debug.Log("Start to add anchor...");
            UInt64 anchorHandle = 0;
            NativeMat4f nativePose;
            ConversionUtility.UnityPoseToApiPose(pose, out nativePose);
            var result = NativeApi.NRTrackingAcquireNewAnchor(m_NativeInterface.TrackingHandle, nativePose, ref anchorHandle);
            Debug.Log("End to add anchor :" + result);
            return anchorHandle;
        }

        public UInt64 CreateAnchorList()
        {
            Debug.Log("Start to create anchor list...");
            UInt64 anchorlisthandle = 0;
            var result = NativeApi.NRAnchorListCreate(m_NativeInterface.TrackingHandle, ref anchorlisthandle);
            Debug.Log("End to create anchor :" + result);
            return anchorlisthandle;
        }

        public bool DestroyAnchorList(UInt64 anchorlisthandle)
        {
            Debug.Log("Start to destroy anchor list...");
            var result = NativeApi.NRAnchorListDestroy(m_NativeInterface.TrackingHandle, anchorlisthandle);
            Debug.Log("End to destroy anchor :" + result);
            return result == NativeResult.Success;
        }

        public int GetAnchorListSize(UInt64 anchor_list_handle)
        {
            int size = 0;
            NativeApi.NRAnchorListGetSize(m_NativeInterface.TrackingHandle, anchor_list_handle, ref size);
            return size;
        }

        public UInt64 AcquireItem(UInt64 anchor_list_handle, int index)
        {
            UInt64 anchorHandle = 0;
            NativeApi.NRAnchorListAcquireItem(m_NativeInterface.TrackingHandle, anchor_list_handle, index, ref anchorHandle);
            return anchorHandle;
        }

        public TrackingState GetTrackingState(UInt64 anchor_handle)
        {
            TrackingState trackingState = TrackingState.Stopped;
            NativeApi.NRAnchorGetTrackingState(m_NativeInterface.TrackingHandle, anchor_handle, ref trackingState);
            return trackingState;
        }

        public Pose GetAnchorPose(UInt64 anchor_handle)
        {
            NativeMat4f nativePose = NativeMat4f.identity;
            NativeApi.NRAnchorGetPose(m_NativeInterface.TrackingHandle, anchor_handle, ref nativePose);

            Pose unitypose;
            ConversionUtility.ApiPoseToUnityPose(nativePose, out unitypose);
            return unitypose;
        }

        public bool DestroyAnchor(UInt64 anchor_handle)
        {
            var result = NativeApi.NRAnchorDestroy(m_NativeInterface.TrackingHandle, anchor_handle);
            return result == NativeResult.Success;
        }

        private partial struct NativeApi
        {
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRWorldMapDatabaseCreate(UInt64 tracking_handle,
                ref UInt64 out_world_map_database_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRWorldMapDatabaseDestroy(UInt64 tracking_handle,
                UInt64 world_map_database_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRWorldMapDatabaseLoadFile(UInt64 tracking_handle,
                UInt64 world_map_database_handle, string world_map_database_file_path);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRWorldMapDatabaseSaveFile(UInt64 tracking_handle,
                UInt64 world_map_database_handle, string world_map_database_file_path);

            // NRTracking
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackingAcquireNewAnchor(
                 UInt64 tracking_handle, NativeMat4f pose, ref UInt64 out_anchor_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackingUpdateAnchors(
               UInt64 tracking_handle, UInt64 out_anchor_list_handle);

            // NRAnchorList
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRAnchorListCreate(
                 UInt64 tracking_handle, ref UInt64 out_anchor_list_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRAnchorListDestroy(
                 UInt64 tracking_handle, UInt64 anchor_list_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRAnchorListGetSize(UInt64 tracking_handle,
                UInt64 anchor_list_handle, ref int out_list_size);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRAnchorListAcquireItem(UInt64 tracking_handle,
                UInt64 anchor_list_handle, int index, ref UInt64 out_anchor);

            // NRAnchor
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRAnchorGetTrackingState(UInt64 tracking_handle,
                UInt64 anchor_handle, ref TrackingState out_tracking_state);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRAnchorGetPose(UInt64 tracking_handle,
                UInt64 anchor_handle, ref NativeMat4f out_pose);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRAnchorDestroy(UInt64 tracking_handle,
                UInt64 anchor_handle);
        }
    }
}
