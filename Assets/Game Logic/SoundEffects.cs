using UnityEngine;

public class SoundEffects : MonoBehaviour
{
    public AudioSource src;
    public AudioClip musik;


    void Start()
    {
        src.clip = musik;
        src.Play();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
