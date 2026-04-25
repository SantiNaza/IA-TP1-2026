using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class GoalZone : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text messageText;

    [Header("Detection")]
    public string playerTag = "Player";

    [Header("Timing")]
    public float missingBallsMessageDuration = 3f;
    public float restartDelay = 2f;

    private Coroutine clearMessageCoroutine;
    private bool hasWon;

    private void Reset()
    {
        // Ensure the goal area works as a trigger volume when added in the editor.
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasWon)
        {
            return;
        }

        // Only react when the player enters the goal trigger.
        if (!other.CompareTag(playerTag))
        {
            return;
        }

        if (GameManager.Instance == null)
        {
            SetMessage("GAME MANAGER NO DISPONIBLE");
            return;
        }

        if (GameManager.Instance.HasAllBalls())
        {
            hasWon = true;
            SetMessage("GANASTE!");

            if (clearMessageCoroutine != null)
            {
                StopCoroutine(clearMessageCoroutine);
                clearMessageCoroutine = null;
            }

            // Stop player control after winning.
            FirstPersonController controller = other.GetComponentInParent<FirstPersonController>();
            if (controller != null)
            {
                controller.enabled = false;
            }

            StartCoroutine(RestartLevelAfterDelay());
        }
        else
        {
            SetMessage("FALTAN BOLAS");

            if (clearMessageCoroutine != null)
            {
                StopCoroutine(clearMessageCoroutine);
            }

            clearMessageCoroutine = StartCoroutine(ClearMessageAfterDelay());
        }
    }

    private IEnumerator ClearMessageAfterDelay()
    {
        yield return new WaitForSeconds(missingBallsMessageDuration);
        SetMessage(string.Empty);
        clearMessageCoroutine = null;
    }

    private IEnumerator RestartLevelAfterDelay()
    {
        yield return new WaitForSeconds(restartDelay);
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    private void SetMessage(string text)
    {
        if (messageText != null)
        {
            messageText.text = text;
        }
        else
        {
            Debug.LogWarning($"GoalZone message: {text}", this);
        }
    }
}
