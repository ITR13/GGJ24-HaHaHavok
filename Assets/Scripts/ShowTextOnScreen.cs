using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    public class ShowTextOnScreen : MonoBehaviour
    {
        private static ShowTextOnScreen _instance;

        [SerializeField] public TextMeshProUGUI _text;
        [SerializeField] public CanvasGroup _canvasGroup;


        private float _secondsVisible;
        private float _state;

        private void Awake()
        {
            _instance = this;
        }

        public static void ShowText(string text, float time = 3)
        {
            if (_instance == null) return;
            _instance._text.text = text;
            _instance._secondsVisible = time;
        }


        private void Update()
        {
            switch (_secondsVisible)
            {
                case > 0 when _state < 1:
                    _state = Mathf.MoveTowards(_state, 1, Time.deltaTime * 3);
                    _canvasGroup.alpha = _state;
                    return;
                case <= 0 when _state > 0:
                    _state = Mathf.MoveTowards(_state, 0, Time.deltaTime * 3);
                    _canvasGroup.alpha = _state;
                    return;
                default:
                    _secondsVisible -= Time.deltaTime;
                    break;
            }
        }
    }
}