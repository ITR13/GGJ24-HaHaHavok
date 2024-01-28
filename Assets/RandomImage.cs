using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomImage : MonoBehaviour
{
    [SerializeField] private Material _material;
    [SerializeField] private List<Texture2D> _textures;
    
    public void Start()
    {
        var r = Random.Range(0, _textures.Count);
        _material.mainTexture = _textures[r];
    }
}
