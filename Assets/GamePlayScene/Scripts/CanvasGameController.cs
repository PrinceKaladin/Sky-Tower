using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class CanvasGameController : MonoBehaviour
{
    [Header("UI ��������")]
    public GameObject PauseMenu;
    public GameObject GameOverMenu;
    public GameObject Background;

    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI GameOverScoreText;
    public TextMeshProUGUI BestScoreText;

    [Header("�������� �����")]
    [SerializeField] private float scoreAnimationSpeed = 300f; // ������ � ������� ��������

    [Header("�����")]
    public float fadeDuration = 0.5f;

    // ��������� ���� ��� ������������ �������� �����
    private float displayedScoreFloat = 0f;
    private int targetScore = 0;

    private CanvasGroup GetCanvasGroup(GameObject obj)
    {
        CanvasGroup cg = obj.GetComponent<CanvasGroup>();
        if (cg == null)
        {
            cg = obj.AddComponent<CanvasGroup>();
        }
        return cg;
    }

    void Update()
    {
        // ������������ �������� ����� (�������� ���� ��� �����)
        if (displayedScoreFloat != targetScore)
        {
            displayedScoreFloat = Mathf.MoveTowards(displayedScoreFloat, targetScore,
                scoreAnimationSpeed * Time.unscaledDeltaTime);

            int displayInt = Mathf.FloorToInt(displayedScoreFloat);
            ScoreText.text = $"{displayInt}";
        }
    }

    public void UpdateScore(int meters)
    {
        targetScore = meters;
    }

    public void ShowPauseMenu()
    {
        StartCoroutine(ShowScreen(PauseMenu));
        if (GameOverMenu.activeSelf)
        {
            StartCoroutine(HideScreen(GameOverMenu));
        }
        StartCoroutine(ShowScreen(Background));
        Time.timeScale = 0f;
    }

    public void HideAllMenu()
    {
        if (PauseMenu.activeSelf)
        {
            StartCoroutine(HideScreen(PauseMenu));
        }
        if (GameOverMenu.activeSelf)
        {
            StartCoroutine(HideScreen(GameOverMenu));
        }
        if (Background.activeSelf)
        {
            StartCoroutine(HideScreen(Background));
        }
        Time.timeScale = 1f;
    }

    public void ShowGameOverMenu(int finalMeters)
    {
        if (PauseMenu.activeSelf)
        {
            StartCoroutine(HideScreen(PauseMenu));
        }

        StartCoroutine(ShowScreen(GameOverMenu));
        StartCoroutine(ShowScreen(Background));

        // ��������� ���������� ��������� ���� (����)
        GameOverScoreText.text = $"Score: {finalMeters}";
        ScoreText.text = $"Score: {finalMeters}";
        displayedScoreFloat = finalMeters;
        targetScore = finalMeters;

        // ��������� ������ ���������
        if (finalMeters > PlayerData.Instance.BestScore)
        {
            PlayerData.Instance.BestScore = finalMeters;
        }

        BestScoreText.text = $"Best Score: {PlayerData.Instance.BestScore}";
    }

    private IEnumerator ShowScreen(GameObject obj)
    {
        if (!obj) yield break;
        obj.SetActive(true);
        CanvasGroup cg = GetCanvasGroup(obj);
        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }

        cg.alpha = 1f;
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }

    private IEnumerator HideScreen(GameObject obj)
    {
        if (!obj || !obj.activeSelf) yield break;
        CanvasGroup cg = GetCanvasGroup(obj);
        float t = 0f;
        float startAlpha = cg.alpha;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(startAlpha, 0f, t / fadeDuration);
            yield return null;
        }

        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;
        obj.SetActive(false);
    }
    public void levelselect(){
        SceneManager.LoadScene(0);
    }
}