using System.Diagnostics;
using UnityEngine;


public class DamageZone : AbstractEffectZone
{

    //handles damage and healing
    //to heal use positive values for "Damage"
    //to damage use negative values for "Damage"
    //naming should be more clear in the future

    //[SerializeField] private float maxBoostMultiplier = 2f;
    [SerializeField] private Rigidbody targetRb;
    [SerializeField] private int persistentDamage = -5; //per second
    [SerializeField] private int initialDamage = -50;
    [SerializeField] private int emotionalDamage = -500;
    //TODO: replace with our player or enemy health reference
    //private EntityHealth targetHealth;

    protected override void OnEnterEffect(Collider targetCollider)
    {
        if (hasAudio)
        {
            audioSource.Play();
        }

        UnityEngine.Debug.Log("target entered damage zone");
        //targetHealth = targetCollider.gameObject.GetComponent<EntityHealth>();
        //could get RB component as well if we want a knockback effect but we may want that added dynamically a number of zone types - may want to use a different pattern here
        //targetHealth.Change(initialDamage);
    }


    protected override void OnStayEffect(Collider targetCollider)
    {
        
        UnityEngine.Debug.Log("Zone: Damage of " + persistentDamage + " Applied!");
        //targetRb = targetCollider.gameObject.GetComponent<Rigidbody>();
        //targetHealth = targetCollider.gameObject.GetComponent<EntityHealth>();
        //targetHealth.Change(persistentDamage);
        //apply damage to target
        //apply damage over time if burn
        //targetRb.AddForce(targetCollider.gameObject.transform.forward * boostForce, forceMode);

    }

    protected override void OnExitEffect(Collider other)
    {
        if (hasAudio)
        {
            audioSource.Stop();
        }
    }
}