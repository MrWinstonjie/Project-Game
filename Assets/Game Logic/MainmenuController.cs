using UnityEngine;
using UnityEngine.SceneManagement;

public class MainmenuController : MonoBehaviour
{
    
    void Start()
    {
        
    }

   
    void Update()
    {
        
    }

    public void onStartClick()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void onExitClick()
    {
        UnityEditor.EditorApplication.isPlaying = false;

        Application.Quit();
    }
}
