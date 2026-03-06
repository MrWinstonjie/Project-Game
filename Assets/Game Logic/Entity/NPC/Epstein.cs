using UnityEngine;
using Pathfinding;

public class Epstein : Entity
{
    private Transform player;
    protected float activationRange = 5f;
    private AIDestinationSetter pathScript;
    private bool isChasing = false; 

    void Start()
    {
        pathScript = GetComponent<AIDestinationSetter>();
        pathScript.enabled = false; 

        GameObject p = GameObject.Find("Player");
        if (p != null)
            player = p.transform;
        else
            Debug.LogError("No Player object found in the scene!");
    }

    void Update()
    {
        if (player == null) return;

        // use normal distance, easy to reason about
        float distance = Vector3.Distance(player.position, transform.position);

        if (!isChasing && distance <= activationRange)
        {
            pathScript.enabled = true;
            isChasing = true;
        }
        else if (isChasing && distance > activationRange)
        {
            pathScript.enabled = false;
            isChasing = false;
        }

        // optional debug
        Debug.DrawLine(transform.position, player.position, Color.red);
    }
}