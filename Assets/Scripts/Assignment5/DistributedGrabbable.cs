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
        
        if (isGrabbed)
        {

            return false;
            
        }
        else
        {
            photonView.RPC("GrabRPC", RpcTarget.AllBuffered, true);
            photonView.RequestOwnership();
            return true;
        }
    }

    public void Release()
    {
        photonView.RPC("GrabRPC", RpcTarget.AllBuffered, false);
    }

    #endregion

    #region RPCS

    [PunRPC]
    public void GrabRPC(bool b)
    {
        isGrabbed = b;
    }

    #endregion
}
