using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(VolumeController))]
public class MusicManager : PersistentSingleton<MusicManager>
{
    [SerializeField]
    private List<AudioResource> _tavernList;
    [SerializeField]
    private List<AudioResource> _battleList;
    [SerializeField]
    public AudioMixer MusicAudioMixer;
    private AudioSource _currenTrack;

    protected override void Awake()
    {
        base.Awake();
        _currenTrack = gameObject.AddComponent<AudioSource>();
        _currenTrack.outputAudioMixerGroup = MusicAudioMixer.FindMatchingGroups("Music")[0];
        _currenTrack.loop = false;
        _currenTrack.playOnAwake = false;
    }

    void Start()
    {
        StartCoroutine(StartMusic());
    }

    IEnumerator StartMusic()
    {
        var vc = GetComponent<VolumeController>();
        vc.SetMusicVolume(0f);
        PlayMusic(_tavernList);

        // плавно поднимаем громкость
        float duration = 1f;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float v = Mathf.Lerp(0f, 0.25f, t / duration);
            vc.SetMusicVolume(v);
            yield return null;
        }
        vc.SetMusicVolume(0.2f);
    }

    void Update()
    {
        if (!_currenTrack.isPlaying)
        {
            if (SceneManager.GetActiveScene().name == "BarScene1")
            {
                PlayMusic(_battleList);
            }
            else
            {
                PlayMusic(_tavernList);
            }

        }
    }

    private IEnumerator SetStartValue() {
        yield return new WaitForNextFrameUnit();
        this.GetComponent<VolumeController>().SetMusicVolume(0.5f);
    }

    private void PlayMusic(List<AudioResource> musicList)
    {
        _currenTrack.resource = GetRandomMusic(musicList);
        _currenTrack.Play();
    }

    private AudioResource GetRandomMusic(List<AudioResource> musicList) 
    {
        return musicList[Random.Range(0, musicList.Count)];
    }

    void OnEnable()
    {
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        if (newScene.name == "BarScene1")
        {
            _currenTrack.Stop();
            PlayMusic(_battleList);
        }
        else
        {
            _currenTrack.Stop();
            PlayMusic(_tavernList);
        }
    }
}
