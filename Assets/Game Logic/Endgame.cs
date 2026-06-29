using UnityEngine;
using UnityEngine.SceneManagement;

public class Endgame : MonoBehaviour
{
    // Drag the Boss into this slot in the Inspector
    public GameObject boss;

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Check if the thing that left the trigger is the boss
        if (collision.gameObject == boss)
        {
            Debug.Log("you've avenged ur dad");
            
            // Load your main menu
            SceneManager.LoadScene("MainMenu");
        }
    }
}