using DefaultNamespace;
using UnityEngine;

public class FlagStorage : MonoBehaviour
{
    [SerializeField] private string flagName;

    public void setFlag() => FlagManager.Set(flagName);

    // Update is called once per frame
    void Update()
    {
        
    }
}
