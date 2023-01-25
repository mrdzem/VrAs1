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
            collidingObject.GetComponent<MaterialHandler>().Hover(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isColliding && other.gameObject == collidingObject)
        {
            collidingObject.GetComponent<MaterialHandler>().Hover(false);
            isColliding = false;
            collidingObject = null;
        }
    }

    #endregion
}
