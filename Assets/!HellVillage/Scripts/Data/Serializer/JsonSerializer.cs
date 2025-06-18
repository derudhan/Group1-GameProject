using UnityEngine;

namespace HellVillage.Data {
    public class JsonSerializer : ISerializer {
        public string Serialize<T>(T obj, bool prettyPrint = true) {
            return JsonUtility.ToJson(obj, prettyPrint);
        }

        public T Deserialize<T>(string json) {
            return JsonUtility.FromJson<T>(json);
        }
    }
}
