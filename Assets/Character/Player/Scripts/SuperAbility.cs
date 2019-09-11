using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;

public class SuperAbility : MonoBehaviour
{
    [SerializeField] private string prefabTag = "VFXUlti";
    private PlayerSuperState.SuperStruct super;
    private List<Transform> hitObjects = new List<Transform>();
    private List<IDamageable> toTakeDamage = new List<IDamageable>();
    private float currentRadius => super.minMaxRadius.x + (super.minMaxRadius.y - super.minMaxRadius.x) * timer / super.AttackTime;

    private float timer;
    private IDamageable currentDamageable;
    private bool setup = false;
    private ObjectPooler OP = ObjectPooler.Instance;
    Collider[] hits;

    public void Initialize(PlayerSuperState.SuperStruct newSuper, Vector3 pos)
    {
        toTakeDamage.Clear();
        transform.position = pos;
        hitObjects.Clear();
        timer = 0;
        super = newSuper;
        setup = true;
        hits = Physics.OverlapSphere(transform.position, currentRadius, super.mask);
    }

    private void Update()
    {
        if (!setup)
            return;

        if (timer > super.AttackTime)
        {
            foreach(IDamageable x in toTakeDamage)
            {
                x.TakeDamage(super.Damage);
            }
            gameObject.transform.parent = null;
            gameObject.SetActive(false);

            
        }

        Debug.Log("HEH");
            

        foreach (Collider hit in hits)
        {
            if (hitObjects.Contains(hit.transform.root))
                continue;

            currentDamageable = hit.transform.root.GetComponent<IDamageable>();
            if (currentDamageable != null)
            {
               OP.SpawnFromPool(prefabTag, hit.transform.root.position, hit.transform.rotation, hit.transform.root);
                toTakeDamage.Add(currentDamageable);
                super.HitParticle.Play(hit.transform.position, Quaternion.identity);

            }
            
            RuntimeManager.PlayOneShotAttached(super.HitSFX, gameObject);
            hitObjects.Add(hit.transform.root);
            currentDamageable = null;
        }

        timer += Time.deltaTime;
    }



}
