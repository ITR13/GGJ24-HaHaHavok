using UnityEngine;

namespace DefaultNamespace
{
    public class Distractable : MonoBehaviour
    {
        [field: SerializeField] public Waypoint ClosestWaypoint { get; private set; }
        [SerializeField] private GameObject _interactable, _distractable;

        public void SetState(bool activated)
        {
            if (_interactable)
            {
                _interactable.SetActive(!activated);
            }
            else
            {
                _distractable.SetActive(activated);
            }
        }
    }
}