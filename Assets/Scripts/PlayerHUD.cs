using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] Launcher launcher;

    [SerializeField] Button launchButton;
    [SerializeField] Button angleButton;
    [SerializeField] Button shootKaren;

    [Header("Force slider (shown with the launch button)")]
    [SerializeField] Slider launchSlider;
    [SerializeField] RawImage explod;

    [Header("Bounce counter")]
    [SerializeField] List<RawImage> bounceImages = new List<RawImage>();

    int bouncesLeft;

    void Start()
    {
        angleButton.gameObject.SetActive(true);
        launchButton.gameObject.SetActive(false);
        launchSlider.gameObject.SetActive(false);
        explod.gameObject.SetActive(false);
        shootKaren.gameObject.SetActive(false);

        bouncesLeft = bounceImages.Count;
        foreach (RawImage img in bounceImages)
            img.gameObject.SetActive(true);
    }

    public void SetAngle()
    {
        angleButton.gameObject.SetActive(false);
        launcher.OnAngle();                       // also starts the force bar
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