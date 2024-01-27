using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Unity.VisualScripting;
using UnityEngine;

public class BabyPickup : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private Distractable _distraction;
    private bool _thrown;
    private bool _destroyed;
    
    private void Update()
    {
        if(_thrown) return;
        if(!Input.GetKeyDown(KeyCode.B)) return;
        _thrown = true;
        transform.parent = null;
        _rigidbody.isKinematic = false;
        _rigidbody.AddForce(transform.forward * 12, ForceMode.Impulse);
        _distraction.SetState(true);
    }

    private void OnCollisionEnter(Collision other)
    {
        //if((_layerMask.value & other.gameObject.layer) == 0) return;
        if(!_thrown || _destroyed) return;
        _destroyed = true;
        Destroy(gameObject, 1.5f);
    }
}
