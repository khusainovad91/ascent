using System;
using System.Collections;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class ButtonPress : MonoBehaviour
{
    [SerializeField]
    private AudioClip _clickAudio;
    [SerializeField]
    private Sprite _clickedImage;
    private Button _button;
    [SerializeField]
    private AudioSource _audioSource;

    private void Awake()
    {
        if (_audioSource == null)
            _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.playOnAwake = false;
        _audioSource.outputAudioMixerGroup = MusicManager.Instance.MusicAudioMixer.FindMatchingGroups("Music")[0];
        _audioSource.resource = _clickAudio;
        _button = this.GetComponent<Button>();
        _button.onClick.AddListener(HandleClick);
    }

    private void HandleClick()
    {
        StartCoroutine(StartClickAnimation()); 
    }

    IEnumerator StartClickAnimation()
    {
        if (_clickedImage == null)
        {
            yield break;
        }

        var ImageBuffer = _button.image;
        _button.image.sprite = _clickedImage;
        yield return new WaitForSeconds(1f);
        _button.image = ImageBuffer;
    }

}
