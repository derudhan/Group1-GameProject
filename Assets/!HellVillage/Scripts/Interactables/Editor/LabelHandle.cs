using HellVillage.InteractionSystem;
using UnityEditor;
using UnityEngine;

namespace HellVillage.InteractionSystem {
    [CustomEditor(typeof(SceneChangerInteraction))]
    class LabelHandle : Editor {
        private static GUIStyle labelStyle;

        private void OnEnable() {
            labelStyle = new GUIStyle();
            labelStyle.normal.textColor = Color.white;
            labelStyle.alignment = TextAnchor.MiddleCenter;
        }

        private void OnSceneGUI() {
            SceneChangerInteraction areaChanger = (SceneChangerInteraction)target;

            Handles.BeginGUI();
            Handles.Label(areaChanger.transform.position + new Vector3(0f, 4f, 0f), areaChanger.CurrentAreaPosition.ToString(), labelStyle);
            Handles.EndGUI();
        }


    }
}