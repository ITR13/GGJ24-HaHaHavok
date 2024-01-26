using UnityEngine;

namespace DefaultNamespace
{
    public class Distractable : MonoBehaviour
    {
        [field: SerializeField] public Waypoint ClosestWaypoint { get; private set; }
        [SerializeField] private GameObject _interactable, _distractable;

        public void SetState(bool activated)
        {
            _interactable.SetActive(!activated);
            _distractable.SetActive(activated);
        }
    }
}