#pragma warning disable 0649
using UnityEngine;
using System.Collections;

public class ShooterProjectile : MonoBehaviour
{
    [Header("Values:")]
    [SerializeField] public int Damage = 20;
    [SerializeField] private float speed = 20f;

    [Header("Setup:")]
    [SerializeField] private float radius;
    [SerializeField] private LayerMask nexusMask;
    [SerializeField] private Transform trail;
    [SerializeField] private float timeTrailFadeOut = 0.5f;

    private bool move = false;
    private bool notPlayed = false;



    private void Update()
    {
        if (move)
            transform.Translate(Vector3.forward * Time.deltaTime * speed);

        if (Physics.CheckSphere(transform.position, radius, nexusMask, QueryTriggerInteraction.Collide))
        {
            if (!notPlayed)
            {
                CoreHealth.Instance.TakeDamage(Damage);
                move = false;
                notPlayed = true;
                StartCoroutine(ScaleDownTrail());
            }
          
        }

    }

    public void ActivatThis()
    {
        notPlayed = false;
        move = true;
        trail.localScale = Vector3.one;
    }

    private IEnumerator ScaleDownTrail()
    {
        float timer = 0;

        while(timer < timeTrailFadeOut)
        {

            timer += Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(false);

    }


}