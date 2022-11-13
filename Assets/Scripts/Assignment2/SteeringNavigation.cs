using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SteeringNavigation : MonoBehaviour
{
    public InputActionProperty flyAction;

    public Transform directionIndicator;

    public float speedFactor = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(flyAction.action.IsPressed())
            transform.position += directionIndicator.forward * flyAction.action.ReadValue<float>() * Time.deltaTime * speedFactor;
    }
}


/*

*Controller Directed
Advantages:     Bigger flexibility, do not need to move body as much to pick a direction.
Disadvantages:  If you accidently switch controllers you have no idea what is going on, you screwed man.

*Gaze directed
Advantages:     Feels more realistic and natural, like a fish.
Disadvantages:  You cannot look around while walking because you switch direction. You can get dizzy more easily.

*Conslusion:     We pick controller directed movement, because we like looking around while walking.

*/