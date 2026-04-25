using UnityEngine;

public class Collectible : MonoBehaviour
{
    [Header("Detection")]
    public string playerTag = "Player";

    // Prevents multiple trigger events from collecting the same object twice.
    private bool isCollected;

    private void Reset()
    {
        // Ensure this object has a Collider set as trigger when the script is added.
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    private void Awake()
    {
        // Validate collider setup at runtime to avoid missing trigger events.
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogError("Collectible requires a Collider component.", this);
            enabled = false;
            return;
        }

        if (!col.isTrigger)
        {
            Debug.LogWarning("Collectible collider was not set as Trigger. Enabling Trigger automatically.", this);
            col.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isCollected)
        {
            return;
        }

        // Only collect when the player enters the trigger.
        if (!other.CompareTag(playerTag))
        {
            return;
        }

        isCollected = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddBall();
        }
        else
        {
            Debug.LogWarning("GameManager.Instance is null. Ball was collected but not counted.", this);
        }

        Destroy(gameObject);
    }
}
