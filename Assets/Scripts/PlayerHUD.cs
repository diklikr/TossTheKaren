using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] Launcher launcher;

    [SerializeField] Button launchButton;
    [SerializeField] Button angleButton;
    [SerializeField] Button shootKaren;

    [Header("Force slider (shown with the launch button)")]
    [SerializeField] Slider launchSlider;
    [SerializeField] RawImage explod;

    [Header("Player height slider")]
    [SerializeField] Slider PlayerHeightSlider;
    [SerializeField] Transform Player;
    [SerializeField] float MaxHeight = 1000f;

    [Header("Distance tracker")]
    [SerializeField] TMP_Text distanceText;    // shows e.g. "25m"

    [Header("Bounce counter")]
    [SerializeField] List<RawImage> bounceImages = new List<RawImage>();

    int bouncesLeft;
    float startX;
    float maxDistance;

    void Start()
    {
        angleButton.gameObject.SetActive(true);
        launchButton.gameObject.SetActive(false);
        launchSlider.gameObject.SetActive(false);
        explod.gameObject.SetActive(false);
        shootKaren.gameObject.SetActive(false);

        PlayerHeightSlider.minValue = 0f;
        PlayerHeightSlider.maxValue = MaxHeight;
        PlayerHeightSlider.value = 0f;

        startX = Player.position.x;
        maxDistance = 0f;
        distanceText.text = "0m";

        bouncesLeft = bounceImages.Count;
        foreach (RawImage img in bounceImages)
            img.gameObject.SetActive(true);
    }

    void Update()
    {
        UpdateHeightSlider();
        UpdateDistance();
    }

    void UpdateHeightSlider()
    {
        if (Player == null || PlayerHeightSlider == null) return;

        PlayerHeightSlider.value = Mathf.Clamp(Player.position.y, 0f, MaxHeight);
    }

    void UpdateDistance()
    {
        if (Player == null || distanceText == null) return;

        // Furthest distance reached, so it never goes down if the player bounces back
        maxDistance = Mathf.Max(maxDistance, Player.position.x - startX);
        distanceText.text = Mathf.FloorToInt(maxDistance) + "m";
    }

    public void SetAngle()
    {
        angleButton.gameObject.SetActive(false);
        launcher.OnAngle();
        launchButton.gameObject.SetActive(true);
        launchSlider.gameObject.SetActive(true);
        explod.gameObject.SetActive(true);
    }

    public void LaunchButton()
    {
        launchButton.gameObject.SetActive(false);
        launchSlider.gameObject.SetActive(false);
        explod.gameObject.SetActive(false);
        launcher.OnLaunch();
        shootKaren.gameObject.SetActive(bouncesLeft > 0);
    }

    public void InAirBounce()
    {
        if (bouncesLeft <= 0) return;

        launcher.OnBounce();

        bounceImages[bounceImages.Count - bouncesLeft].gameObject.SetActive(false);
        bouncesLeft--;

        if (bouncesLeft <= 0)
            shootKaren.gameObject.SetActive(false);
    }
}