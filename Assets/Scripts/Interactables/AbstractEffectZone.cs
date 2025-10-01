using System.Collections.Generic;
using UnityEngine;
using static AbstractEffectZone;

public abstract class AbstractEffectZone : MonoBehaviour
{
    [Tooltip("")]
    public float effectStartDelay = 0; //time in s before effect begins
    public AnimationCurve effectCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);
    public float effectDuration = 0; //0 is instant
    public float effectInterval = 0; //0 does not repeat
    public float disableTimer = 0; //0 always active. Greater than 0 = time to re-activate before effect can recur
    private float nextEffectTime = 0f; //If effect persists over time this is the time that must pass before the affect is re-applied
    public List<string> affectedEntitiesByTag = new List<string> { "Player", "Enemy" };

    public float effectStartTimer = 0;
    public float effectDurationTimer = 0;
    private float nextEffectTimer = 0;
    private float lastEffectTime = 0;
    //need a reset timer

    protected AudioSource audioSource;
    protected bool hasAudio = true;

    public ForceMode forceMode = ForceMode.Force;

    public enum EffectMode
    {
        impulse = 0,
        animationCurve = 1
        //anticipating more types or would use a bool here
    }
    public EffectMode effectMode = EffectMode.impulse;

    public virtual void Start()
    {
        UnityEngine.Debug.Log("damage zone start - find audio source");
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            hasAudio = false;
            UnityEngine.Debug.Log("no audio source found on zone " + this.name);
        }
        else
        {
            UnityEngine.Debug.Log("set audio source to " + audioSource.name + " " + audioSource.clip);
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        UnityEngine.Debug.Log("zone trigger entered");
        if (IsEntityAffected(other))
        {
            UnityEngine.Debug.Log("zone: does it affect " + other.tag);
            OnEnterEffect(other);
            // next effect may no longer occur as we're now adding an initial effect
            UnityEngine.Debug.Log("zone Effect interval = " + effectInterval);
            if (effectInterval > 0)
            {
                UnityEngine.Debug.Log("zone init nextEffectTime");
                nextEffectTimer = effectInterval;
                effectDurationTimer = effectDuration;
            }
        }
    }

    protected virtual void OnTriggerStay(Collider other)
    {
        effectDurationTimer -= Time.fixedDeltaTime;
        nextEffectTime -= Time.fixedDeltaTime;
        UnityEngine.Debug.Log("zone: trigger stayed - effect timer = " + effectDurationTimer + " and nextEffectTime = " + nextEffectTime);
        if (effectDurationTimer > 0f && IsEntityAffected(other) && nextEffectTime <= 0) //TODO: this logic won't run a 'next effect'
        {
            UnityEngine.Debug.Log("zone: apply on stay effect");
            float timeInZone = Time.fixedDeltaTime - nextEffectTime + effectInterval;
            OnStayEffect(other);
            nextEffectTime = effectInterval;
        }

    }

    protected virtual void OnTriggerExit(Collider other)
    {
        UnityEngine.Debug.Log("zone trigger exited");
        if (IsEntityAffected(other))
        {
            OnExitEffect(other);
        }
    }

    private bool IsEntityAffected(Collider other)
    {
        UnityEngine.Debug.Log("zone: check if entity is affected " + other.tag);
        return affectedEntitiesByTag.Contains(other.tag);
    }

    protected float GetEffectCurveValue(float time)
    {
        return effectCurve.Evaluate(time);
    }

    protected abstract void OnEnterEffect(Collider other); //Apply effect should check the effectMode before applying its effect

    protected abstract void OnStayEffect(Collider other); //Apply effect should check the effectMode before applying its effect

    protected virtual void OnExitEffect(Collider other) { } //TODO: to really take advantage of this we probably need effects defined in individual classes so we can have an enter, stay, and exit effect
    // Optional cleanup logic when the entity leaves the zone.    

}