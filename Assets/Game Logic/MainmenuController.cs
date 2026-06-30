using Microsoft.Unity.VisualStudio.Editor;
using Mono.Cecil.Cil;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainmenuController : MonoBehaviour
{
    // private ColorBlock color;
    
    void Start()
    {
        CleanupPersistentLevelObjects();
    }

    private void CleanupPersistentLevelObjects()
    {
        SpawnPoint[] spawnPoints = FindObjectsOfType<SpawnPoint>();
        foreach (SpawnPoint spawnPoint in spawnPoints)
        {
            if (spawnPoint != null)
            {
                Destroy(spawnPoint.gameObject);
            }
        }

        Loading[] loadingObjects = FindObjectsOfType<Loading>();
        foreach (Loading loading in loadingObjects)
        {
            if (loading != null)
            {
                Destroy(loading.gameObject);
            }
        }
    }

   
    void Update()
    {
        
    }

    public void onStartClick()
    {
        SceneManager.LoadScene("Level1Test");
    }

    public void onHover()
    {
        var colors = GetComponent<Button> ().colors;
        colors.normalColor = Color.red;
        GetComponent<Button> ().colors = colors;
    }

    public void onExitHover()
    {
        
    }


    public void onExitClick()
    {
        UnityEditor.EditorApplication.isPlaying = false;

        Application.Quit();
    }
}
