using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class MaterialHandler : MonoBehaviour
{
    #region Member Variables

    private Renderer renderer;
    
    public Material defaultMaterial;
    public Material hoverMaterial;
    public Material grabMaterial;

    #endregion

    #region MonoBehaviour Callbacks

    private void Start()
    {
        renderer = GetComponent<Renderer>();

        if (defaultMaterial == null)
            defaultMaterial = renderer.material;
    }

    #endregion
    
    #region Custom Methods

    public void Hover(bool entered)
    {
        if (entered)
        {
            renderer.material = hoverMaterial;
        }
        else
        {
            renderer.material = defaultMaterial;
        }
    }

    public void Grab(bool entered)
    {
        if (entered)
        {
            renderer.material = grabMaterial;
        }
        else
        {
            renderer.material = hoverMaterial;
        }
    }

    #endregion
}
