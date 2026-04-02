using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueLogic : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public float textSpeed;
    public string[] lines; 
    public bool talking;
    private int index;

    void Start()
    {
        textComponent.text = string.Empty;
        gameObject.SetActive(false);
        talking = false;
        
        // StartDialogue();
    }

    void Update()
    {   
        if (Input.GetMouseButtonDown(0))
        {
            
            if (textComponent.text == lines[index]) 
            {
                StartCoroutine(NextLine());
            }
            else 
            {
                
                StopAllCoroutines();
                textComponent.text = lines[index];
                StartCoroutine(NextLine());
            }

        }
        
    }


    public void StartDialogue() 
    {
        gameObject.SetActive(true);
        index = 0;
        textComponent.text = string.Empty;
        StartCoroutine(TypeLine());
        talking = true;

    }



    IEnumerator TypeLine()
    {
        foreach(char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        StartCoroutine(NextLine());
    }

    IEnumerator NextLine()
    {

        yield return new WaitForSeconds(1);
        if(index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            talking = false;
            gameObject.SetActive(false); 
        }
    }
}