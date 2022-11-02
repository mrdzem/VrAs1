using System;
using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using UnityEngine;

public class MoveAroundTarget : MonoBehaviour
{
    public Transform target;

    public float degreesPerSecond = 20;

    private float timer = 0.0f;

    private Vector3 targetPositionXZ
    {
        get
        {
            return new Vector3(target.position.x, 0, target.position.z);
        }
    }

    private Vector3 positionXZ
    {
        get
        {
            return new Vector3(transform.position.x, 0, transform.position.z);
        }
    }

    private Vector3 directionToTarget
    {
        get
        {
            return (targetPositionXZ - positionXZ);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var newPosition = CalculatePositionUpdate();
        var newRotation = CalculateRotationUpdate(newPosition);
        
        transform.position = newPosition;
        transform.rotation = newRotation;
    }

    Vector3 CalculatePositionUpdate()
    {
        // TODO: Exercise 1.5

        timer += Time.deltaTime;
        Vector3 returnPosition = new Vector3(target.position.x + Mathf.Cos(timer / 60 * degreesPerSecond), 0, target.position.z + Mathf.Sin(timer / 60 * degreesPerSecond));
        return returnPosition;

       
    }

    Quaternion CalculateRotationUpdate(Vector3 newPosition)
    {
        // TODO: Exercise 1.5

        return Quaternion.LookRotation(newPosition - transform.position, Vector3.up);
    }
}
