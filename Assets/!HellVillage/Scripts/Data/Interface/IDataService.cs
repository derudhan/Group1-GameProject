using System.Collections.Generic;
using UnityEngine;

namespace HellVillage.Data {
    public interface IDataService {
        void Save(GameData data, bool overwrite = true);
        GameData Load(string name);
        void Delete(string name);
        void DeleteAll();
        IEnumerable<string> ListSaves();
        bool SaveExists(string name);
    }
}
