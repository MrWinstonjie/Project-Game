using System.Threading;
using Mono.Cecil.Cil;
using UnityEngine;

public class MapBarrier : MonoBehaviour
{
    public BarrierDialogue barrierDialogue;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            barrierDialogue.StartDialogue();
            print("You cannot Go there..");
        }
    }
}
