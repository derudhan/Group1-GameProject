using UnityEngine;

namespace HellVillage.Data {
    public class JsonBase64Serializer : ISerializer {
        public string Serialize<T>(T obj, bool prettyPrint = true) {
            string plainText = JsonUtility.ToJson(obj);
            byte[] plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public T Deserialize<T>(string json) {
            byte[] plainTextBytes = System.Convert.FromBase64String(json);
            string plainText = System.Text.Encoding.UTF8.GetString(plainTextBytes);
            return JsonUtility.FromJson<T>(plainText);
        }
    }
}
