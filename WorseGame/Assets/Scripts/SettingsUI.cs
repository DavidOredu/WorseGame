using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using TMPro;
using MoreMountains.Feedbacks;

public class SettingsUI : MonoBehaviour
{
    public List<UniversalRenderPipelineAsset> graphics = new List<UniversalRenderPipelineAsset>();
    public TMP_Dropdown graphicsDropdown;
    public Toggle motionBlurToggle;
    public Toggle filmGrainToggle;
    public Toggle bloomToggle;
    public Toggle musicToggle;
    public Toggle sfxToggle;

    public SettingsData settingsData;
   
    // Start is called before the first frame update
    void Start()
    {
        graphicsDropdown.value = graphics.IndexOf(settingsData.pipelineSetting);
        bloomToggle.isOn = settingsData.bloom;
        filmGrainToggle.isOn = settingsData.filmGrain;
        motionBlurToggle.isOn = settingsData.motionBlur;
        musicToggle.isOn = settingsData.music;
        sfxToggle.isOn = settingsData.sound;

    }
    // Update is called once per frame
    void Update()
    {

    }
    public void SetGraphics(int index)
    {
        settingsData.pipelineSetting = graphics[index];
        GameManager.instance.SetGraphics();
        GameSaveManager.instance.SaveAll();
    }
    public void SetBloom(bool active)
    {
        settingsData.bloom = active;
        PostProcessingHandler.instance.SetPostProcessing();
        GameSaveManager.instance.SaveAll();
    }
    public void SetFilmGrain(bool active)
    {
        settingsData.filmGrain = active;
        PostProcessingHandler.instance.SetPostProcessing();
        GameSaveManager.instance.SaveAll();
    }
    public void SetMotionBlur(bool active)
    {
        settingsData.motionBlur = active;
        PostProcessingHandler.instance.SetPostProcessing();
        GameSaveManager.instance.SaveAll();
    }
    public void SetMusic(bool active)
    {
        settingsData.music = active;
        if (!active)
            AudioManager.instance.currentMusic.source.Stop();
        GameSaveManager.instance.SaveAll();
    }
    public void SetSFX(bool active)
    {
        settingsData.sound = active;
        AudioManager.instance.SetFeedbackSounds(false);
        GameSaveManager.instance.SaveAll();
    }
}
