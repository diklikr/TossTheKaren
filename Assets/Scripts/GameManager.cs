using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] CanvasGroup victoryScreen;
    [SerializeField] CanvasGroup roundOver;
    [SerializeField] CanvasGroup HUD;
    [SerializeField] GameObject Karen; // Ragdoll root
    [SerializeField] Transform MovementTarget; // Hips
    [SerializeField] Launcher launcher;
    [SerializeField] PlayerHUD playerHUD;

    [Header("Round end")]
    [SerializeField] float StopSpeed = 1f;
    [SerializeField] float StopTime = 3f;

    Vector3 startPosition;
    Quaternion startRotation;

    Rigidbody[] bodies;
    Vector3[] bodyPositions;
    Quaternion[] bodyRotations;
    bool[] bodyKinematicStates;

    Vector3 lastPosition;
    float stoppedFor;
    bool checkingMovement;
    bool initialized;

    void Awake()
    {
        if (Karen == null || MovementTarget == null || launcher == null ||
            victoryScreen == null || roundOver == null || HUD == null)
        {
            Debug.LogError("Assign Karen, the hips, launcher, and canvas groups.", this);
            enabled = false;
            return;
        }

        startPosition = Karen.transform.position;
        startRotation = Karen.transform.rotation;

        bodies = Karen.GetComponentsInChildren<Rigidbody>();
        if (bodies.Length == 0)
        {
            Debug.LogError("Karen needs Rigidbody components on her body parts.", this);
            enabled = false;
            return;
        }

        bodyPositions = new Vector3[bodies.Length];
        bodyRotations = new Quaternion[bodies.Length];
        bodyKinematicStates = new bool[bodies.Length];

        for (int i = 0; i < bodies.Length; i++)
        {
            bodyPositions[i] = bodies[i].position;
            bodyRotations[i] = bodies[i].rotation;
            bodyKinematicStates[i] = bodies[i].isKinematic;
        }

        initialized = true;
        SetCanvas(victoryScreen, false);
        SetCanvas(roundOver, false);
        SetCanvas(HUD, true);
    }

    void FixedUpdate()
    {
        if (!checkingMovement) return;

        float speed = Vector3.Distance(MovementTarget.position, lastPosition)
                      / Time.fixedDeltaTime;
        lastPosition = MovementTarget.position;

        if (speed < StopSpeed)
        {
            stoppedFor += Time.fixedDeltaTime;
            if (stoppedFor >= StopTime)
                RoundOver();
        }
        else
        {
            stoppedFor = 0f;
        }
    }

    public void StartMovementCheck()
    {
        if (!initialized) return;

        lastPosition = MovementTarget.position;
        stoppedFor = 0f;
        checkingMovement = true;
    }

    public void Victory()
    {
        checkingMovement = false;
        SetCanvas(victoryScreen, true);
        SetCanvas(HUD, false);
    }

    public void RoundOver()
    {
        checkingMovement = false;
        SetCanvas(roundOver, true);
        SetCanvas(HUD, false);
    }

    public void ResetRound()
    {
        if (!initialized) return;

        checkingMovement = false;
        stoppedFor = 0f;

        Karen.transform.SetPositionAndRotation(startPosition, startRotation);

        for (int i = 0; i < bodies.Length; i++)
        {
            Rigidbody body = bodies[i];

            if (!body.isKinematic)
            {
                body.linearVelocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
            }

            body.position = bodyPositions[i];
            body.rotation = bodyRotations[i];
            body.isKinematic = bodyKinematicStates[i];
        }

        launcher.ResetLauncher();
        playerHUD.ResetHUD();

        SetCanvas(victoryScreen, false);
        SetCanvas(roundOver, false);
        SetCanvas(HUD, true);
    }

    void SetCanvas(CanvasGroup group, bool visible)
    {
        group.alpha = visible ? 1f : 0f;
        group.interactable = visible;
        group.blocksRaycasts = visible;
    }
}