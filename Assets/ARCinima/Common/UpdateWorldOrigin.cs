using UnityEngine;
using NREAL.AR;
using UnityEngine.SceneManagement;
using System;
using NRKernal;

public class UpdateWorldOrigin : MonoBehaviour
{
    public static Action OnLoadRoom;
    public bool useMarker = true;
    public bool onlyUseEuler_Y = true;

    private int RootID = -1;
    private bool isRoomLoaded = false;
    private int markerUpdateCount = 3;
    //private int specialNumber = 0;

    void Start()
    {
        if (useMarker)
            MarkerDetector.onARMarkerUpdate += onARMarkerUpdate;
        //ARControllerManager.Instance.arInput.OnDoubleClick += delegate
        //{
        //    specialNumber = 6;
        //};
    }

    void Update()
    {
        if ((!useMarker && !isRoomLoaded && NRInput.GetButtonUp(ControllerHandEnum.Left, ControllerButton.TRIGGER)))
            RootID = 0;

        if (RootID != -1 && !isRoomLoaded)
        {
            isRoomLoaded = true;
            if (OnLoadRoom != null)
                OnLoadRoom();
            SceneManager.LoadSceneAsync("AWE_Room");
        }

        //// Reset the world coordinates
        //if (ARInput.GetButtonDown(ControllerButtonEnum.OK) || ARInput.GetButtonDown(ControllerButtonEnum.HOME))
        //    specialNumber = 0;
        //// Double click OK, then click CANCEL for five times will reset marker
        //if (ARInput.GetButtonUp(ControllerButtonEnum.CANCEL))
        //{
        //    specialNumber -= 1;
        //    if (specialNumber == 1)
        //        ResetMarker();
        //}
    }

    void onARMarkerUpdate(Vector3 pos, Quaternion rot)
    {
        if (markerUpdateCount <= 0)
            return;
        markerUpdateCount--;
        RootID = 0;
        Debug.Log("MarkerPosition:" + pos.ToString());
        Debug.Log("MarkerEulerAngle:" + rot.eulerAngles.ToString());
        var marker_in_world = TransformUtils.GetTMatrix(pos, rot);
        var world_in_marker = Matrix4x4.Inverse(marker_in_world);

        transform.position = TransformUtils.GetPositionFromTMatrix(world_in_marker);
        transform.rotation = TransformUtils.GetRotationFromTMatrix(world_in_marker);
        if (onlyUseEuler_Y)
            transform.eulerAngles = Vector3.up * transform.eulerAngles.y;
    }

    //private void ResetMarker()
    //{
    //    specialNumber = 0;
    //    transform.position = Vector3.zero;
    //    transform.rotation = Quaternion.identity;
    //    RootID = -1;
    //    markerUpdateCount = 3;
    //}

    //private void ResetWorldOrigin(Vector3 position, Quaternion rotation)
    //{
    //    // Rotate 90 degree
    //    //var adjustTransform = TransformUtils.GetTMatrix(Vector3.zero, Quaternion.Euler(90, 0, 0));      
    //    //var marker_in_world = TransformUtils.GetTMatrix(position, rotation);
    //    //var world_in_marker = adjustTransform * Matrix4x4.Inverse(marker_in_world);

    //    var marker_in_world = TransformUtils.GetTMatrix(position, rotation);
    //    var world_in_marker = Matrix4x4.Inverse(marker_in_world);

    //    transform.position = TransformUtils.GetPositionFromTMatrix(world_in_marker);
    //    transform.rotation = TransformUtils.GetRotationFromTMatrix(world_in_marker);
    //}

    ////这是之前MWC用的
    //void onARMarkerUpdate(ARLibMarker result)
    //{
    //    if (RootID != -1 && RootID != (int)result.identifier) return;

    //    if (!isInited)
    //    {
    //        isInited = true;
    //        RootID = (int)result.identifier;
    //    }

    //    var marker_in_world = TransformUtils.GetTMatrix(result.position, result.quaternion);
    //    var world_in_marker = Matrix4x4.Inverse(marker_in_world);

    //    transform.position = TransformUtils.GetPositionFromTMatrix(world_in_marker);
    //    transform.rotation = TransformUtils.GetRotationFromTMatrix(world_in_marker);

    //    if (RootID == 0 && !isRoomLoaded)
    //    {
    //        isRoomLoaded = true;
    //        SceneManager.LoadScene("RoomCes_A");
    //    }
    //    else if (RootID == 1 && !isRoomLoaded)
    //    {
    //        isRoomLoaded = true;
    //        SceneManager.LoadScene("RoomCes_B");
    //    }
    //}


    //public GameObject markerObj;

    //void Start()
    //{
    //    ARLibInterface.ARMarkerUpdateEvent += OnARMarkerUpdate;
    //}

    ///// <summary>
    ///// 返回Marker的数据，主要包括position和rotation
    ///// </summary>
    ///// <param name="marker"></param>
    //public void OnARMarkerUpdate(ARLibMarker marker)
    //{
    //    Loom.QueueOnMainThread(() =>
    //    {
    //        // 以Marker返回的position和rotation作为世界坐标系的（0,0,0）点和正方向，通过转换ARCameras的父节点实现坐标系对齐
    //        var marker_in_world = TransformUtils.GetTMatrix(marker.position, marker.quaternion);
    //        var world_in_marker = Matrix4x4.Inverse(marker_in_world);

    //        this.transform.position = TransformUtils.GetPositionFromTMatrix(world_in_marker);
    //        this.transform.rotation = TransformUtils.GetRotationFromTMatrix(world_in_marker);

    //        markerObj.SetActive(true);
    //    });
    //}
}