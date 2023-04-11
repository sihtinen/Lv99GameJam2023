using System;
using System.Collections;
using System.Collections.Generic;

namespace Tensori.SaveSystem
{
    [System.Serializable]
    public class SaveData
    {
        public SaveData(double versionNumber)
        {
            Version = versionNumber;

            Data_Int = new Dictionary<string, int>();
            Data_Double = new Dictionary<string, double>();
            Data_String = new Dictionary<string, string>();
            Data_Bool = new Dictionary<string, bool>();
            Data_Object = new Dictionary<string, object>();
        }

        public double Version;

        public Dictionary<string, int> Data_Int;
        public Dictionary<string, double> Data_Double;
        public Dictionary<string, string> Data_String;
        public Dictionary<string, bool> Data_Bool;
        public Dictionary<string, object> Data_Object;

        public void RegisterVariable<T>(string id, T value)
        {
            switch (value)
            {
                case int _integer:

                    if (Data_Int.ContainsKey(id))
                        Data_Int[id] = _integer;
                    else
                        Data_Int.Add(id, _integer);

                    break;

                case double _double:

                    if (Data_Double.ContainsKey(id))
                        Data_Double[id] = _double;
                    else
                        Data_Double.Add(id, _double);

                    break;

                case string _string:

                    if (Data_String.ContainsKey(id))
                        Data_String[id] = _string;
                    else
                        Data_String.Add(id, _string);

                    break;

                case bool _boolean:

                    if (Data_Bool.ContainsKey(id))
                        Data_Bool[id] = _boolean;
                    else
                        Data_Bool.Add(id, _boolean);

                    break;

                case object _object:

                    if (Data_Object.ContainsKey(id))
                        Data_Object[id] = _object;
                    else
                        Data_Object.Add(id, _object);

                    break;
            }
        }

        public ValueTuple<bool, int> ReadInt(string id)
        {
            bool _success = Data_Int.TryGetValue(id, out int _result);
            return (_success, _result);
        }

        public ValueTuple<bool, double> ReadDouble(string id)
        {
            bool _success = Data_Double.TryGetValue(id, out double _result);
            return (_success, _result);
        }

        public ValueTuple<bool, string> ReadString(string id)
        {
            bool _success = Data_String.TryGetValue(id, out string _result);
            return (_success, _result);
        }

        public ValueTuple<bool, bool> ReadBool(string id)
        {
            bool _success = Data_Bool.TryGetValue(id, out bool _result);
            return (_success, _result);
        }

        public bool ReadObject<T>(string id, ref T result) where T : class
        {
            bool _success = Data_Object.TryGetValue(id, out object _serializedObjectAsString);
            if (_success)
            {
                _success = JsonFileOperations.ConvertFromString(_serializedObjectAsString.ToString(), result);

                if (_success) 
                    return true;
            }

            result = default(T);
            return false;
        }

        public void ClearRegisteredData()
        {
            Data_Bool.Clear();
            Data_Double.Clear();
            Data_Int.Clear();
            Data_Object.Clear();
            Data_String.Clear();
        }

        public override string ToString()
        {
            string _result = $"Save Data Version {Version}\n";

            foreach (KeyValuePair<string, int> entry in Data_Int)
            {
                _result += $"{entry.Key}: {entry.Value}\n";
            }
            foreach (KeyValuePair<string, double> entry in Data_Double)
            {
                _result += $"{entry.Key}: {entry.Value}\n";
            }
            foreach (KeyValuePair<string, string> entry in Data_String)
            {
                _result += $"{entry.Key}: {entry.Value}\n";
            }
            foreach (KeyValuePair<string, bool> entry in Data_Bool)
            {
                _result += $"{entry.Key}: {entry.Value}\n";
            }
            foreach (KeyValuePair<string, object> entry in Data_Object)
            {
                _result += $"{entry.Key}: {entry.Value}\n";
            }

            return _result;
        }
    }

    [System.Serializable]
    public class SaveDataCompressed
    {
        public byte[] CompressedBytes;

        public SaveDataCompressed(byte[] compressedBytes)
        {
            CompressedBytes = compressedBytes;
        }
    }
}