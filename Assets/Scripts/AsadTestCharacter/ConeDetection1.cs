using UnityEngine;

public class ConeDetectionTest : MonoBehaviour
{
    [Header("Detection Settings")]
    public Transform player;          // Reference to the player
    public float detectionRadius = 5f; // How far the enemy can see
    public float detectionAngle = 45f; // Half-angle of the cone (e.g. 45 = 90° cone)

    [Header("Debug")]
    public bool showDebug = true;

    private int patrolIndex;

    public void SetPatrolIndex(int index)
    {
        patrolIndex = index;
    }

    /// <summary>
    /// Checks if the player is inside the detection cone.
    /// </summary>
    public bool PlayerInCone(bool requireLineOfSight)
    {
        if (player == null) return false;

        // Direction from enemy to player
        Vector2 toPlayer = (player.position - transform.position).normalized;

        // Enemy's facing direction (based on local scale or right vector)
        Vector2 facing = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

        // Check angle
        float angle = Vector2.Angle(facing, toPlayer);
        if (angle > detectionAngle) return false;

        // Check distance
        float dist = Vector2.Distance(transform.position, player.position);
        if (dist > detectionRadius) return false;

        // Optional: check line of sight with raycast
        if (requireLineOfSight)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, toPlayer, detectionRadius);
            if (hit.collider == null || !hit.collider.CompareTag("Player"))
                return false;
        }

        return true;
    }

    private void OnDrawGizmosSelected()
    {
        if (!showDebug) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Draw cone lines
        Vector2 facing = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        float leftLimit = -detectionAngle;
        float rightLimit = detectionAngle;

        Quaternion leftRot = Quaternion.Euler(0, 0, leftLimit);
        Quaternion rightRot = Quaternion.Euler(0, 0, rightLimit);

        Vector2 leftDir = leftRot * facing;
        Vector2 rightDir = rightRot * facing;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + leftDir * detectionRadius);
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + rightDir * detectionRadius);
    }
}
