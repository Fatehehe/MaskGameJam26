using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DialogueDisplayController : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private GameObject dialogueContainer;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Button interactButton;

    [Header("Animation Settings")]
    [SerializeField] private float typingSpeed = 0.04f;
    [SerializeField] private float popAnimDuration = 0.3f;
    [SerializeField] private LeanTweenType popEaseType = LeanTweenType.easeOutBack;
    [SerializeField] private LeanTweenType closeEaseType = LeanTweenType.easeInBack;

    private int typingTweenId = -1;
    private string currentFullText;
    private Action onDialogueClosed;

    private enum State { Hidden, Opening, Typing, Finished, Closing }
    private State _currentState = State.Hidden;

    private void Awake()
    {
        dialogueContainer.transform.localScale = Vector3.zero;

        if (interactButton)
        {
            interactButton.onClick.RemoveAllListeners();
            interactButton.onClick.AddListener(OnInteractClicked);
            interactButton.gameObject.SetActive(false);
        }
    }

    public void ShowText(string text, Action onClosed = null)
    {
        CleanupTweens();

        dialogueText.text = "";
        dialogueText.maxVisibleCharacters = 0;

        currentFullText = text;
        onDialogueClosed = onClosed;

        _currentState = State.Opening;
        dialogueContainer.transform.localScale = Vector3.zero;
        LeanTween.scale(dialogueContainer, Vector3.one, popAnimDuration)
            .setEase(popEaseType)
            .setOnComplete(() =>
            {
                StartTyping();
            });
        interactButton.gameObject.SetActive(true);
    }

    private void StartTyping()
    {
        _currentState = State.Typing;
        dialogueText.text = currentFullText;
        dialogueText.maxVisibleCharacters = 0;

        float duration = currentFullText.Length * typingSpeed;

        typingTweenId = LeanTween.value(gameObject, 0, currentFullText.Length, duration)
            .setEase(LeanTweenType.linear)
            .setOnUpdate((float val) =>
            {
                dialogueText.maxVisibleCharacters = Mathf.RoundToInt(val);
            })
            .setOnComplete(() =>
            {
                _currentState = State.Finished;
            })
            .id;
    }

    private void OnInteractClicked()
    {
        switch (_currentState)
        {
            case State.Typing:
                // SKIP: Tampilkan semua teks + Efek Bounce Kecil
                CleanupTweens();
                dialogueText.maxVisibleCharacters = currentFullText.Length;
                _currentState = State.Finished;

                // Efek Bounce kecil saat skip
                LeanTween.scale(dialogueContainer, Vector3.one * 1.1f, 0.1f)
                    .setEase(LeanTweenType.easeOutQuad)
                    .setLoopPingPong(1);
                break;

            case State.Finished:
                // CLOSE: Tutup dialog box (1 -> 0)
                CloseDialogue();
                break;
        }
    }

    private void CloseDialogue()
    {
        _currentState = State.Closing;

        LeanTween.scale(dialogueContainer, Vector3.zero, popAnimDuration)
            .setEase(closeEaseType)
            .setOnComplete(() =>
            {
                _currentState = State.Hidden;
                onDialogueClosed?.Invoke(); // Panggil callback untuk mulai minigame
            });
        interactButton.gameObject.SetActive(false);
    }

    private void CleanupTweens()
    {
        if (typingTweenId != -1) LeanTween.cancel(typingTweenId);
        LeanTween.cancel(dialogueContainer);
    }
}