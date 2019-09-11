using UnityEngine;
using UnityEngine.Experimental.Rendering.HDPipeline;

public class PulsatingLights : MonoBehaviour
{
    [SerializeField] private AnimationCurve lightCurve;
    [SerializeField] private Light[] lights;

    [SerializeField] private AnimationCurve lightAngleCurve;

    [SerializeField] private AnimationCurve materialCurve;
    [SerializeField] private Material[] mats;

    private HDAdditionalLightData[] lightData;
    private float[] lightAngle;
    private float[] lightForce;
    readonly private float one = 1f;
    readonly private string materailref = "Vector1_150AE1D0";


    private void Start()
    {
        lightData = new HDAdditionalLightData[lights.Length];
        lightAngle = new float[lights.Length];
        for (int i = 0; i < lights.Length; i++)
        {
            lightAngle[i] = lights[i].spotAngle;
            lightData[i] = lights[i].GetComponent<HDAdditionalLightData>();
        }

    }

    void Update()
    {
        for (int i = 0; i < lights.Length; i++)
        {
            lights[i].GetComponent<HDAdditionalLightData>().lightDimmer = lightCurve.Evaluate((one - (float)BeatManager.OffBeatPercent()));
         //   lights[i].spotAngle = lightAngle[i] * lightAngleCurve.Evaluate((one - (float)BeatManager.OffBeatPercent()));


        }

        foreach (Material mat in mats)
        {
            mat.SetFloat(materailref, materialCurve.Evaluate((one - (float)BeatManager.OffBeatPercent())));
        }
        

    }
}
