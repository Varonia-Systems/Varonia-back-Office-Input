#if UNITY_EDITOR
using UnityEditor;

[InitializeOnLoad]
public static class InputDefineEnabler
{
    static InputDefineEnabler()
    {
        string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        if (!symbols.Contains("VBO_Input"))
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(
                EditorUserBuildSettings.selectedBuildTargetGroup,
                symbols + ";VBO_Input"
            );
        }
    }
}
#endif