namespace HellVillage.Scenes {
    /// <summary>
    /// Public enum untuk mengindeks scene yang ada di dalam game.
    /// Ini digunakan untuk menghindari penggunaan string yang raw dalam kode.
    /// Enum ini juga digunakan untuk mengindeks scene yang ada di dalam build settings.
    /// </summary>
    public enum ScenesEnum {
        _PersistentScene,
        BootScene,
        MainMenu,
        TestScenes,
    }

    public static class ScenesEnumExtensions {
        /// <summary>
        /// Get the scene name by its enum value.
        /// </summary>
        public static int GetSceneEnumByName(string sceneName) {
            if (System.Enum.TryParse(sceneName, out ScenesEnum sceneEnum)) {
                return (int)sceneEnum;
            }
            return -1; // Return -1 if the scene name is not found
        }
    }
}
