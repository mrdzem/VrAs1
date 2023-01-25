using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.InputSystem;

public class LightSwitch : MonoBehaviourPunCallbacks
{
    #region Member Variables

    public List<Light> lightSources;
    public InputActionProperty lightToggleAction;

    private bool lightOn = true;
    
    #endregion

    #region MonoBehaviour Callbacks

    private void Start()
    {
        foreach (var light in lightSources)
        {
            light.intensity = lightOn ? 1f : 0f;
        }
    }

    private void Update()
    {
        if(lightToggleAction.action.WasPressedThisFrame())
            ToggleLight();
    }

    #endregion

    #region Custom Methods

    private void ToggleLight()
    {
        photonView.RPC("ToggleLightRPC", RpcTarget.All);
    }

    #endregion

    #region PUN Callbacks

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        photonView.RPC("SendStateRPC", RpcTarget.All, lightOn);
    }

    #endregion

    #region RPCs

    [PunRPC]
    public void ToggleLightRPC()
    {
        // use to toggle lights
        lightOn = !lightOn;
        Debug.Log("light switch value to : " + lightOn);
        foreach (var light in lightSources)
        {
            light.intensity = lightOn ? 1f : 0f;
        }
    }

    [PunRPC]
    public void SendStateRPC(bool lightOnn)
    {
        lightOn = lightOnn;
        // use to inform late joined users
        foreach (var light in lightSources)
        {
            light.intensity = lightOn ? 1f : 0f;
        }
        Debug.Log(lightOn);
    }

    #endregion
}
