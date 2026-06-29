using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Drag your scene names here in the inspector
    public string[] scenesToLoad;

    void Start()
    {
        foreach (string sceneName in scenesToLoad)
        {
            // Load the scene additively so it doesn't destroy the current one
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }
    }
}