using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(SaveGameManager))]
public class SaveGameManagerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SaveGameManager saveGameManager = (SaveGameManager)target;

        GUILayout.Space(10);

        Color origColor = GUI.backgroundColor;

        if (Application.isPlaying)
        {
            GUI.backgroundColor = new Color(0.64f, 0.90f, 0.52f);
            if (GUILayout.Button("Serialize", GUILayout.Height(35.0f)))
            {
                saveGameManager.Serialize();
            }
        }
        else
        {
            GUI.backgroundColor = new Color(0.5f, 0.5f, 0.5f);
            if (GUILayout.Button("Serialize (Runtime Only)", GUILayout.Height(35.0f)))
            {
                Debug.LogWarning("Saving a game only works at runtime!");
            }
        }


        GUI.backgroundColor = origColor;

        //GUILayout.Space(5);

        //if (world.Progress > 0.0f)
        //{
        //    Rect rect = EditorGUILayout.BeginVertical();
        //        EditorGUI.ProgressBar(rect, world.Progress, world.ProgressText);
        //        GUILayout.Space(16);
        //    EditorGUILayout.EndVertical();
        //}
    }

    //public override bool RequiresConstantRepaint()
    //{
    //    return true;
    //}
}