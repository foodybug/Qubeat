using System.Collections;

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class DisplayBGM : MonoBehaviour
{
    Text _text;

    void Start()
    {
        _text = GetComponent<Text>();
        _text.text = "";
    }

    void Update()
    {
        if (SoundManager.Instance == null)
        {
            return;
        }

        if (_text.text != SoundManager.Instance.currentMusicName)
        {
            _text.text = SoundManager.Instance.currentMusicName;
        }
    }
}