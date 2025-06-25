using UnityEngine;
using TMPro;

namespace TextEffects
{
    [RequireComponent(typeof(TMP_Text))]
    public class Scaling : MonoBehaviour
    {
        [SerializeField, Range(0, 1)] private float frequency;
        [SerializeField, Range(0, 1)] private float size;
        
        private RectTransform rect;

        void Awake()
        {
            rect = GetComponent<RectTransform>();
        }

        void Update()
        {
            rect.localScale = new Vector3(1 + Mathf.PingPong(Time.time * frequency, size), 1 + Mathf.PingPong(Time.time * frequency, size), 1 + Mathf.PingPong(Time.time * frequency, size));
        }
    }
}