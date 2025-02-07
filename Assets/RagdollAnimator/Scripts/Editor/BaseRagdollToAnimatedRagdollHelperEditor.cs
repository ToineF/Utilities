using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(BaseRagdollToAnimatedRagdollHelper))]
public class BaseRagdollToAnimatedRagdollHelperEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var data = (BaseRagdollToAnimatedRagdollHelper)target;

        base.OnInspectorGUI();

        EditorGUILayout.Space(25);

        if (GUILayout.Button("Replace Character Joints")) data.ReplaceCharacterJoints();

        if (GUILayout.Button("Set Animations")) data.SetAnimations();
    }
}