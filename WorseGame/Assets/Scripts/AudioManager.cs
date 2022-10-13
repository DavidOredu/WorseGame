using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using MoreMountains.Feedbacks;
using UnityEngine.UI;

public class AudioManager : Singleton<AudioManager>, IPointerEnterHandler, IPointerClickHandler
{
    public GameObject soundPrefab;
    private List<GameObject> soundObjects = new List<GameObject>();
    public Sound[] sounds;
    public SettingsData gameSettings;

    public AudioClip uiClip;
    public Sound currentMusic;
    public float nextSongDelay = 1f;

    public List<MMFeedbacks> feedbackIconSounds;
    public List<MMFeedbacks> feedbackUISounds;
	
	private List<Sound> musicList;
    // Start is called before the first frame update
    public override void Awake()
    {
        base.Awake();
        gameSettings = Resources.Load<SettingsData>("SettingsData");
        foreach (var sound in sounds)
        {
            switch (sound.soundType)
            {
                case SoundType.Music:
              //      sound.loop = true;
                    break;
                case SoundType.SFX:
                    break;
                default:
                    break;
            }
        }
        foreach (Sound sound in sounds)
        {
            var obj = Instantiate(soundPrefab, transform);
            var soundSource = obj.GetComponent<AudioSource>();
            sound.source = soundSource;
            sound.source.clip = sound.clip;
            sound.source.loop = sound.loop;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            if(sound.soundType == SoundType.Music)
                obj.AddComponent<MMAudioSourcePitchShaker>().AlwaysResetTargetValuesAfterShake = true;
        }
		
        foreach (var sound in sounds)
        {
            if (sound.soundType == SoundType.Music)
                musicList.Add(sound);
        }
    }
    public void SetFeedbackSounds(bool firstTime)
    {
        foreach (var sound in feedbackUISounds)
        {
            MMFeedbackSound soundFeedback = null;
            soundFeedback = sound.gameObject.GetComponent<MMFeedbackSound>();
            if (soundFeedback == null)
            {
                soundFeedback = sound.gameObject.AddComponent<MMFeedbackSound>();
                sound.Feedbacks.Add(soundFeedback);
            }
            var button = sound.gameObject.GetComponent<Button>();
            var toggle = sound.gameObject.GetComponent<Toggle>();

            if (firstTime)
            {
                if (button)
                    button.onClick.AddListener(() => sound.PlayFeedbacks());
                if (toggle)
                    toggle.onValueChanged.AddListener((x) => sound.PlayFeedbacks());
            }
            soundFeedback.Sfx = uiClip;
            soundFeedback.Active = gameSettings.sound;
        }
        foreach (var sound in feedbackIconSounds)
        {
            var soundFeedback = sound.gameObject.GetComponent<MMFeedbackSound>();
            soundFeedback.Active = gameSettings.sound;
        }
    }
    private void Start()
    {
        SetFeedbackSounds(true);

        PlayRandomMusic();
        if (!gameSettings.music)
            currentMusic.source.Stop();
    }

    private void Update()
    {
            foreach (var sound in sounds)
            {
                switch (sound.soundType)
                {
                    case SoundType.Music:
                        break;
                    case SoundType.SFX:
                        break;
                   default:
                       break;
                }
            }
        foreach (Sound sound in sounds)
        {
            sound.source.clip = sound.clip;
            sound.source.loop = sound.loop;
        }
        if (!currentMusic.source.isPlaying && gameSettings.music)
            PlayRandomMusic(true, nextSongDelay);
    }
    // Update is called once per frame
    public void PlayRandomMusic(bool delayed = false, float delayTime = .1f)
    {
        var randomSound = musicList[UnityEngine.Random.Range(0, musicList.Count)];
        currentMusic = randomSound;
        if (delayed)
            PlayDelayedSound(randomSound.SoundName, delayTime);
        else
            PlaySound(randomSound.SoundName);
    }
    public void PlaySound(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.SoundName == name);
        if (s == null)
        {
            Debug.LogError("Sound: " + name + " not found!");
            return;
        }

            //if (UIManager.GameIsPaused)
            //{
            //    // play pause sound
            //    s.source.Pause();
            //}
        
         
        s.source.Play();
    }
    public void PlaySound(int index)
    {
        if(sounds.Length == 0) { return; }

        Sound s = sounds[index];
        s.source.Play();
    }
    public void PlayDelayedSound(string name, float delayTime)
    {
        Sound s = Array.Find(sounds, sound => sound.SoundName == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        if (UIManager.GameIsPaused)
        {
            // play pause sound
            s.source.Pause();
        }
        s.source.PlayDelayed(delayTime);
    }
    public void PlayOneShotSound(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.SoundName == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        if (UIManager.GameIsPaused)
        {
            // play pause sound
            s.source.Pause();
        }
        s.source.PlayOneShot(s.clip);
    }
    public Sound FindSound(string name)
    {
        foreach (Sound s in sounds)
        {
            if (s.SoundName == name)
            {
                return s;
            }
            else
            {
                continue;
            }
        }
        Debug.LogError("Given name does not correspond to any sound name existing in the sound array. Make sure the spelling corresponds to the required sound name or add a sound to fit that name.");
        return null;
    }
    public enum SoundType
    {
        Music,
        SFX,
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        throw new NotImplementedException();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        throw new NotImplementedException();
    }
}
