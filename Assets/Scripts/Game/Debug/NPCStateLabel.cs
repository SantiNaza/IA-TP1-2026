using UnityEngine;
using TMPro;

public class NPCStateLabel : MonoBehaviour
{
    [Header("Configuracion")]
    public float heightOffset = 2.2f;

    [Tooltip("Tamaño del texto")]
    public float fontSize = 0.4f;

    private TextMeshPro tmp;
    private Transform labelTransform;

    private GuardEnemyController guard;
    private CowardEnemyController coward;
    private FlockLeader leader;

    private void Awake()
    {
        guard = GetComponent<GuardEnemyController>();
        coward = GetComponent<CowardEnemyController>();
        leader = GetComponent<FlockLeader>();

        CreateLabel();
    }

    private void CreateLabel()
    {
        var labelGO = new GameObject("StateLabel");
        labelGO.transform.SetParent(transform);
        labelGO.transform.localPosition = new Vector3(0f, heightOffset, 0f);
        labelTransform = labelGO.transform;

        tmp = labelGO.AddComponent<TextMeshPro>();
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontSize = fontSize;
        tmp.fontStyle = FontStyles.Bold;
        tmp.text = "...";
    }

    private void LateUpdate()
    {
        if (Camera.main != null)
            labelTransform.rotation = Camera.main.transform.rotation;

        tmp.text = GetStateName();
    }

    private string GetStateName()
    {
        if (guard != null) return guard.GetCurrentState();
        if (coward != null) return coward.GetCurrentState();
        if (leader != null) return leader.currentState.ToString();

        return "?";
    }
}