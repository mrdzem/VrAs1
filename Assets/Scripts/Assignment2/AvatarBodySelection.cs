using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AvatarBodySelection : MonoBehaviour
{
    public List<GameObject> bodyPrefabs = new List<GameObject>();

    public InputActionProperty switchBodyAction; //Joystick LR

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
        if (switchBodyAction.action.IsPressed())
        {
            print("is working?");
        }


            int nextBodyIndex = CalcNextBodyIndex();
        if(nextBodyIndex != -1 && IsLookingAtMirror())
            AttachBodyPrefab(nextBodyIndex);
    }

    int CalcNextBodyIndex() // -1 means invalid aka "do nothing"
    {
        if (switchBodyAction.action.IsPressed())
        {
            currentBodyIndex++;
            Debug.Log(currentBodyIndex % 4);
            print("yay");
        }
        
        return currentBodyIndex % bodyPrefabs.Count;
    }

    bool IsLookingAtMirror()
    {
        return false;
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
