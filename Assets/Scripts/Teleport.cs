using UnityEngine;
using UnityEngine.SceneManagement;

public class Teleport : MonoBehaviour
{
    [SerializeField] private Transform _toPosition;

    private void OnTriggerEnter(Collider other)
    {
        if (_toPosition == null)
        {
            SceneManager.LoadScene(1);
            return;
        }

        other.transform.position = _toPosition.position;
    }
}