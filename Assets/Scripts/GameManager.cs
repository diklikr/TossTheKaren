using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] CanvasGroup victoryScreen;
    [SerializeField] CanvasGroup roundOver;
    [SerializeField] CanvasGroup HUD;
    [SerializeField] GameObject Karen;
    [SerializeField] Launcher launcher;

    [Header("Round end")]
    [SerializeField] float StopSpeed = 1f;
    [SerializeField] float StopTime = 3f;

    Vector3 startPosition;
    Quaternion startRotation;
    Rigidbody karenBody;
    float stoppedFor;
    bool checkingMovement;

    void Start()
    {
        if (Karen == null || !Karen.TryGetComponent(out karenBody) ||
            launcher == null || victoryScreen == null ||
            roundOver == null || HUD == null)
        {
            Debug.LogError("Assign Karen, the launcher, and all canvas groups.", this);
            enabled = false;
            return;
        }

        startPosition = karenBody.position;
        startRotation = karenBody.rotation;
    }

    void FixedUpdate()
    {
        if (!checkingMovement) return;

        if (karenBody.linearVelocity.sqrMagnitude < StopSpeed * StopSpeed)
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
        if (!enabled) return;

        checkingMovement = false;
        stoppedFor = 0f;

        if (!karenBody.isKinematic)
        {
            karenBody.linearVelocity = Vector3.zero;
            karenBody.angularVelocity = Vector3.zero;
        }

        karenBody.position = startPosition;
        karenBody.rotation = startRotation;

        launcher.ResetLauncher();

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