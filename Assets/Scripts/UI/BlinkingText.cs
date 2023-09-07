using System.Collections;

using UnityEngine;
using UnityEngine.UI;

public class BlinkingText : MonoBehaviour
{
    [SerializeField] Text _text;
    [SerializeField] Color _blinkingColor = new Color(1.0f, 1.0f, 1.0f, 0.0f);
    [SerializeField] float _showTime = 0.1f;
    [SerializeField] float _hideTime = 0.1f;

    void Start()
    {
        if (_text == null)
        {
            _text = GetComponent<Text>();
        }

        if (_text != null)
        {
            StartCoroutine(Blink());
        }
    }

    IEnumerator Blink()
    {
        Color originalColor = _text.color;

        while (true)
        {
            yield return new WaitForSeconds(_hideTime);

            _text.color = _blinkingColor;

            yield return new WaitForSeconds(_showTime);

            _text.color = originalColor;
        }
    }
}