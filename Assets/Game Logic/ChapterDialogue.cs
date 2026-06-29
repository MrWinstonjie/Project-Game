using UnityEngine;
using TMPro;
using System.Collections;

public class ChapterDialogue : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public float textSpeed;
    public float lineDelay = 1f;
    public string[] lines;
    public bool startOnAwake = false;

    private int index;

    void Start()
    {
        textComponent.text = "";
        gameObject.SetActive(startOnAwake);

        if (startOnAwake)
            StartDialogue();
    }

    public void StartDialogue()
    {
        gameObject.SetActive(true);
        index = 0;
        textComponent.text = "";
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        // Add a newline before every line except the first one
        if (index > 0)
        {
            textComponent.text += "\n";
        }

        foreach (char c in lines[index])
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        yield return new WaitForSeconds(lineDelay);

        if (index < lines.Length - 1)
        {
            index++;
            StartCoroutine(TypeLine());
        }
        else
        {
            yield return new WaitForSeconds(1f);
            gameObject.SetActive(false);
        }
    }
}