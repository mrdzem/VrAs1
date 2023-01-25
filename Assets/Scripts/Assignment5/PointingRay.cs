using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.Demo.SlotRacer.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(LineRenderer))]
public class PointingRay : MonoBehaviourPun, IPunObservable
{
    #region Member Variables

    [SerializeField] private LineRenderer lineRenderer;

    [Header("Ray parameter")] 
    public float rayWidth;
    public float idleLength = 10f;
    public float maxDistance = 100f;
    public Color idleColor;
    public Color highlightColor;
    public LayerMask layersToInclude;
    public InputActionProperty rayActivation;

    private Vector3 lineBegin;
    private Vector3 lineEnd;
    private Color currColor;
    private bool isOn = false;

    private bool isHitting;

    #endregion

    #region MonoBehaviour Callbacks

    private void Start()
    {
        InitializeRay();
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            UpdateRay();
        }
    }

    #endregion

    #region Custom Methods

    private void InitializeRay()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;

        lineRenderer.startWidth = rayWidth;
        lineRenderer.endWidth = rayWidth;
        
        lineRenderer.positionCount = 2;

        lineRenderer.startColor = idleColor;
        lineRenderer.endColor = idleColor;

    }

    private void UpdateRay()
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (rayActivation.action.WasPressedThisFrame())
        {
            isOn = !isOn;
            lineRenderer.enabled = isOn;
        }
        lineBegin = this.transform.parent.position;
        lineRenderer.SetPosition(0, lineBegin);
        
        if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, maxDistance, layersToInclude))
        {
            lineEnd = hit.point;
            
            isHitting = true;
        }
        else
        {
            lineEnd = this.transform.parent.position + this.transform.parent.forward * idleLength;
            isHitting = false;
        }

        if (isHitting)
        {
            currColor = highlightColor;
        }
        else
        { 
            currColor = idleColor; 
        }
        lineRenderer.startColor = currColor;
        lineRenderer.endColor = currColor;



        lineRenderer.SetPosition(1, lineEnd);



    }

    #endregion

    #region IPunObservable

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (photonView.IsMine && stream.IsWriting)
        {
            stream.SendNext(isOn);
            stream.SendNext(lineBegin);
            stream.SendNext(lineEnd);
            stream.SendNext(isHitting);

        }
        else if (stream.IsReading)
        {
            isOn = (bool)stream.ReceiveNext();
            lineBegin = (Vector3)stream.ReceiveNext();
            lineEnd = (Vector3)stream.ReceiveNext();
            isHitting = (bool)stream.ReceiveNext();

            lineRenderer.enabled = isOn;
            lineRenderer.SetPosition(0, lineBegin);
            lineRenderer.SetPosition(1, lineEnd);
            if (isHitting)
            {
                currColor = highlightColor;
            }
            else
            {
                currColor = idleColor;
            }
            lineRenderer.startColor = currColor;
            lineRenderer.endColor = currColor;

        }
    }

    #endregion

}
