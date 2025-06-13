using UnityEngine;

[CreateAssetMenu(menuName = "Varonia/Addon Config/Varonia Input Settings")]
public class VaroniaInputSettings : ScriptableObject
{
    [Header("Parameter")]
    public bool showDebugRenderInit;
    public bool hideDebugRenderAfterChangeScene;
}
