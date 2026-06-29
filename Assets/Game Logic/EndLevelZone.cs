using UnityEngine;

public class EndLevelZone : MonoBehaviour
{
    public GameObject loadingPanel;
    public string nextLevelSceneName;
    public float loadingDuration = 4f;
    public Transform teleportTarget;
    public bool teleportPlayer = true;
    private static int _levelCounter = 1;
    public SpawnPoint spawnPoint;
    public static int LevelCounter 
    {
        get { return _levelCounter; }
        set { _levelCounter = value; }
    }

    private bool triggered = false;

    private void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggered) return;
                triggered = true;

        
        if (spawnPoint != null)
        {
            spawnPoint.IncrementLevel();
        }
    

        if (teleportPlayer && teleportTarget != null)
        {
            collision.transform.position = teleportTarget.position;
        }

        if (loadingPanel != null)
        {
            loadingPanel.SetActive(true);
        }

        if (!string.IsNullOrEmpty(nextLevelSceneName))
        {
            StartCoroutine(WaitAndLoadNextLevel());
        }
        else if (loadingPanel != null)
        {
            StartCoroutine(HideLoadingPanelAfterDelay());
        }
    }

    private System.Collections.IEnumerator WaitAndLoadNextLevel()
    {
        yield return new WaitForSeconds(loadingDuration);
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextLevelSceneName);
    }

    private System.Collections.IEnumerator HideLoadingPanelAfterDelay()
    {
        yield return new WaitForSeconds(loadingDuration);
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(false);
        }
    }
}
