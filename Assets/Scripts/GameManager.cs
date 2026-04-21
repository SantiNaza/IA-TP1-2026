using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Progress")]
    public int totalBallsRequired = 3;

    [Header("UI")]
    public TMP_Text counterText;
    public TMP_Text messageText;

    [Header("Scene Objects")]
    public GameObject door;

    private int collectedBalls;
    private bool doorOpened;

    private void Awake()
    {
        // Basic singleton setup so other scripts can use GameManager.Instance safely.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        UpdateCounterUI();
        SetMessage(string.Empty);
    }

    public void AddBall()
    {
        // Prevent counting more than required.
        if (collectedBalls >= totalBallsRequired)
        {
            return;
        }

        collectedBalls++;
        UpdateCounterUI();

        if (collectedBalls >= totalBallsRequired)
        {
            OpenDoor();
        }
    }

    public bool HasAllBalls()
    {
        return collectedBalls >= totalBallsRequired;
    }

    private void UpdateCounterUI()
    {
        if (counterText != null)
        {
            counterText.text = $"{collectedBalls}/{totalBallsRequired}";
        }
    }

    private void OpenDoor()
    {
        if (doorOpened)
        {
            return;
        }

        doorOpened = true;

        if (door != null)
        {
            Destroy(door);
        }

        SetMessage("PUERTA ABIERTA");
    }

    private void SetMessage(string text)
    {
        if (messageText != null)
        {
            messageText.text = text;
        }
    }
}
