using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
//using UnityEngine.Rendering.PostProcessing;
public class PostProcessingHandler : Singleton<PostProcessingHandler>
{
    public Volume volume;
    public Vignette vignette;
    public Bloom bloom;
    public ChannelMixer channelMixer;
    public ChromaticAberration chromaticAberration;
    public FilmGrain filmGrain;
    public MotionBlur motionBlur;

    public SettingsData settingsData;
    // Start is called before the first frame update
    void Start()
    {
        volume.profile.TryGet(out bloom);
        volume.profile.TryGet(out filmGrain);
        volume.profile.TryGet(out motionBlur);

        SetPostProcessing();
    }
    public void SetPostProcessing()
    {
        bloom.active = settingsData.bloom;
        filmGrain.active = settingsData.filmGrain;
        motionBlur.active = settingsData.motionBlur;
    }
    void Update()
    {
     
    }
}
