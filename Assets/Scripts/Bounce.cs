using System.Collections.Generic;
using UnityEngine;

public class Bounce : MonoBehaviour
{
    [SerializeField] string PlayerTag = "Player";

    [Header("Bounce")]
    [SerializeField] float BounceForce = 25f;
    [SerializeField, Range(0f, 90f)] float BounceAngle = 60f;
    [SerializeField] bool ForwardIsRight = true;

    [Header("Squash effect")]
    [SerializeField] float SquashAmount = 0.5f;
    [SerializeField] float SquashTime = 0.15f;

    Vector3 startScale;
    float squashTimer;

    // Tracks body parts touching this bounce object.
    readonly Dictionary<Transform, HashSet<Collider>> touching =
        new Dictionary<Transform, HashSet<Collider>>();

    void Start()
    {
        startScale = transform.localScale;
    }

    void Update()
    {
        if (squashTimer > 0f && SquashTime > 0f)
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
        Transform root = FindPlayerRoot(other.transform);
        if (root == null) return;

        if (touching.TryGetValue(root, out HashSet<Collider> colliders))
        {
            colliders.Add(other);
            return;
        }

        Rigidbody[] bodies = root.GetComponentsInChildren<Rigidbody>();
        if (bodies.Length == 0) return;

        touching.Add(root, new HashSet<Collider> { other });

        float rad = BounceAngle * Mathf.Deg2Rad;
        float side = ForwardIsRight ? 1f : -1f;
        Vector3 direction = new Vector3(
            Mathf.Cos(rad) * side,
            Mathf.Sin(rad),
            0f
        );

        foreach (Rigidbody rb in bodies)
        {
            if (rb.isKinematic) continue;

            Vector3 velocity = rb.linearVelocity;
            if (velocity.y < 0f) velocity.y = 0f;
            rb.linearVelocity = velocity;

            rb.AddForce(direction * BounceForce, ForceMode.VelocityChange);
        }

        squashTimer = SquashTime;
    }

    void OnTriggerExit(Collider other)
    {
        Transform root = FindPlayerRoot(other.transform);
        if (root == null) return;

        if (touching.TryGetValue(root, out HashSet<Collider> colliders))
        {
            colliders.Remove(other);
            if (colliders.Count == 0)
                touching.Remove(root);
        }
    }

    Transform FindPlayerRoot(Transform part)
    {
        while (part != null)
        {
            if (part.CompareTag(PlayerTag))
                return part;

            part = part.parent;
        }

        return null;
    }
}