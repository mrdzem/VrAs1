using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AvatarBodySelection : MonoBehaviour
{
    public List<GameObject> bodyPrefabs = new List<GameObject>();

    public InputActionProperty switchBodyAction;

    public LayerMask mirrorLayer;

    public Transform headTransform;

    private GameObject bodyInstance;

    private int currentBodyIndex = 0;

    private bool disableInputHandling = false;

    // Start is called before the first frame update
    void Start()
    {
        AttachBodyPrefab(0);
    }

    // Update is called once per frame
    void Update()
    {
        int nextBodyIndex = CalcNextBodyIndex();
        if(nextBodyIndex != -1 && IsLookingAtMirror())
            AttachBodyPrefab(nextBodyIndex);
    }

    int CalcNextBodyIndex() // -1 means invalid aka "do nothing"
    {
        var joystickValue = switchBodyAction.action.ReadValue<Vector2>();
        if (Mathf.Abs(joystickValue.x) > 0.5 && !disableInputHandling)
        {
            int nextBodyIndex = (currentBodyIndex + (joystickValue.x > 0 ? 1 : -1));
            if (nextBodyIndex < 0)
                nextBodyIndex += bodyPrefabs.Count;
            else
                nextBodyIndex %= bodyPrefabs.Count;
            disableInputHandling = true;
            return nextBodyIndex;
        }
        else if (Mathf.Abs(joystickValue.x) < 0.5 && disableInputHandling)
        {
            disableInputHandling = false;
        }
        return -1;
    }

    bool IsLookingAtMirror()
    {
        return Physics.Raycast(headTransform.position, headTransform.forward, 10, mirrorLayer);
    }

    private void AttachBodyPrefab(int index)
    {
        if(index < 0 || index >= bodyPrefabs.Count)
        {
            Debug.LogWarning("Body selection offers no prefab at given index '" + index + "'");
            return;
        }

        if(bodyInstance != null)
        {
            Destroy(bodyInstance);
            bodyInstance = null;
        }

        currentBodyIndex = index;
        bodyInstance = Instantiate(bodyPrefabs[currentBodyIndex], transform);
    }
}
