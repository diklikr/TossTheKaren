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

    [Header("Button sounds")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip launchSound;
    [SerializeField] AudioClip angleSound;
    [SerializeField] AudioClip bounceSound;

    [Header("Explosion")]
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] Transform explosionTransform;

    [Header("Force slider (shown with the launch button)")]
    [SerializeField] Slider launchSlider;
    [SerializeField] RawImage explod;

    [Header("Player height slider")]
    [SerializeField] Slider PlayerHeightSlider;
    [SerializeField] Transform Player;
    [SerializeField] float MaxHeight = 1000f;

    [Header("Distance tracker")]
    [SerializeField] TMP_Text distanceText;

    [Header("Bounce counter")]
    [SerializeField] List<RawImage> bounceImages = new List<RawImage>();

    int bouncesLeft;
    float startX;
    float maxDistance;

    void Start()
    {
        ResetHUD();

        if (audioSource == null)
            Debug.LogWarning("Assign an AudioSource for button sounds.", this);

        if (launchSound == null || angleSound == null || bounceSound == null)
            Debug.LogWarning("Assign all three button sound clips.", this);

        if (explosionPrefab == null || explosionTransform == null)
            Debug.LogWarning("Assign the explosion prefab and transform.", this);
    }

    void Update()
    {
        UpdateHeightSlider();
        UpdateDistance();
    }

    public void ResetHUD()
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

    void UpdateHeightSlider()
    {
        if (Player == null || PlayerHeightSlider == null) return;

        PlayerHeightSlider.value = Mathf.Clamp(Player.position.y, 0f, MaxHeight);
    }

    void UpdateDistance()
    {
        if (Player == null || distanceText == null) return;

        maxDistance = Mathf.Max(maxDistance, Player.position.x - startX);
        distanceText.text = Mathf.FloorToInt(maxDistance) + "m";
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }

    void SpawnExplosion()
    {
        if (explosionPrefab != null && explosionTransform != null)
            Instantiate(explosionPrefab, explosionTransform.position, explosionTransform.rotation);
    }

    public void SetAngle()
    {
        PlaySound(angleSound);
        angleButton.gameObject.SetActive(false);
        launcher.OnAngle();
        launchButton.gameObject.SetActive(true);
        launchSlider.gameObject.SetActive(true);
        explod.gameObject.SetActive(true);
    }

    public void LaunchButton()
    {
        PlaySound(launchSound);
        SpawnExplosion();
        launchButton.gameObject.SetActive(false);
        launchSlider.gameObject.SetActive(false);
        explod.gameObject.SetActive(false);
        launcher.OnLaunch();
        shootKaren.gameObject.SetActive(bouncesLeft > 0);
    }

    public void InAirBounce()
    {
        if (bouncesLeft <= 0) return;

        PlaySound(bounceSound);
        SpawnExplosion();
        launcher.OnBounce();

        bounceImages[bounceImages.Count - bouncesLeft].gameObject.SetActive(false);
        bouncesLeft--;

        if (bouncesLeft <= 0)
            shootKaren.gameObject.SetActive(false);
    }
}
