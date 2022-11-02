using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentSwitcher : MonoBehaviour
{
    public List<Transform> parents = new List<Transform>();

    public KeyCode nextParentKey = KeyCode.RightArrow;

    private int currentParent = 0;

    // Start is called before the first frame update
    void Start()
    {
        SetParent(0);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(nextParentKey))
            SetParent((currentParent + 1) % parents.Count);
    }

    void SetParent(int idx)
    {
        // TODO: Exercise 1.4 -> 1.)
        //if (Event.current.Equals(Event.KeyboardEvent(nextParentKey.ToString())))
        //{
            this.gameObject.transform.SetParent(parents[idx]);
        //}
        

        // what is the effect of worldPositionStays?
        /*
        The default value of worldPositionStays argument is true, wich modifies the parent relative position, scale and rotation
        so that the objects keeps the same world space position, rotation and scale as before.
        

        If false, the child keeps its local orientation rather than its global orientation also moving 
        the GameObject to be positioned next to its new parent.

        

        */
    }
}
