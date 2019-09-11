using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.HDPipeline;
using UnityEngine.Rendering;

public class PostProcessingBeat : MonoBehaviour
{
    [SerializeField] private AnimationCurve _lerpInCurve;
    [SerializeField] private float maxBloom;
    [SerializeField] private float minBloom;
    [SerializeField] private float maxExposure;
    [SerializeField] private float minExposure;
    private VolumeProfile profile;
    private Bloom _bloomer;
    private Exposure _exposure;
    private bool _lerping;

    private float _startBloom;
    private Volume volume;
    
    // Start is called before the first frame update
    void Start()
    {
        volume = GetComponent<Volume>();
        volume.profile.TryGet(out _bloomer);
        volume.profile.TryGet(out _exposure);
        _startBloom = _bloomer.intensity.value;
        _lerping = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(PauseGame.IsPaused())
            return;
        
        _bloomer.intensity.value = Mathf.Lerp(minBloom, maxBloom, _lerpInCurve.Evaluate(BeatManager.GetTimeToQuarterNote()));
        _exposure.fixedExposure.value = Mathf.Lerp(minExposure, maxExposure, _lerpInCurve.Evaluate(BeatManager.GetTimeToQuarterNote()));
        
    }

    private void OnDisable()
    {
        Debug.Log("Hello stiop");
        _bloomer.intensity.value = _startBloom;
    }
}
