using UnityEngine;

public class UFO : MonoBehaviour
{
    [SerializeField] Transform ufo;
    [SerializeField] string PlayerTag = "Player";
    [SerializeField] string HipsName = "Hips";

    [Header("Lift")]
    [SerializeField] float LiftHeight = 50f;
    [SerializeField] float LiftSpeed = 10f;
    [SerializeField] float SpinSpeed = 90f;

    [Header("After release")]
    [SerializeField] float DepartureSpeed = 10f;
    [SerializeField] float DisappearDelay = 3f;

    struct BodyState
    {
        public Rigidbody body;
        public bool wasKinematic;
        public Vector3 velocity;
        public Vector3 angularVelocity;
        public Vector3 offsetFromHips;
        public Quaternion rotation;
    }

    BodyState[] playerBodies;
    Animator[] animators;
    bool[] animatorStates;

    Collider beam;
    Vector3 startPivot;
    float lifted;
    float spinAngle;
    float departureTime;
    bool captured;
    bool released;

    void Awake()
    {
        beam = GetComponent<Collider>();

        if (ufo == null || beam == null || !beam.isTrigger ||
            LiftHeight <= 0f || LiftSpeed <= 0f ||
            DepartureSpeed < 0f || DisappearDelay < 0f)
        {
            Debug.LogError(
                "Assign the UFO, use a trigger collider, and check movement settings.",
                this
            );
            enabled = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!enabled || captured || released) return;

        Transform playerRoot = FindPlayerRoot(other.transform);
        if (playerRoot == null || other.attachedRigidbody == null) return;

        Rigidbody[] bodies = playerRoot.GetComponentsInChildren<Rigidbody>();
        Rigidbody hips = null;

        foreach (Rigidbody body in bodies)
        {
            if (body.name == HipsName)
            {
                hips = body;
                break;
            }
        }

        if (hips == null)
        {
            Debug.LogError("Could not find Karen's hips Rigidbody.", playerRoot);
            return;
        }

        animators = playerRoot.GetComponentsInChildren<Animator>();
        animatorStates = new bool[animators.Length];

        for (int i = 0; i < animators.Length; i++)
        {
            animatorStates[i] = animators[i].enabled;
            animators[i].enabled = false;
        }

        Vector3 hipsPosition = hips.position;
        Vector3 beamCenter = beam.bounds.center;

        // Center horizontally while keeping the hips at their current height.
        startPivot = new Vector3(
            beamCenter.x,
            hipsPosition.y,
            beamCenter.z
        );

        playerBodies = new BodyState[bodies.Length];

        for (int i = 0; i < bodies.Length; i++)
        {
            Rigidbody body = bodies[i];

            playerBodies[i] = new BodyState
            {
                body = body,
                wasKinematic = body.isKinematic,
                velocity = body.linearVelocity,
                angularVelocity = body.angularVelocity,
                offsetFromHips = body.position - hipsPosition,
                rotation = body.rotation
            };

            body.isKinematic = true;
        }

        captured = true;
    }

    void FixedUpdate()
    {
        if (captured)
        {
            if (lifted >= LiftHeight)
            {
                ReleasePlayer();
                released = true;
                return;
            }

            float distance = Mathf.Min(
                LiftSpeed * Time.fixedDeltaTime,
                LiftHeight - lifted
            );

            lifted += distance;
            spinAngle += SpinSpeed * Time.fixedDeltaTime;

            Quaternion spin = Quaternion.AngleAxis(spinAngle, Vector3.right);
            Vector3 pivot = startPivot + Vector3.up * lifted;

            ufo.position += Vector3.up * distance;

            foreach (BodyState state in playerBodies)
            {
                if (state.body == null) continue;

                state.body.MovePosition(pivot + spin * state.offsetFromHips);
                state.body.MoveRotation(spin * state.rotation);
            }
        }
        else if (released)
        {
            ufo.position += Vector3.up * DepartureSpeed * Time.fixedDeltaTime;
            departureTime += Time.fixedDeltaTime;

            if (departureTime >= DisappearDelay)
                Destroy(ufo.gameObject);
        }
    }

    void ReleasePlayer()
    {
        if (!captured) return;

        foreach (BodyState state in playerBodies)
        {
            if (state.body == null) continue;

            state.body.isKinematic = state.wasKinematic;

            if (!state.wasKinematic)
            {
                state.body.linearVelocity = state.velocity;
                state.body.angularVelocity = state.angularVelocity;
                state.body.WakeUp();
            }
        }

        for (int i = 0; i < animators.Length; i++)
        {
            if (animators[i] != null)
                animators[i].enabled = animatorStates[i];
        }

        captured = false;
    }

    void OnDisable()
    {
        ReleasePlayer();
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