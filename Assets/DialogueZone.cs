using UnityEngine;

public class DialogueZone : MonoBehaviour
{
    public DialogueLogic dialogueLogic;
    public enum InteractionType 
    {
        Interaction,
        Trigger,
    }
    public InteractionType CurrentInteractionType;

    [Header("Type what this specific zone says here!")]
    public string[] myLines;
    private bool triggered;
    public GameObject interactPromptUI;
    private bool isPlayerInRange = false;

    private void Update()
    {
        if (isPlayerInRange && CurrentInteractionType == InteractionType.Interaction)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                print("The player Interacted with something");
                dialogueLogic.StartDialogue();
                
                if (interactPromptUI != null) interactPromptUI.SetActive(false);
                
                gameObject.SetActive(false); 
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) 
        {
            switch (CurrentInteractionType)
            {
                case InteractionType.Interaction:
                    if (interactPromptUI != null) interactPromptUI.SetActive(true);
                    
                    isPlayerInRange = true; 
                    break;
                    
                default:
                    if (!triggered)
                    {
                        dialogueLogic.StartDialogue();
                        gameObject.SetActive(false);
                    }
                    break;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (interactPromptUI != null)
            {
                interactPromptUI.SetActive(false);
            }
            
            isPlayerInRange = false; 
        }
    }
}