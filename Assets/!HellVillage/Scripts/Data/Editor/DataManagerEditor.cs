using UnityEditor;
using UnityEngine;

namespace HellVillage.Data {
    [CustomEditor(typeof(DataManager))]
    public class DataManagerEditor : Editor {
        public override void OnInspectorGUI() {
            DataManager dataManager = (DataManager)target;
            string gameName = dataManager.gameData.Name;

            DrawDefaultInspector();

            if (GUILayout.Button("Save Game")) {
                dataManager.SaveGame();
            }

            if (GUILayout.Button("Load Game")) {
                dataManager.LoadGameOrCreate(gameName);
            }
        }
    }
}