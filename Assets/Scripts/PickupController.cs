using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PickupController : MonoBehaviour
{
    private static PickupController _instance;

    private void Awake()
    {
        _instance = this;
    }

    public static void Take(Pickup pickup)
    {
        var instance = Instantiate(pickup.Prefab, _instance.transform);
        pickup.gameObject.SetActive(false);
        
        instance.transform.localPosition = Vector3.zero;
        instance.transform.localRotation = Quaternion.identity;
    }
}