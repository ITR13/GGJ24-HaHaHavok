using UnityEngine;

public class TriggerExample : MonoBehaviour
{
    [SerializeField]
    private GameObject targetGameObject;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            // Set the serialized field gameobject to active
            if (targetGameObject != null)
            {
                targetGameObject.SetActive(true);
            }
        }
    }
}
