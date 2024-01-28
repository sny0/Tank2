using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyBrain))] // MyScript�ɓK�����X�N���v�g�ɕύX
public class CircleDrawer : Editor
{
    private void OnSceneGUI()
    {
        EnemyBrain script = target as EnemyBrain; // MyScript�ɓK�����X�N���v�g�ɕύX

        if (script == null)
            return;

        Handles.color = Color.green;
        Handles.DrawWireDisc(script.transform.position, Vector3.forward, script.SonarSearchRadius); // ���S���W�A�@���A���a
    }
}
