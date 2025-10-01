using UnityEngine;

[RequireComponent(typeof(Light))]
public class SimpleFlickerLight : MonoBehaviour
{
    [Header("Light State")]
    [SerializeField] private bool isOn = true;
    [SerializeField] private Light lightSource;
    [SerializeField] private Animator animator;

    [Header("Light Animation States (Optional)")]
    [SerializeField] private AnimationClip onAnimationClip;
    [SerializeField] private AnimationClip offAnimationClip;

    [Header("Base Settings")]
    [SerializeField] private float intensityBase = 1f;
    [SerializeField] private float rangeBase = 5f;

    [Header("Fade Settings")]
    [SerializeField] private float fadeSpeed = 3f;

    [Header("Flicker Settings")]
    [Tooltip("Set variation to 0 for no flicker.")]
    [SerializeField] private float intensityVariation = 0.2f;
    [SerializeField] private float rangeVariation = 0.5f;
    [SerializeField] private float flickerSpeed = 2f;

    private float targetIntensity;
    private float targetRange;
    private float noiseSeed;

    void Awake()
    {
        if (lightSource == null)
            lightSource = GetComponent<Light>();
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
        lightSource.range = Mathf.MoveTowards(lightSource.range, targetRange, Time.deltaTime * fadeSpeed);

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
            targetRange = 0f;
        }
        else
        {
            targetIntensity = intensityBase;
            targetRange = rangeBase;
        }
    }

    private void UpdateFlicker()
    {
        if (intensityVariation <= 0f && rangeVariation <= 0f)
            return;

        float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, noiseSeed);
        float intensityFlicker = Mathf.Lerp(-intensityVariation, intensityVariation, noise);
        float rangeFlicker = Mathf.Lerp(-rangeVariation, rangeVariation, noise);

        targetIntensity = intensityBase + intensityFlicker;
        targetRange = rangeBase + rangeFlicker;
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
