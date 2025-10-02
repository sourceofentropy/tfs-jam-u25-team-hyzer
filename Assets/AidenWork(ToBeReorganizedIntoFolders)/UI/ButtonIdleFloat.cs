using UnityEngine;

public class ButtonIdleFloat : MonoBehaviour
{
    [Header("Float Settings")]
    [SerializeField] private float amplitude = 5f;
    [SerializeField] private float frequency = 2f;
    [Tooltip("If true, float horizontally. If false, float vertically.")]
    [SerializeField] private bool floatHorizontally = false;

    private RectTransform rectTransform;
    private Vector3 initialLocalPos;
    private static int buttonCount = 0;
    private int index;
    private float phaseOffset;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        initialLocalPos = rectTransform.localPosition;

        index = buttonCount++;
        // Spread phases evenly (assumes ~5 buttons; adjust if needed)
        phaseOffset = (index * Mathf.PI * 2f) / 5f;
    }

    void Update()
    {
        float offset = Mathf.Sin(Time.time * frequency + phaseOffset) * amplitude;

        Vector3 delta = floatHorizontally
                        ? new Vector3(offset, 0, 0)
                        : new Vector3(0, offset, 0);

        rectTransform.localPosition = initialLocalPos + delta;
    }
}
