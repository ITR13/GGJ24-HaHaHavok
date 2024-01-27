using UnityEngine;

public class Pickup : MonoBehaviour
{
    [field: SerializeField] public GameObject Prefab { get; private set; }
    
    public void Take()
    {
        PickupController.Take(this);
    }
}
