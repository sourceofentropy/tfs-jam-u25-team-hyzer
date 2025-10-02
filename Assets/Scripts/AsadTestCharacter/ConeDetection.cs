
using UnityEngine;

public class ConeDetection : MonoBehaviour
{
    /// <summary>
    ///  add Range asset to show indicator of threatdetection
    /// </summary>
    [Header("References")]
    public Transform player;
    public Transform rightPatrolBarrier;
    public Transform leftPatrolBarrier;
    public Rigidbody2D rb; // enemy rigidbody

    [Header("Detection Settings")]
    public float detectionRadius = 5f;
    public float detectionAngle = 45f; // half-cone in degrees

    private int currentPatrolIndex;
    private float lastFacingDir = 1f; // 1 = right, -1 = left

    public void SetPatrolIndex(int index)
    {
        currentPatrolIndex = index;
    }

    public bool PlayerInCone(bool usePlayerForward)
    {
        if (player == null) return false;

        Vector2 toPlayer = (player.position - transform.position).normalized;

        // Forward direction: based on velocity or last facing direction
        Vector2 forward;
        if (Mathf.Abs(rb.linearVelocity.x) > 0.1f)
        {
            lastFacingDir = Mathf.Sign(rb.linearVelocity.x);
            forward = new Vector2(lastFacingDir, 0f);
        }
        else if (usePlayerForward)
            forward = toPlayer;
        else
            forward = new Vector2(lastFacingDir, 0f);

        // --- Boundary checks ---
        if (player.position.x < leftPatrolBarrier.position.x) return false;
        if (player.position.x > rightPatrolBarrier.position.x) return false;

        float distance = Vector2.Distance(transform.position, player.position);
        float angle = Vector2.Angle(forward, toPlayer);

        return distance <= detectionRadius && angle <= detectionAngle;
    }

    // ----------- DEBUG VISUALS -----------
    private void OnDrawGizmos()
    {
        if (rb == null) return;

        Gizmos.color = Color.red;

        // Determine forward direction
        Vector3 forward = (lastFacingDir >= 0) ? Vector3.right : Vector3.left;

        // Draw fan (cone)
        int segments = 20;
        for (int i = 0; i <= segments; i++)
        {
            float t = i / (float)segments;
            float angle = Mathf.Lerp(-detectionAngle, detectionAngle, t);
            Quaternion rot = Quaternion.Euler(0, 0, angle);
            Vector3 dir = rot * forward * detectionRadius;
            Gizmos.DrawLine(transform.position, transform.position + dir);
        }

        // Center line
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + forward * detectionRadius);
    }
}
