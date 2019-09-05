using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NRKernal;

public class NREventCenter {
    public static GameObject GetCurrentRaycastTarget()
    {
        GameObject target = null;
        ControllerAnchorEnum anchor = NRInput.DomainHand == ControllerHandEnum.Right ? ControllerAnchorEnum.RightLaserAnchor : ControllerAnchorEnum.LeftLaserAnchor;
        NRPointerRaycaster raycaster = NRInput.AnchorsHelper.GetAnchor(anchor).GetComponentInChildren<NRPointerRaycaster>(true);
        var result = raycaster.FirstRaycastResult();
        if (result.isValid)
            target = result.gameObject;
        return target;
    }
}
