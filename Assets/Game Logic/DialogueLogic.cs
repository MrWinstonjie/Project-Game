using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueLogic : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public float textSpeed;
    
    public string[] lines; 
    private int index;

    void Start()
    {
        textComponent.text = string.Empty;
   
        StartDialogue();
    }

    void Update()
    {   
        if (Input.GetMouseButtonDown(0))
        {
            
            if (textComponent.text == lines[index]) 
            {
                NextLine();
            }
            else 
            {
                
                StopAllCoroutines();
                textComponent.text = lines[index];
            }
        }
    }

    public void StartDialogue() 
    {
        gameObject.SetActive(true); // CRITICAL: Turn the UI on BEFORE starting coroutines
 
        index = 0;
        textComponent.text = string.Empty;
        StartCoroutine(TypeLine());
    }

    // public void StartDialogue(string[] newLines) 
    // {
    //     gameObject.SetActive(true); // CRITICAL: Turn the UI on BEFORE starting coroutines
    //     lines = newLines; // Take the text from the trigger zone
    //     index = 0;
    //     textComponent.text = string.Empty;
    //     StartCoroutine(TypeLine());
    // }


    IEnumerator TypeLine()
    {
        foreach(char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void NextLine()
    {
        if(index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            gameObject.SetActive(false); // Hide the UI when dialogue is over
        }
    }
}