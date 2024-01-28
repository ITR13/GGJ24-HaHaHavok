using DefaultNamespace;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private GameObject _disableThis;
    [SerializeField] private string _needsKey = "";

    [SerializeField] private string _onSucceedOpen = "";
    [SerializeField] private string _onFailOpen = "";

    private bool _open;
    
    public void TryOpen()
    {
        if(_open) return;
        
        if (!string.IsNullOrEmpty(_needsKey))
        {
            if (!FlagManager.Check(_needsKey))
            {
                ShowTextOnScreen.ShowText(_onFailOpen);
                return;
            }
        }

        ShowTextOnScreen.ShowText(_onSucceedOpen);
        _open = true;
        _disableThis.SetActive(false);
    }
}