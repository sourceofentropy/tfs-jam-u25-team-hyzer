using UnityEngine;
using UnityEngine.Rendering.Universal; // for Light2D

public class SimpleFlickerLight2D : MonoBehaviour
{
    [Header("Light State")]
    [SerializeField] private bool isOn = true;
    [SerializeField] private Light2D lightSource;
    [SerializeField] private Animator animator;

    [Header("Light Animation States (Optional)")]
    [SerializeField] private AnimationClip onAnimationClip;
    [SerializeField] private AnimationClip offAnimationClip;

    [Header("Base Settings")]
    [SerializeField] private float intensityBase = 1f;
    [SerializeField] private float radiusBase = 5f; // 2D lights use radius instead of range

    [Header("Fade Settings")]
    [SerializeField] private float fadeSpeed = 3f;

    [Header("Flicker Settings")]
    [Tooltip("Set variation to 0 for no flicker.")]
    [SerializeField] private float intensityVariation = 0.2f;
    [SerializeField] private float radiusVariation = 0.5f;
    [SerializeField] private float flickerSpeed = 2f;

    private float targetIntensity;
    private float targetRadius;
    private float noiseSeed;

    void Awake()
    {
        if (lightSource == null)
            lightSource = GetComponent<Light2D>();
        if (animator == null)
            animator = GetComponent<Animator>();

        noiseSeed = Random.Range(0f, 100f);
        UpdateLightState();
    }

    void Update()
    {
        if (lightSource == null) return;

        // Smoothly fade to target values
        lightSource.intensity = Mathf.MoveTowards(lightSource.intensity, targetIntensity, Time.deltaTime * fadeSpeed);
        lightSource.pointLightOuterRadius = Mathf.MoveTowards(lightSource.pointLightOuterRadius, targetRadius, Time.deltaTime * fadeSpeed);

        if (isOn)
            UpdateFlicker();
    }

    private void UpdateLightState()
    {
        if (animator != null)
        {
            if (isOn && onAnimationClip != null)
                animator.Play(onAnimationClip.name);
            else if (!isOn && offAnimationClip != null)
                animator.Play(offAnimationClip.name);
        }

        if (!isOn)
        {
            targetIntensity = 0f;
            targetRadius = 0f;
        }
        else
        {
            targetIntensity = intensityBase;
            targetRadius = radiusBase;
        }
    }

    private void UpdateFlicker()
    {
        if (intensityVariation <= 0f && radiusVariation <= 0f)
            return;

        float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, noiseSeed);
        float intensityFlicker = Mathf.Lerp(-intensityVariation, intensityVariation, noise);
        float radiusFlicker = Mathf.Lerp(-radiusVariation, radiusVariation, noise);

        targetIntensity = intensityBase + intensityFlicker;
        targetRadius = radiusBase + radiusFlicker;
    }

    public void TurnOn()
    {
        isOn = true;
        UpdateLightState();
    }

    public void TurnOff()
    {
        isOn = false;
        UpdateLightState();
    }

    public void SetLightState(bool state)
    {
        isOn = state;
        UpdateLightState();
    }
}
