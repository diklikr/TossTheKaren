using UnityEngine;

public class skybox : MonoBehaviour
{
    [SerializeField] Transform player;

    [Header("Starting distance")]
    [SerializeField] Material startLow;
    [SerializeField] Material startHigh;

    [Header("Middle distance")]
    [SerializeField] Material middleLow;
    [SerializeField] Material middleHigh;

    [Header("Far distance")]
    [SerializeField] Material farLow;
    [SerializeField] Material farHigh;

    [Header("Distance thresholds (world X)")]
    [SerializeField] float middleStartsAt = 1000f;
    [SerializeField] float farStartsAt = 2000f;

    [Header("Height thresholds (world Y)")]
    [SerializeField] float highHeightStartsAt = 500f;

    Material currentSkybox;

    void Start()
    {
        if (player == null ||
            startLow == null || startHigh == null ||
            middleLow == null || middleHigh == null ||
            farLow == null || farHigh == null)
        {
            Debug.LogError("Assign the player and all six skybox materials.", this);
            enabled = false;
            return;
        }

        if (farStartsAt <= middleStartsAt)
        {
            Debug.LogError("farStartsAt must be greater than middleStartsAt.", this);
            enabled = false;
            return;
        }

        UpdateSkybox();
    }

    void Update()
    {
        UpdateSkybox();
    }

    void UpdateSkybox()
    {
        float x = player.position.x;
        bool high = player.position.y >= highHeightStartsAt;

        Material next;
        if (x >= farStartsAt)
            next = high ? farHigh : farLow;
        else if (x >= middleStartsAt)
            next = high ? middleHigh : middleLow;
        else
            next = high ? startHigh : startLow;

        if (next != currentSkybox)
        {
            RenderSettings.skybox = next;
            currentSkybox = next;
        }
    }
}