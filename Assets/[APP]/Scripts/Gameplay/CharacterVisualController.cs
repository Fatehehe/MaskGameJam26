using UnityEngine;
using UnityEngine.UI;
using System;

public class CharacterVisualController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform characterContainer;
    [SerializeField] private Image bodyImage;
    [SerializeField] private Image maskImage;

    [Header("Animation Points (Drag RectTransform here)")]
    [SerializeField] private Transform spawnPoint;   // Initial Position (Right off-screen)
    [SerializeField] private Transform talkPoint;    // Talking Position (Center screen)
    [SerializeField] private Transform despawnPoint; // Exit Position (Left off-screen)

    [Header("Settings")]
    [SerializeField] private float walkDuration = 1.0f;
    [SerializeField] private LeanTweenType walkEase = LeanTweenType.easeOutQuart;

    public void SetupVisuals(ActiveCustomer customer)
    {
        // 1. Set Sprite Badan
        bodyImage.sprite = customer.ActiveCharacterVisual.BodySprite;

        // 2. Set Sprite Topeng (Awal datang pasti Intact)
        UpdateMaskState(customer);
    }

    public void UpdateMaskState(ActiveCustomer customer)
    {
        MaskData data = customer.ActiveMaskData;
        maskImage.enabled = true;
        maskImage.color = Color.white;

        switch (customer.CurrentState)
        {
            case MaskState.Intact:
                maskImage.sprite = data.NormalMaskSprite;
                break;
            case MaskState.Cracked:
                maskImage.sprite = data.CrackedMaskSprite;
                break;
            case MaskState.Broken:
                maskImage.enabled = false;
                break;
        }
    }

    public void AnimateEnter(Action onComplete)
    {
        // Reset posisi ke spawn point
        characterContainer.position = spawnPoint.position;
        characterContainer.gameObject.SetActive(true);

        // Jalan ke tengah
        LeanTween.move(characterContainer.gameObject, talkPoint.position, walkDuration)
            .setEase(walkEase)
            .setOnComplete(onComplete);

        // Opsional: Bobbing animation (naik turun dikit biar kayak jalan)
        LeanTween.moveY(characterContainer.gameObject, characterContainer.position.y + 20f, 0.2f)
        .setLoopPingPong(5);
    }

    public void AnimateExit(Action onComplete)
    {
        // Jalan ke titik keluar
        LeanTween.move(characterContainer.gameObject, despawnPoint.position, walkDuration)
            .setEase(LeanTweenType.easeInBack)
            .setOnComplete(() =>
            {
                characterContainer.gameObject.SetActive(false);
                onComplete?.Invoke();
            });
    }

    public void HideInstant()
    {
        characterContainer.gameObject.SetActive(false);
    }
}