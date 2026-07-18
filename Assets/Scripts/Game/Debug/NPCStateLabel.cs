using UnityEngine;
using TMPro;

public class NPCStateLabel : MonoBehaviour
{
    [Header("Configuracion del label")]
    [Tooltip("Altura sobre el pivot del NPC donde aparece el label")]
    public float heightOffset = 2.2f;
 
    [Tooltip("Tamaño del texto en unidades de mundo")]
    public float fontSize = 0.4f;
 
    [Header("Colores por estado")]
    public Color colorPatrol   = new Color(0.4f, 0.8f, 1f);    // celeste
    public Color colorIdle     = new Color(0.8f, 0.8f, 0.8f);  // gris
    public Color colorChase    = new Color(1f,   0.6f, 0f);     // naranja
    public Color colorAlert    = new Color(1f,   1f,   0f);     // amarillo
    public Color colorPathfinding = new Color(0.5f, 0.3f, 1f); // violeta
    public Color colorRunAway  = new Color(0.3f, 1f,   0.4f);  // verde
    public Color colorSafe     = new Color(0.2f, 0.9f, 0.5f);  // verde agua
    public Color colorAttack   = new Color(1f,   0.15f, 0.15f);// rojo
    public Color colorDefault  = Color.white;
 
    private TextMeshPro _tmp;
    private Transform _labelTransform;

    private GuardEnemyController  _guard;
    private CowardEnemyController _coward;
    private FlockLeader           _leader;
 
    private void Awake()
    {
        _guard  = GetComponent<GuardEnemyController>();
        _coward = GetComponent<CowardEnemyController>();
        _leader = GetComponent<FlockLeader>();
 
        CreateLabel();
    }
 
    private void CreateLabel()
    {
        var labelGO = new GameObject("StateLabel");
        labelGO.transform.SetParent(transform);
        labelGO.transform.localPosition = new Vector3(0f, heightOffset, 0f);
        _labelTransform = labelGO.transform;
 
        _tmp = labelGO.AddComponent<TextMeshPro>();
        _tmp.alignment  = TextAlignmentOptions.Center;
        _tmp.fontSize   = fontSize;
        _tmp.fontStyle  = FontStyles.Bold;
        _tmp.text       = "...";
    }
 
    private void LateUpdate()
    {
        if (Camera.main != null)
            _labelTransform.rotation = Camera.main.transform.rotation;
 
        string stateName = GetStateName();
        _tmp.text  = stateName;
        _tmp.color = GetStateColor(stateName);
    }
 
    private string GetStateName()
    {
        if (_guard  != null) return _guard.GetCurrentState();
        if (_coward != null) return _coward.GetCurrentState();
        if (_leader != null) return _leader.currentState.ToString();
        return "?";
    }
 
    private Color GetStateColor(string state)
    {
        switch (state.ToLower())
        {
            case "patrol":      return colorPatrol;
            case "idle":        return colorIdle;
            case "chase":       return colorChase;
            case "alert":       return colorAlert;
            case "pathfinding": return colorPathfinding;
            case "runaway":     return colorRunAway;
            case "safe":        return colorSafe;
            case "attack":      return colorAttack;
            default:            return colorDefault;
        }
    }
}