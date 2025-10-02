//JONATHAN'S LOGIC
//using System.Diagnostics;
//using UnityEngine;


//public class DamageZone : AbstractEffectZone
//{

//    //handles damage and healing
//    //to heal use positive values for "Damage"
//    //to damage use negative values for "Damage"
//    //naming should be more clear in the future

//    //[SerializeField] private float maxBoostMultiplier = 2f;
//    [SerializeField] private Rigidbody targetRb;
//    [SerializeField] private int persistentDamage = -5; //per second
//    [SerializeField] private int initialDamage = -50;
//    [SerializeField] private int emotionalDamage = -500;
//    //TODO: replace with our player or enemy health reference
//    //private EntityHealth targetHealth;

//    protected override void OnEnterEffect(Collider targetCollider)
//    {
//        if (hasAudio)
//        {
//            audioSource.Play();
//        }

//        UnityEngine.Debug.Log("target entered damage zone");
//        //targetHealth = targetCollider.gameObject.GetComponent<EntityHealth>();
//        //could get RB component as well if we want a knockback effect but we may want that added dynamically a number of zone types - may want to use a different pattern here
//        //targetHealth.Change(initialDamage);
//    }


//    protected override void OnStayEffect(Collider targetCollider)
//    {

//        UnityEngine.Debug.Log("Zone: Damage of " + persistentDamage + " Applied!");
//        //targetRb = targetCollider.gameObject.GetComponent<Rigidbody>();
//        //targetHealth = targetCollider.gameObject.GetComponent<EntityHealth>();
//        //targetHealth.Change(persistentDamage);
//        //apply damage to target
//        //apply damage over time if burn
//        //targetRb.AddForce(targetCollider.gameObject.transform.forward * boostForce, forceMode);

//    }

//    protected override void OnExitEffect(Collider other)
//    {
//        if (hasAudio)
//        {
//            audioSource.Stop();
//        }
//    }
//}
using System.Diagnostics;
using UnityEngine;

public class DamageZone : AbstractEffectZone
{
    //handles damage and healing
    //to heal use positive values for "Damage"
    //to damage use negative values for "Damage"

    [Header("Damage Settings")]
    [SerializeField] private int persistentDamage = 5; //per second (positive value)
    [SerializeField] private int initialDamage = 50; //(positive value)
    [SerializeField] private bool instantKill = true; //set to true for lava

    protected override void OnEnterEffect(Collider targetCollider)
    {
        if (hasAudio && audioSource != null)
        {
            audioSource.Play();
        }

        UnityEngine.Debug.Log("DamageZone: target entered damage zone - Tag: " + targetCollider.tag);

        // Check if PlayerHealthController exists
        if (PlayerHealthController.instance == null)
        {
            UnityEngine.Debug.LogError("DamageZone: PlayerHealthController.instance is NULL!");
            return;
        }

        if (instantKill)
        {
            // Instant death for lava - deal max health as damage
            UnityEngine.Debug.Log("DamageZone: Applying instant kill!");
            PlayerHealthController.instance.DamagePlayer(PlayerHealthController.instance.maxHealth);
        }
        else if (initialDamage > 0)
        {
            // Apply initial damage
            UnityEngine.Debug.Log("DamageZone: Applying initial damage of " + initialDamage);
            PlayerHealthController.instance.DamagePlayer(initialDamage);
        }
    }

    protected override void OnStayEffect(Collider targetCollider)
    {
        // Skip persistent damage if instant kill is enabled
        if (instantKill)
        {
            UnityEngine.Debug.Log("DamageZone: Instant kill mode - skipping persistent damage");
            return;
        }

        UnityEngine.Debug.Log("DamageZone: Applying persistent damage of " + persistentDamage);

        // Check if PlayerHealthController exists
        if (PlayerHealthController.instance == null)
        {
            UnityEngine.Debug.LogError("DamageZone: PlayerHealthController.instance is NULL!");
            return;
        }

        // Apply persistent damage
        if (persistentDamage > 0)
        {
            PlayerHealthController.instance.DamagePlayer(persistentDamage);
        }
    }

    protected override void OnExitEffect(Collider other)
    {
        if (hasAudio && audioSource != null)
        {
            audioSource.Stop();
        }

        UnityEngine.Debug.Log("DamageZone: target exited damage zone");
    }
}