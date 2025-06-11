namespace HellVillage.Scenes {
    /// <summary>
    /// Data struktur untuk menyimpan informasi tentang operasi scene yang sedang dilakukan.
    /// Operasi ini bisa berupa loading atau unloading scene.
    /// </summary>
    public struct SceneOperation {
        public string ID;

        public int SceneIndex;
        public int ActiveSceneIndex;

        // Mendeklarasikan apakah operasi ini merupakan operasi untuk loading atau untuk unloading.
        public bool IsLoadingOperation;
    }
}
