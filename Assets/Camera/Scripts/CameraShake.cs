using UnityEngine;

public class CameraShake: MonoBehaviour {
    private bool isCameraShaking = false;
    private float shakeStrength = 0.1f;
    private float shakeTimeHolder;

    private static CameraShake instance;

    private void Awake() {
        instance = this;
    }

    private void LateUpdate() {
        ShakeInternal();
#if UNITY_EDITOR
        if(Input.GetKeyDown(KeyCode.K)) {
            Shake();
        }

        if(Input.GetKeyDown(KeyCode.L)) {
            transform.position = transform.position + Shake2D();
        }
#endif
    }

    public static void Shake(float strength = 0.1f, float length = 0.1f) {
        if(instance.isCameraShaking) {
            return;
        }
        instance.isCameraShaking = true;
        instance.shakeStrength = strength;
        instance.shakeTimeHolder = Time.time + length;
    }

    private void ShakeInternal() {
        if(!isCameraShaking) {
            return;
        }

        if(shakeTimeHolder < Time.time) {
            isCameraShaking = false;
            transform.localPosition = new Vector3(0, 0, 0);
        }

        Vector3 shake = transform.position + Random.insideUnitSphere * shakeStrength;
        transform.position = shake;
    }

    public static Vector3 Shake2D(float amplitude = 1, float frequency = 0.98f, int octaves = 2, float persistance = 0.2f, float lacunarity = 20, float burstFrequency = 0.5f, int burstContrast = 2) {
        float valX = 0;
        float valY = 0;

        float iAmplitude = 1;
        float iFrequency = frequency;
        float maxAmplitude = 0;

        // Burst frequency
        float burstCoord = Time.time / (1 - burstFrequency);

        // Sample diagonally trough perlin noise
        float burstMultiplier = Mathf.PerlinNoise(burstCoord, burstCoord);

        //Apply contrast to the burst multiplier using power, it will make values stay close to zero and less often peak closer to 1
        burstMultiplier = Mathf.Pow(burstMultiplier, burstContrast);

        for(int i = 0; i < octaves; i++) // Iterate trough octaves
        {
            float noiseFrequency = Time.time / (1 - iFrequency) / 10;

            float perlinValueX = Mathf.PerlinNoise(noiseFrequency, 0.5f);
            float perlinValueY = Mathf.PerlinNoise(0.5f, noiseFrequency);

            // Adding small value To keep the average at 0 and   *2 - 1 to keep values between -1 and 1.
            perlinValueX = (perlinValueX + 0.0352f) * 2 - 1;
            perlinValueY = (perlinValueY + 0.0345f) * 2 - 1;

            valX += perlinValueX * iAmplitude;
            valY += perlinValueY * iAmplitude;

            // Keeping track of maximum amplitude for normalizing later
            maxAmplitude += iAmplitude;

            iAmplitude *= persistance;
            iFrequency *= lacunarity;
        }

        valX *= burstMultiplier;
        valY *= burstMultiplier;

        // normalize
        valX /= maxAmplitude;
        valY /= maxAmplitude;

        valX *= amplitude;
        valY *= amplitude;

        return new Vector3(valX, valY);
    }

    private void OnDisable() {
        instance = null;
    }
}