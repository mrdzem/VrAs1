using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class DistributedGrabbable : MonoBehaviourPun
{
    #region Member Variables

    private bool isGrabbed = false;

    #endregion

    #region Custom Methods

    public bool RequestGrab()
    {
        return false;
    }

    public void Release()
    {
        
    }

    #endregion

    #region RPCS

    [PunRPC]
    public void GrabRPC(bool b)
    {
        
    }

    #endregion
}
