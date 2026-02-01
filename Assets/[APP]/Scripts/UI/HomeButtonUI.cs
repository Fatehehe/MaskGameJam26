using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class HomeButtonUI : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private string sceneToLoad = "GameScene";

    [Header("Floating Text")]
    [SerializeField] private RectTransform textTarget;
    [SerializeField] private float floatDistance = 20f;
    [SerializeField] private float floatSpeed = 2f;
    [SerializeField] private float minAlpha = 0.35f;
    [SerializeField] private float fadeSpeed = 2f;

    [Header("Storyboard")]
    [SerializeField] private GameObject storyboardRoot;
    [SerializeField] private GameObject[] storyPanels; // Story1, Story2, Story3
    [SerializeField] private TMP_Text storyText;
    [SerializeField] private Button nextButton;

    [Header("Typing Settings")]
    [SerializeField] private float typingSpeed = 0.03f;
    [SerializeField] private float popDuration = 0.3f;

    private Graphic textGraphic;
    private Vector2 startPos;

    private int currentPanelIndex = 0;
    private bool isTyping = false;
    private string currentFullText = "";
    private int typingTweenId = -1;

    private string[] storyLines =
    {
        "Di tempat yang tak terikat waktu, ada sebuah kedai yang tak pernah benar-benar tutup. Di sini, teh diseduh bukan hanya untuk menghangatkan tubuh… tapi untuk menenangkan sesuatu yang lebih dalam dari itu.",
        "Tak ada yang tahu bagaimana mereka bisa sampai ke sini. Langkah mereka selalu ragu, seolah tersesat di antara dua arah. Tapi setiap kali pintu itu terbuka… lonceng kecilnya selalu berbunyi pelan.",
        "Mereka datang membawa sesuatu yang tak terlihat, tersembunyi di balik topeng yang tak pernah mereka lepas. Dan mungkin, dengan secangkir teh yang tepat… mereka akhirnya bisa menemukan jalan pulang."
    };

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(StartStorySequence);

        if (textTarget != null)
        {
            startPos = textTarget.anchoredPosition;
            textGraphic = textTarget.GetComponent<Graphic>();
        }

        storyboardRoot.SetActive(false);
        nextButton.onClick.AddListener(OnNextClicked);
    }

    void Update()
    {
        AnimateHomeText();
    }

    void AnimateHomeText()
    {
        if (textTarget == null || textGraphic == null) return;

        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatDistance;
        textTarget.anchoredPosition = startPos + new Vector2(0, yOffset);

        float t = (Mathf.Sin(Time.time * fadeSpeed) + 1f) * 0.5f;
        float alpha = Mathf.Lerp(minAlpha, 1f, t);

        Color c = textGraphic.color;
        c.a = alpha;
        textGraphic.color = c;
    }

    void StartStorySequence()
    {
        GetComponent<Button>().interactable = false;
        storyboardRoot.SetActive(true);
        currentPanelIndex = 0;

        ShowPanel(currentPanelIndex);
    }

    void ShowPanel(int index)
    {
        for (int i = 0; i < storyPanels.Length; i++)
            storyPanels[i].SetActive(i == index);

        storyText.text = "";
        storyText.maxVisibleCharacters = 0;

        LeanTween.scale(storyPanels[index], Vector3.one, popDuration).setFrom(Vector3.zero);

        StartTyping(storyLines[index]);
    }

    void StartTyping(string text)
    {
        currentFullText = text;
        isTyping = true;
        storyText.text = text;
        storyText.maxVisibleCharacters = 0;

        float duration = text.Length * typingSpeed;

        typingTweenId = LeanTween.value(gameObject, 0, text.Length, duration)
            .setEase(LeanTweenType.linear)
            .setOnUpdate((float val) =>
            {
                storyText.maxVisibleCharacters = Mathf.RoundToInt(val);
            })
            .setOnComplete(() =>
            {
                isTyping = false;
            }).id;
    }

    void OnNextClicked()
    {
        if (isTyping)
        {
            LeanTween.cancel(typingTweenId);
            storyText.maxVisibleCharacters = currentFullText.Length;
            isTyping = false;
            return;
        }

        currentPanelIndex++;

        if (currentPanelIndex >= storyPanels.Length)
        {
            SceneManager.LoadScene(sceneToLoad);
            return;
        }

        ShowPanel(currentPanelIndex);
    }
}
