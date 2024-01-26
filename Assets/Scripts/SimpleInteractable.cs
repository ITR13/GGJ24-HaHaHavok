using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleInteractable : MonoBehaviour
{
    public bool IsEnabled;
    
    public void Enable()
    {
        IsEnabled = true;
    }
}
