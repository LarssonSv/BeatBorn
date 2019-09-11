using System.Collections;
using System.Collections.Generic;
using UnityEditor.VFX;
using UnityEngine;
using UnityEngine.Experimental.VFX;

public class ParticlePool : MonoBehaviour
{
    private static ParticlePool _instanceHelper;
    private static ParticlePool _instance
    {
        get
        {
            if (_instanceHelper == null)
                _instanceHelper = FindObjectOfType<ParticlePool>();
            if(_instanceHelper == null)
                _instanceHelper = new GameObject("ParticleHelper").AddComponent<ParticlePool>();

            return _instanceHelper;
        }
    }

    public static VisualEffect SpawnOneShot(GameObject visualEffectGraphPrefab, VisualEffectAsset visualAsset ,Vector3 posistion, Quaternion rotation, Transform parent = null)
    {
        VisualEffect vfx = ObjectPool.Instantiate(visualEffectGraphPrefab, posistion, rotation, parent).GetComponent<VisualEffect>();

        vfx.visualEffectAsset = visualAsset;
        
        _instance.StartCoroutine(PlayParticleOneShot(vfx));
        return vfx;
    }
    
    private static IEnumerator PlayParticleOneShot(VisualEffect vfx)
    {
        vfx.Play();

        yield return new WaitForSeconds(0.1f);
        
        while (vfx.aliveParticleCount > 0)
        {
            yield return null;
        }
        vfx.Stop();
        ObjectPool.Destroy(vfx.gameObject);
       
    }

    public static void SpawnOneShot(GameObject particlePrefab, Vector3 posistion, Quaternion rotation, Transform parent = null)
    {
       _instance.StartCoroutine(PlayParticleOneShot(ObjectPool.Instantiate(particlePrefab, posistion, rotation, parent).GetComponent<ParticleSystem>()))  ;
    }

    private static IEnumerator PlayParticleOneShot(ParticleSystem particleComponent)
    {     
        yield return new WaitForSeconds(0.1f);
        
        while (particleComponent.isPlaying)
        {
            yield return null;
        }

        ObjectPool.Destroy(particleComponent.gameObject);
       
    }
}
