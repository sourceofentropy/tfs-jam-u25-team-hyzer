using UnityEngine;

public class ButtonIdleFloat : MonoBehaviour
{
    [Header("Float Settings")]
    [SerializeField] private float amplitude = 5f;
    [SerializeField] private float frequency = 2f;
    [Tooltip("If true, float horizontally. If false, float vertically.")]
    [SerializeField] private bool floatHorizontally = false;

    private RectTransform rectTransform;
    private Vector2 initialAnchoredPos;

    // Static counter to assign a small offset to each button
    private static int buttonCount = 0;
    private float phaseOffset;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        initialAnchoredPos = rectTransform.anchoredPosition;

        // Tiny stagger based on index for fluid motion
        phaseOffset = buttonCount * 0.2f; // tweak 0.1 - 0.3 for more/less offset
        buttonCount++;

        Debug.Log($"[ButtonIdleFloat] {name} initialized at AnchoredPos={initialAnchoredPos}, PhaseOffset={phaseOffset}");
    }

    void Update()
    {
        float offset = Mathf.Sin(Time.time * frequency + phaseOffset) * amplitude;

        Vector2 delta = floatHorizontally
            ? new Vector2(offset, 0)
            : new Vector2(0, offset);

        rectTransform.anchoredPosition = initialAnchoredPos + delta;
    }
}
