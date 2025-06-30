using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace HellVillage.Data {
    public class FileDataService : IDataService {
        private ISerializer _serializer;
        private string _dataPath;
        private string _fileExtension;

        public FileDataService(ISerializer serializer) {
            this._dataPath = Application.persistentDataPath + "/Saves/";
            this._fileExtension = ".json";
            this._serializer = serializer;
        }

        private string GetFilePathOrCreate(string fileName) {
            if (!Directory.Exists(_dataPath)) {
                Directory.CreateDirectory(_dataPath);
            }
            string fileLocation = Path.Combine(_dataPath, string.Concat(fileName, _fileExtension));
            fileLocation = GetLocalFilePath(fileName);
            return fileLocation;
        }

        private string GetLocalFilePath(string fileName) {
            string localPath = Application.dataPath + "/!HellVillage/Temp/";
            if (!Directory.Exists(localPath)) {
                Directory.CreateDirectory(localPath);
            }
            return Path.Combine(localPath, string.Concat(fileName, _fileExtension));
        }

        public void Save(GameData data, bool overwrite = true) {
            string fileLocation = GetFilePathOrCreate(data.Name);

            if (!overwrite && File.Exists(fileLocation)) {
                throw new IOException($"The file '{data.Name}{_fileExtension}' already exists and cannot be overwritten.");
            }

            File.WriteAllText(fileLocation, _serializer.Serialize(data));
        }

        public GameData Load(string name) {
            string fileLocation = GetFilePathOrCreate(name);
            Debug.Log($"Loading game data from: {fileLocation}");

            if (!File.Exists(fileLocation)) {
                throw new FileNotFoundException($"The file '{name}{_fileExtension}' does not exist at the specified location: {fileLocation}");
            }

            return _serializer.Deserialize<GameData>(File.ReadAllText(fileLocation));
        }

        public void Delete(string name) {
            string fileLocation = GetFilePathOrCreate(name);

            if (File.Exists(fileLocation)) {
                File.Delete(fileLocation);
            }
        }

        public void DeleteAll() {
            foreach (string filePath in Directory.GetFiles(_dataPath, $"*{_fileExtension}")) {
                File.Delete(filePath);
            }
        }

        public IEnumerable<string> ListSaves() {
            foreach (string path in Directory.EnumerateFiles(_dataPath, $"*{_fileExtension}")) {
                yield return Path.GetFileNameWithoutExtension(path);
            }
        }

        public bool SaveExists(string name) {
            string fileLocation = GetFilePathOrCreate(name);
            return File.Exists(fileLocation);
        }
    }
}
