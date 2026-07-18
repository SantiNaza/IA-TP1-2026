using TMPro;
using UnityEngine;

public class GlobalEnemyDebugUI : MonoBehaviour
{
    public TextMeshProUGUI debugText;
    public GuardEnemyController[] guards;
    public CowardEnemyController[] cowards;

    private bool showDebug = false;

    void Start()
    {
        if (debugText != null)
            debugText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            showDebug = !showDebug;

            if (debugText != null)
                debugText.gameObject.SetActive(showDebug);
        }

        if (!showDebug) return;
        if (debugText == null) return;

        string msg = "";

        if (guards != null)
        {
            for (int i = 0; i < guards.Length; i++)
            {
                if (guards[i] != null)
                    msg += $"Guard {i}: {guards[i].GetCurrentState()}\n";
            }
        }

        if (cowards != null)
        {
            for (int i = 0; i < cowards.Length; i++)
            {
                if (cowards[i] != null)
                    msg += $"Coward {i}: {cowards[i].GetCurrentState()}\n";
            }
        }

        debugText.text = msg;
    }
}