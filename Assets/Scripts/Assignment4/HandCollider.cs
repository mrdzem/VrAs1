using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HandCollider : MonoBehaviour
{
    #region Member Variables

    public bool isColliding = false;
    public GameObject collidingObject;

    #endregion

    #region MonoBehaviour Callbacks

    private void OnTriggerEnter(Collider other)
    {
        if (!isColliding)
        {
            isColliding = true;
            collidingObject = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isColliding && other.gameObject == collidingObject)
        {
            isColliding = false;
            collidingObject = null;
        }
    }

    #endregion
}
