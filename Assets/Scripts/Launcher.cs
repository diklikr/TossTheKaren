using UnityEngine;
using UnityEngine.UI;
public class Launcher : MonoBehaviour
{
    [SerializeField]
    Slider LaunchBAR;
    [SerializeField]
    GameObject Player;
    [SerializeField]
    GameObject Cannon;

    [Header("Force")]
    [SerializeField] float ForceSpeed = 200f;     // bar units per second (higher = faster)
    [SerializeField] float BounceForce = 20f; // force applied to the player when shooting them in the air

    [Header("Angle")]
    [SerializeField] float MaxAngle = 45f;        // cannon moves between 0 and 45 degrees
    [SerializeField] float AngleSpeed = 60f;      // degrees per second

    float LaunchForce = 0f;
    float MaxLaunchForce = 100f;

    float launchAngle = 0f;

    bool forceMoving = true;
    bool angleMoving = true;
    bool launched = false;

    void Start()
    {
        LaunchBAR.minValue = 0f;
        LaunchBAR.maxValue = MaxLaunchForce;
        LaunchBAR.value = 0f;
    }

    void Update()
    {
        if (launched) return;

        if (angleMoving)
            UpdateAngle();

        if (forceMoving)
            UpdateSlider();
    }

    // Stops the angle movement and keeps the current angle
    public void OnAngle()
    {
        angleMoving = false;
    }

    // Stops the slider and launches the player with the current force and angle
    public void OnLaunch()
    {
        if (launched) return;

        forceMoving = false;
        angleMoving = false;
        launched = true;

        Rigidbody rb = Player.GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero; // use rb.velocity on older Unity versions
        rb.AddForce(Cannon.transform.forward * LaunchForce, ForceMode.Impulse);
    }

    // Bar goes 0 -> 100 -> 0 quickly, repeating
    void UpdateSlider()
    {
        LaunchForce = Mathf.PingPong(Time.time * ForceSpeed, MaxLaunchForce);
        LaunchBAR.value = LaunchForce;
    }

    // Cannon rotates on the X axis between 0 and MaxAngle
    void UpdateAngle()
    {
        launchAngle = Mathf.PingPong(Time.time * AngleSpeed, MaxAngle);
        Cannon.transform.localRotation = Quaternion.Euler(-launchAngle, 0f, 0f); // negative X tilts the cannon up
    }

    public void OnBounce()
    {
        if (!launched) return;

        Rigidbody rb = Player.GetComponent<Rigidbody>();
        Vector3 v = rb.linearVelocity;   // use rb.velocity on older Unity versions
        v.y = 0f;                        // cancel the downward speed so the bounce is consistent
        rb.linearVelocity = v;
        rb.AddForce(Vector3.up * BounceForce, ForceMode.Impulse);
    }
}
