using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PhotonView))]
public class PhysicalLightSwitch : MonoBehaviourPunCallbacks
{
    #region Member Variables

    public List<Light> lightSources;

    private bool lightOn = true;
    
    #endregion

    #region MonoBehaviour Callbacks

    private void Start()
    {
        
    }

    #endregion

    #region Custom Methods

    private void ToggleLight()
    {
        
    }

    #endregion

    #region PUN Callbacks

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        
    }

    #endregion

    #region RPCs

    [PunRPC]
    public void ToggleLightRPC()
    {
        
    }

    [PunRPC]
    public void SendStateRPC(bool lightOn)
    {
        
    }

    #endregion
}
