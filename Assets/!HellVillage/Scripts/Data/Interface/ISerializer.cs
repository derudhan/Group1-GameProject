namespace HellVillage.Data {
    public interface ISerializer {
        string Serialize<T>(T obj, bool prettyPrint = true);
        T Deserialize<T>(string json);
    }
}
