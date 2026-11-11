using UnityEngine;
using UnityEngine.UI;

public class Launcher : MonoBehaviour
{
    [SerializeField] Slider LaunchBAR;
    [SerializeField] Transform RagdollRoot;
    [SerializeField] Animator PlayerAnimator;
    [SerializeField] GameObject Cannon;

    [Header("Force")]
    [SerializeField] float ForceSpeed = 200f;
    [SerializeField] float MaxLaunchForce = 300f;
    [SerializeField] float BounceForce = 20f;

    [Header("Angle")]
    [SerializeField] float MaxAngle = 45f;
    [SerializeField] float AngleSpeed = 60f;

    Rigidbody[] ragdollBodies;
    float launchForce;
    float launchAngle;

    bool forceMoving = true;
    bool angleMoving = true;
    bool launched;
    bool animatorWasEnabled;

    void Start()
    {
        if (LaunchBAR == null || RagdollRoot == null || Cannon == null)
        {
            Debug.LogError("Assign the launch bar, ragdoll root, and cannon.", this);
            enabled = false;
            return;
        }

        ragdollBodies = RagdollRoot.GetComponentsInChildren<Rigidbody>();

        if (ragdollBodies.Length == 0)
        {
            Debug.LogError("No rigidbodies found on the ragdoll.", this);
            enabled = false;
            return;
        }
        animatorWasEnabled = PlayerAnimator != null && PlayerAnimator.enabled;
        LaunchBAR.minValue = 0f;
        LaunchBAR.maxValue = MaxLaunchForce;
        LaunchBAR.value = 0f;
    }

    void Update()
    {
        if (launched) return;

        if (angleMoving)
        {
            launchAngle = Mathf.PingPong(Time.time * AngleSpeed, MaxAngle);
            Cannon.transform.localRotation = Quaternion.Euler(-launchAngle, 0f, 0f);
        }

        if (forceMoving)
        {
            launchForce = Mathf.PingPong(Time.time * ForceSpeed, MaxLaunchForce);
            LaunchBAR.value = launchForce;
        }
    }

    public void OnAngle()
    {
        angleMoving = false;
    }

    public void OnLaunch()
    {
        if (!enabled || launched) return;

        forceMoving = false;
        angleMoving = false;
        launched = true;

        if (PlayerAnimator != null)
            PlayerAnimator.enabled = false;

        Vector3 launchVelocity = Cannon.transform.forward * launchForce;

        foreach (Rigidbody rb in ragdollBodies)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.AddForce(launchVelocity, ForceMode.VelocityChange);
        }
    }
    public void OnBounce()
    {
        if (!enabled || !launched) return;

        foreach (Rigidbody rb in ragdollBodies)
        {
            Vector3 velocity = rb.linearVelocity;
            velocity.y = 0f;
            rb.linearVelocity = velocity;
            rb.AddForce(Vector3.up * BounceForce, ForceMode.VelocityChange);
        }
    }
    public void ResetLauncher()
    {
        launched = false;
        forceMoving = true;
        angleMoving = true;
        launchForce = 0f;
        launchAngle = 0f;

        LaunchBAR.value = 0f;
        Cannon.transform.localRotation = Quaternion.identity;

        if (PlayerAnimator != null)
            PlayerAnimator.enabled = animatorWasEnabled;
    }
}