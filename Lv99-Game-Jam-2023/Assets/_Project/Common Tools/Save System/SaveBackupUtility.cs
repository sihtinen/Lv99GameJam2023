using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

using UnityEngine;

namespace Tensori.SaveSystem
{
    public static class SaveBackupUtility
    {
        private static string DATE_FORMAT = "yyyy-MM-dd-HH-mm-ss";
        private static int BACKUP_LIMIT = 6;
        private static List<SaveFileInstance> m_allFoundFileInstances = new List<SaveFileInstance>();

        private struct SaveFileInstance
        {
            public DateTime SaveTime;
            public string FullFilePath;
        }

        public struct BackupLoadResult
        {
            public bool FileFound;
            public string FilePath;
        }

        public static string GetSaveDateAndDeleteOldBackup(string folderPath, string fileName, string fileType)
        {
            findAndCollectAllSaveFileInstances(folderPath, fileName, fileType);

            if (m_allFoundFileInstances.Count >= BACKUP_LIMIT)
            {
                while (m_allFoundFileInstances.Count >= BACKUP_LIMIT)
                {
                    SaveFileInstance _oldest = m_allFoundFileInstances[0];

                    for (int i = 1; i < m_allFoundFileInstances.Count; i++)
                    {
                        if (m_allFoundFileInstances[i].SaveTime < _oldest.SaveTime)
                        {
                            _oldest = m_allFoundFileInstances[i];
                        }
                    }

                    if (File.Exists(_oldest.FullFilePath))
                        File.Delete(_oldest.FullFilePath);

                    m_allFoundFileInstances.Remove(_oldest);
                }
            }

            string _result = DateTime.Now.ToString(DATE_FORMAT);
            return "_" + _result;
        }

        public static BackupLoadResult GetNewestSaveFile(string folderPath, string fileName, string fileType)
        {
            findAndCollectAllSaveFileInstances(folderPath, fileName, fileType);

            BackupLoadResult _result = new BackupLoadResult { FileFound = false, FilePath = string.Empty };

            if (m_allFoundFileInstances.Count > 0)
            {
                SaveFileInstance _newest = m_allFoundFileInstances[0];

                for (int i = 1; i < m_allFoundFileInstances.Count; i++)
                {
                    if (m_allFoundFileInstances[i].SaveTime > _newest.SaveTime)
                    {
                        _newest = m_allFoundFileInstances[i];
                    }
                }

                _result.FileFound = true;
                _result.FilePath = _newest.FullFilePath;
            }

            return _result;
        }

        private static void findAndCollectAllSaveFileInstances(string folderPath, string fileName, string fileType)
        {
            string[] _fileArray = Directory.GetFiles(folderPath, $"*{fileType}");
            m_allFoundFileInstances.Clear();

            if (_fileArray != null)
            {
                foreach (string _filePathInstance in _fileArray)
                {
                    if (_filePathInstance.Contains(fileName))
                    {
                        string _rawFileName = _filePathInstance.Substring(_filePathInstance.LastIndexOf(fileName));

                        if (_rawFileName.Contains(fileName + "_"))
                        {
                            string _saveDateAsString = _rawFileName.Replace(fileName + "_", string.Empty).Replace(fileType, string.Empty);
                            DateTime _date = DateTime.ParseExact(_saveDateAsString, DATE_FORMAT, null);
                            m_allFoundFileInstances.Add(new SaveFileInstance { SaveTime = _date, FullFilePath = _filePathInstance });
                        }
                        else
                        {
                            Debug.LogWarning("SaveBackupUtility: invalid file found, deleting file:\n" + _filePathInstance);

                            if (File.Exists(_filePathInstance))
                                File.Delete(_filePathInstance);
                        }
                    }
                }
            }
        }
    }
}
