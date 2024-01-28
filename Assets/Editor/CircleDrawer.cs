using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyBrain))] // MyScriptに適したスクリプトに変更
public class CircleDrawer : Editor
{
    private void OnSceneGUI()
    {
        EnemyBrain script = target as EnemyBrain; // MyScriptに適したスクリプトに変更

        if (script == null)
            return;

        Handles.color = Color.green;
        Handles.DrawWireDisc(script.transform.position, Vector3.forward, script.SonarSearchRadius); // 中心座標、法線、半径
    }
}
