using UnityEngine;

public class Bounce : MonoBehaviour
{
    [SerializeField] string PlayerTag = "Player";

    [Header("Bounce")]
    [SerializeField] float BounceForce = 25f;
    [SerializeField, Range(0f, 90f)] float BounceAngle = 60f;  // degrees up from the horizontal
    [SerializeField] bool ForwardIsRight = true;               // false = bounce to the left

    [Header("Squash effect")]
    [SerializeField] float SquashAmount = 0.5f;
    [SerializeField] float SquashTime = 0.15f;

    Vector3 startScale;
    float squashTimer = 0f;

    void Start()
    {
        startScale = transform.localScale;
    }

    void Update()
    {
        if (squashTimer > 0f)
        {
            squashTimer -= Time.deltaTime;
            float t = 1f - Mathf.Clamp01(squashTimer / SquashTime);
            float y = Mathf.Lerp(startScale.y * SquashAmount, startScale.y, t);
            transform.localScale = new Vector3(startScale.x, y, startScale.z);
        }
        else
        {
            transform.localScale = startScale;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(PlayerTag)) return;

        Rigidbody rb = other.attachedRigidbody;
        if (rb == null) return;

        float rad = BounceAngle * Mathf.Deg2Rad;
        float side = ForwardIsRight ? 1f : -1f;
        Vector3 dir = new Vector3(Mathf.Cos(rad) * side, Mathf.Sin(rad), 0f);

        Vector3 v = rb.linearVelocity;
        if (v.y < 0f) v.y = 0f;
        rb.linearVelocity = v;

        rb.AddForce(dir * BounceForce, ForceMode.Impulse);

        squashTimer = SquashTime;
    }
}