using UnityEngine;

public class DialogueZone : MonoBehaviour
{
    public DialogueLogic dialogueLogic;
    
    [Header("Type what this specific zone says here!")]
    public string[] myLines;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        print("The player collided with the square!");
        dialogueLogic.StartDialogue();  
  
        
    }
}