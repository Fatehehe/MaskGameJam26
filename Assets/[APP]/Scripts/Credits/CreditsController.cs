using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreditsController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform creditContent;
    [SerializeField] private Button backButton;
    [SerializeField] private string mainMenuScene = "Home";

    [Header("Settings")]
    [SerializeField] private float scrollSpeed = 50f;
    [SerializeField] private float endYPosition = 1500f;

    private bool isScrolling = true;

    private void Start()
    {
        if (backButton) backButton.onClick.AddListener(BackToMenu);
    }

    private void Update()
    {
        if (!isScrolling) return;

        // Logic Scroll Manual tanpa Animation Clip (lebih smooth)
        creditContent.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

        // Cek kalau sudah lewat batas
        if (creditContent.anchoredPosition.y > endYPosition)
        {
            // Opsional: Stop atau Auto back to menu
            // isScrolling = false;
            // BackToMenu();
        }

        // Input Skip (Klik mouse buat mempercepat)
        if (Input.GetMouseButton(0))
        {
            creditContent.anchoredPosition += Vector2.up * scrollSpeed * 2f * Time.deltaTime;
        }
    }

    private void BackToMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }
}