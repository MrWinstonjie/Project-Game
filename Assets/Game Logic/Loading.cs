using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    public GameObject loadingPanel;
    public string loadingSceneName = "SampleScene";
    public bool showOnStart = false;
    public float loadScreenDuration = 2f;
    public ChapterDialogue chapterDialogue;

    private static Loading instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        if (showOnStart)
        {
            if (loadingPanel != null)
                loadingPanel.SetActive(true);

            StartCoroutine(ShowLoadingThenStartDialogue());
        }
        else if (loadingPanel != null)
        {
            loadingPanel.SetActive(false);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (loadingPanel != null && loadingPanel.activeSelf)
        {
            loadingPanel.SetActive(false);
        }
    }

    public void LoadScene()
    {
        if (loadingPanel != null)
            loadingPanel.SetActive(true);

        StartCoroutine(LoadSceneAsync());
    }

    public void ShowLoadingForSeconds(float seconds, string nextSceneName = null)
    {
        if (loadingPanel != null)
            loadingPanel.SetActive(true);

        StartCoroutine(ShowLoadingCoroutine(seconds, nextSceneName));
    }

    private IEnumerator LoadSceneAsync()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(loadingSceneName);

        while (!operation.isDone)
        {
            yield return null;
        }
    }

    private IEnumerator ShowLoadingCoroutine(float seconds, string nextSceneName)
    {
        yield return new WaitForSeconds(seconds);

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(nextSceneName);
            while (!operation.isDone)
            {
                yield return null;
            }
        }

        if (loadingPanel != null)
            loadingPanel.SetActive(false);
    }

    private IEnumerator ShowLoadingThenStartDialogue()
    {
        yield return new WaitForSeconds(loadScreenDuration);

        if (loadingPanel != null)
            loadingPanel.SetActive(false);

        if (chapterDialogue != null)
            chapterDialogue.StartDialogue();
    }
}
