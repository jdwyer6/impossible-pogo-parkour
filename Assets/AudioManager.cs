using UnityEngine;
using UnityEngine.Audio;
using System;

public class AudioManager : MonoBehaviour
{

    public Sound[] sounds;
    public static AudioManager instance;

    public string[] soundtrackClips;
    public int currentSoundtrack = 0;
    bool incrememtingSoundtrack = false;

    // Start is called before the first frame update
    void Awake()
    {
        
        if(instance == null){
            instance = this;
            DontDestroyOnLoad(gameObject);
        }else{
            Destroy(gameObject);
            return;
        }
        
        

        foreach (Sound s in sounds){
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    private void Start() {
        // this.Play(soundtrackClips[currentSoundtrack]);    
    }

    private void Update() {

        // if (!AudioManager.instance.sounds[currentSoundtrack].source.isPlaying) {
        //     incrememtingSoundtrack = true;
        //     incrememtSoundtrack();
        // }

    }

    public void Play(string name){      
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if(s == null){
            Debug.LogWarning("Sound: " + s.name + "not found");
            return;
        }
        s.source.Play();
    }

    public void Stop(string name){
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if(s == null){
            Debug.LogWarning("Sound: " + s.name + "not found");
            return;
        }
        s.source.Stop();
    }

    public void incrememtSoundtrack() {
        if(incrememtingSoundtrack){
            currentSoundtrack += 1;
            this.Play(soundtrackClips[currentSoundtrack]);  
            incrememtingSoundtrack = false;
        }
    }
}
