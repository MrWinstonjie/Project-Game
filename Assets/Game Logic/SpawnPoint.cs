using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    // DRAG THESE IN THE INSPECTOR
    public Transform pos1;
    public Transform pos2;
    public Transform pos3;

    // This counter will survive because the object survives
    public int currentLevel = 1;

    void Awake()
    {
        // This keeps THIS object and its Inspector-assigned slots alive
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        MoveSpawnPoint();
    }

    // Call this from your EndLevelZone
    public void IncrementLevel()
    {
        currentLevel++;
        MoveSpawnPoint();
    }

    private void MoveSpawnPoint()
    {
        if (currentLevel == 2) transform.position = pos2.position;
        else if (currentLevel >= 3) transform.position = pos3.position;
        else transform.position = pos1.position;
    }
}