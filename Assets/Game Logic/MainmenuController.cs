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
