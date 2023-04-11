using System.IO;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Tensori.SaveSystem
{
    public static class SaveManager
    {
        public enum SaveFileFormat
        {
            Binary,
            Json
        }

        private static readonly string s_systemIODataPath = "/SaveData/";
        private static readonly string s_fileType_bytes = ".bytes";
        private static readonly string s_fileType_json = ".json";

        public static void SaveToFile(string fileName, SaveData saveData, SaveFileFormat format = SaveFileFormat.Json)
        {
            string _filePath = Application.persistentDataPath + s_systemIODataPath;

            if (Directory.Exists(_filePath) == false)
                Directory.CreateDirectory(_filePath);

            string _fileStreamPath = null;
            string _saveDate;

            switch (format)
            {
                case SaveFileFormat.Binary:
                    _saveDate = SaveBackupUtility.GetSaveDateAndDeleteOldBackup(_filePath, fileName, s_fileType_bytes);
                    _fileStreamPath = _filePath + fileName + _saveDate + s_fileType_bytes;
                    break;
                case SaveFileFormat.Json:
                    _saveDate = SaveBackupUtility.GetSaveDateAndDeleteOldBackup(_filePath, fileName, s_fileType_json);
                    _fileStreamPath = _filePath + fileName + _saveDate + s_fileType_json;
                    break;
            }

            switch (format)
            {
                case SaveFileFormat.Binary:
                    SaveDataCompressed _compressedSaveFile = new SaveDataCompressed(saveData.SerializeToByteArray(true));
                    BinaryFileOperations.Write(_fileStreamPath, _compressedSaveFile);
                    break;
                case SaveFileFormat.Json:
                    JsonFileOperations.Write(_fileStreamPath, saveData);
                    break;
            }
        }

        public static bool LoadFromFile(string fileName, ref SaveData populateSource, SaveFileFormat format = SaveFileFormat.Json)
        {
            string _filePath = Application.persistentDataPath + s_systemIODataPath;

            if (Directory.Exists(_filePath) == false)
                Directory.CreateDirectory(_filePath);

            string _fileType = null;

            switch (format)
            {
                case SaveFileFormat.Binary:
                    _fileType = s_fileType_bytes;
                    break;
                case SaveFileFormat.Json:
                    _fileType = s_fileType_json;
                    break;
            }

            SaveBackupUtility.BackupLoadResult _backupResult = SaveBackupUtility.GetNewestSaveFile(_filePath, fileName, _fileType);
            bool _loadOperationSuccess = false;

            while (_loadOperationSuccess == false && _backupResult.FileFound == true)
            {
                switch (format)
                {
                    case SaveFileFormat.Binary:

                        try
                        {
                            SaveDataCompressed _compressed = BinaryFileOperations.ReadBinary<SaveDataCompressed>(_backupResult.FilePath);
                            populateSource = _compressed.CompressedBytes.DeserializeFromByteArray<SaveData>(true);
                            _loadOperationSuccess = true;
                        }
                        catch (System.Exception ex)
                        {
                            Debug.LogError(ex);
                        }

                        break;
                    case SaveFileFormat.Json:
                        _loadOperationSuccess = JsonFileOperations.ReadJsonFromFile(_backupResult.FilePath, ref populateSource);
                        break;
                }

                if (_loadOperationSuccess)
                {
                    Debug.Log($"SaveManager: file '{fileName}{_fileType}' loaded");
                    return true;
                }
                else
                {
                    Debug.LogError($"SaveManager: file corrupted, deleting file:\n{_backupResult.FilePath}");

                    if (File.Exists(_backupResult.FilePath))
                        File.Delete(_backupResult.FilePath);

                    _backupResult = SaveBackupUtility.GetNewestSaveFile(_filePath, fileName, _fileType);
                }
            }

            Debug.Log($"SaveManager: no valid save file found for {fileName}{_fileType}");

            return false;
        }
    }
}