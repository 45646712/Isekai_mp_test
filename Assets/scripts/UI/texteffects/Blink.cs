using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TextEffects
{
    [RequireComponent(typeof(TMP_Text))]
    public class Blink : MonoBehaviour
    {
        [SerializeField, Range(0, 2)] private float frequency;
        
        private TMP_Text text;

        void Awake()
        {
            text = GetComponent<TMP_Text>();
        }

        void Update()
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, Mathf.PingPong(Time.time * frequency, 1));
        }
    }
}