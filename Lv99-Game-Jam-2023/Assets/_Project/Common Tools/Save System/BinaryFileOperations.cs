using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;

namespace Tensori.SaveSystem
{
    public static class BinaryFileOperations
    {
        public static bool Write<T>(string filePath, T toSave)
        {
            try
            {
                using (Stream stream = File.Open(filePath, FileMode.Create))
                {
                    BinaryFormatter _binaryFormatter = new BinaryFormatter();
                    _binaryFormatter.Serialize(stream, toSave);
                }
            }
            catch (Exception exc)
            {
                Debug.LogError("Error while saving: Read ERRORS-file.\n" + exc);
                return false;
            }

            return true;
        }

        public static T ReadBinary<T>(string filePath)
        {
            try
            {
                using (Stream stream = File.Open(filePath, FileMode.Open))
                {
                    BinaryFormatter _binaryFormatter = new BinaryFormatter();
                    return (T)_binaryFormatter.Deserialize(stream);
                }
            }
            catch (Exception exc)
            {
                Debug.LogError("Error while loading: Read ERRORS-file.\n" + exc);
                return default(T);
            }
        }
    }
}
