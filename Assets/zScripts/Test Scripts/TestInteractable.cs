using System;
using System.Collections;
using UnityEngine;

public class TestInteractable : Interaction
{
    public override void OnFocus()
    {
        Debug.Log("Looking at" + gameObject);
        // throw new NotImplementedException();
    }

    public override void OnInteract()
    {
        Debug.Log("Clicked On" + gameObject);
        // throw new NotImplementedException();
    }

    public override void OnLoseFocus()
    {
        Debug.Log("Stopped Looking at" + gameObject);
        // throw new NotImplementedException();
    }


}
