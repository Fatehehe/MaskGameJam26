using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class HomeButtonUI : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private string sceneToLoad = "GameScene";

    [Header("Text Target")]
    [SerializeField] private RectTransform textTarget;

    [Header("Floating")]
    [SerializeField] private float floatDistance = 20f;
    [SerializeField] private float floatSpeed = 2f;

    [Header("Fade")]
    [SerializeField] private float minAlpha = 0.35f;
    [SerializeField] private float fadeSpeed = 2f;

    private Vector2 startPos;
    private Graphic textGraphic; // bisa Text atau TMP

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(LoadScene);

        if (textTarget != null)
        {
            startPos = textTarget.anchoredPosition;
            textGraphic = textTarget.GetComponent<Graphic>();
            // Graphic = base class untuk Text & TMP_Text
        }
    }

    void Update()
    {
        if (textTarget == null || textGraphic == null) return;

        // Floating
        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatDistance;
        textTarget.anchoredPosition = startPos + new Vector2(0, yOffset);

        // Fade
        float t = (Mathf.Sin(Time.time * fadeSpeed) + 1f) * 0.5f;
        float alpha = Mathf.Lerp(minAlpha, 1f, t);

        Color c = textGraphic.color;
        c.a = alpha;
        textGraphic.color = c;
    }

    void LoadScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
