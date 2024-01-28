using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetScript : MonoBehaviour
{
    public bool AlsoSpace;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) || (AlsoSpace && Input.GetKeyDown(KeyCode.Space)))
        {
            SceneManager.LoadScene(sceneBuildIndex: 0);
        }
    }
}