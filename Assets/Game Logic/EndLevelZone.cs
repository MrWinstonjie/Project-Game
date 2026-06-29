using UnityEngine;

public class EndLevelZone : MonoBehaviour
{
    public Loading loadingManager;
    public string nextLevelSceneName = "Level2";
    public float loadingDuration = 4f;
    public Vector3 teleportPosition;
    public bool teleportPlayer = true;
    public string playerTag = "Player";

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggered)
            return;

        if (!collision.CompareTag(playerTag))
            return;

        triggered = true;

        if (loadingManager != null)
        {
            loadingManager.ShowLoadingForSeconds(loadingDuration, nextLevelSceneName);
        }
        else if (!string.IsNullOrEmpty(nextLevelSceneName))
        {
            StartCoroutine(WaitAndLoadNextLevel());
        }
        else if (teleportPlayer)
        {
            collision.transform.position = teleportPosition;
        }
    }

    private System.Collections.IEnumerator WaitAndLoadNextLevel()
    {
        yield return new WaitForSeconds(loadingDuration);
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextLevelSceneName);
    }
}
