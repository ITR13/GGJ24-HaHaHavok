using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    [SerializeField] private Transform _toPosition;

    private void OnTriggerEnter(Collider other)
    {
        other.transform.position = _toPosition.position;
    }
}
