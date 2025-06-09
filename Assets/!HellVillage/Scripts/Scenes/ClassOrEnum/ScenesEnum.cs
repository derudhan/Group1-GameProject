namespace HellVillage.Scenes {
    /// <summary>
    /// Public enum untuk mengindeks scene yang ada di dalam game.
    /// Ini digunakan untuk menghindari penggunaan string yang raw dalam kode.
    /// Enum ini juga digunakan untuk mengindeks scene yang ada di dalam build settings.
    /// </summary>
    public enum ScenesEnum : byte {
        BootScene = 0,
        MainMenu = 1,
        TestScenes = 2,
    }
}
