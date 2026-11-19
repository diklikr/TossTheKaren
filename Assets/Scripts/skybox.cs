using UnityEngine;

public class skybox : MonoBehaviour
{
    [SerializeField] Transform player;

    [Header("Starting distance")]
    [SerializeField] Material startLow;
    [SerializeField] Material startMiddle;
    [SerializeField] Material startHigh;

    [Header("Middle distance")]
    [SerializeField] Material middleLow;
    [SerializeField] Material middleMiddle;
    [SerializeField] Material middleHigh;

    [Header("Far distance")]
    [SerializeField] Material farLow;
    [SerializeField] Material farMiddle;
    [SerializeField] Material farHigh;

    [Header("Distance thresholds (world X)")]
    [SerializeField] float middleStartsAt = 1000f;
    [SerializeField] float farStartsAt = 2000f;

    [Header("Height thresholds (world Y)")]
    [SerializeField] float middleHeightStartsAt = 100f;
    [SerializeField] float highHeightStartsAt = 500f;

    Material currentSkybox;

    void Start()
    {
        if (player == null ||
            startLow == null || startMiddle == null || startHigh == null ||
            middleLow == null || middleMiddle == null || middleHigh == null ||
            farLow == null || farMiddle == null || farHigh == null)
        {
            Debug.LogError("Assign the player and all nine skybox materials.", this);
            enabled = false;
            return;
        }

        if (farStartsAt <= middleStartsAt ||
            highHeightStartsAt <= middleHeightStartsAt)
        {
            Debug.LogError("Higher zone thresholds must be greater than lower thresholds.", this);
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
        Material next;
        float x = player.position.x;
        float y = player.position.y;

        if (x >= farStartsAt)
        {
            next = y >= highHeightStartsAt ? farHigh :
                   y >= middleHeightStartsAt ? farMiddle : farLow;
        }
        else if (x >= middleStartsAt)
        {
            next = y >= highHeightStartsAt ? middleHigh :
                   y >= middleHeightStartsAt ? middleMiddle : middleLow;
        }
        else
        {
            next = y >= highHeightStartsAt ? startHigh :
                   y >= middleHeightStartsAt ? startMiddle : startLow;
        }

        if (next != currentSkybox)
        {
            RenderSettings.skybox = next;
            currentSkybox = next;
        }
    }
}