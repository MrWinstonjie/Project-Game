using UnityEngine;

public class DialogueZone : MonoBehaviour
{
    public DialogueLogic dialogueLogic;
    
    [Header("Type what this specific zone says here!")]
    public string[] myLines;

    private bool triggered;

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (!triggered)
        {
            print("The player collided with the square!");
            dialogueLogic.StartDialogue();
            gameObject.SetActive(false);
        }
        
    }
}