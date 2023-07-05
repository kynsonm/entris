using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] List<AudioClip> music;
    [SerializeField] AudioClip tap;
    [SerializeField] AudioClip tap_nothing;
    [SerializeField] AudioClip menu_move;
    [SerializeField] AudioClip menu_drag;

    // Start is called before the first frame update
    void Awake()
    {
        if (GameObject.Find("Music Player") == null) {
            CreateMusicObject();
        } 
    }

    public void Play(int soundClip) {
        switch (soundClip) {
            case 0: Play(tap); break;
            case 1: Play(tap_nothing); break;
            case 2: Play(menu_move); break;
            case 3: Play(menu_drag); break;
            default:
                Debug.LogWarning("No sound clip that corresponds to int " + soundClip + " - Nothing will be played");
                break;
        }
    }
    public void Play(AudioClip clip) {
        GameObject audioObject = new GameObject("Sound " + clip.name);
        GameObject.DontDestroyOnLoad(audioObject);
        AudioSource audio = audioObject.AddComponent<AudioSource>();

        audio.clip = clip;
        audio.Play();
        LeanTween.value(0f, 1f, /* 1.1f * */ clip.length)
        .setOnComplete(() => {
            GameObject.Destroy(audioObject);
        });
    }

    // Create music that plays forever
    void CreateMusicObject() {
        if (music == null || music.Count == 0) { return; }

        GameObject musicPlayer = new GameObject("Music Player");
        GameObject.DontDestroyOnLoad(musicPlayer);

        AudioSource audio = musicPlayer.AddComponent<AudioSource>();
        audio.clip = music[0];
        audio.loop = true;
        audio.Play();
    }
}
