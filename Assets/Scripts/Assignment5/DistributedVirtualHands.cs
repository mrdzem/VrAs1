using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class DistributedVirtualHands : MonoBehaviourPun
{
    #region Member Variables

    [Header("Input Actions")] 
    public InputActionProperty rightHandGrab;
    public InputActionProperty leftHandGrab;
    
    [Header("Collider")]
    public HandCollider rightHandCollider;
    public HandCollider leftHandCollider;

    private GameObject rightGrabbedObject;
    private GameObject leftGrabbedObject;

    private Matrix4x4 rightOffsetMat;
    private Matrix4x4 leftOffsetMat;

    #endregion

    #region MonoBehaviour Callbacks

    private void Awake()
    {
        if (!photonView.IsMine)
        {
            Destroy(this);
        }
    }

    private void Update()
    {
        GrabCalculation();
    }

    #endregion

    #region Custom Methods

    public void GrabCalculation()
    {
        if (rightHandGrab.action.IsPressed())
        {
            if (rightGrabbedObject == null && rightHandCollider.isColliding && rightHandCollider.collidingObject != leftGrabbedObject)
            {
                if (rightHandCollider.collidingObject.GetComponent<DistributedGrabbable>().RequestGrab())
                {
                    // initial offset calculation
                    rightGrabbedObject = rightHandCollider.collidingObject;
                    rightOffsetMat = GetTransformationMatrix(rightHandCollider.transform, true).inverse *
                                     GetTransformationMatrix(rightGrabbedObject.transform, true);
                
                    rightGrabbedObject.GetComponent<MaterialHandler>().Grab(true);
                }
            }
            else if (rightGrabbedObject != null)
            {
                Matrix4x4 newTransform = GetTransformationMatrix(rightHandCollider.transform, true) * rightOffsetMat;

                rightGrabbedObject.transform.position = newTransform.GetColumn(3);
                rightGrabbedObject.transform.rotation = newTransform.rotation;
            }
        }
        else if(rightGrabbedObject != null)
        {
            rightGrabbedObject.GetComponent<MaterialHandler>().Grab(false);
            rightGrabbedObject.GetComponent<DistributedGrabbable>().Release();
            rightGrabbedObject = null;
            rightOffsetMat = Matrix4x4.identity;
        }
        
        if (leftHandGrab.action.IsPressed())
        {
            if (leftGrabbedObject == null && leftHandCollider.isColliding &&
                leftHandCollider.collidingObject != rightGrabbedObject)
            {
                if (leftHandCollider.collidingObject.GetComponent<DistributedGrabbable>().RequestGrab())
                {
                    // initial offset calculation
                    leftGrabbedObject = leftHandCollider.collidingObject;
                    leftOffsetMat = GetTransformationMatrix(leftHandCollider.transform, true).inverse *
                                    GetTransformationMatrix(leftGrabbedObject.transform, true);
                
                    leftGrabbedObject.GetComponent<MaterialHandler>().Grab(true);
                }
            }
            else if (leftGrabbedObject != null)
            {
                Matrix4x4 newTransform = GetTransformationMatrix(leftHandCollider.transform, true) * leftOffsetMat;

                leftGrabbedObject.transform.position = newTransform.GetColumn(3);
                leftGrabbedObject.transform.rotation = newTransform.rotation;
            }
        }
        else if(leftGrabbedObject != null)
        {
            leftGrabbedObject.GetComponent<MaterialHandler>().Grab(false);
            leftGrabbedObject.GetComponent<DistributedGrabbable>().Release();
            leftGrabbedObject = null;
            leftOffsetMat = Matrix4x4.identity;
        }
    }

    public Matrix4x4 GetTransformationMatrix(Transform t, bool world = true)
    {
        if (world)
        {
            return Matrix4x4.TRS(t.position, t.rotation, t.lossyScale);
        }
        else
        {
            return Matrix4x4.TRS(t.localPosition, t.localRotation, t.localScale);
        }
    }
    
    #endregion
}
