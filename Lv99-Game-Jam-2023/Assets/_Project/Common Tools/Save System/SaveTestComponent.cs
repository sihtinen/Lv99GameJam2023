using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Tensori.SaveSystem
{
    public class SaveTestComponent : MonoBehaviour
    {
        [System.Serializable]
        public class SaveableObject
        {
            public float Float;
            public char[] CharArray = null;
            public SaveableObject NestedSaveable = null;

            public override string ToString()
            {
                string _result = "Saveable Object: Float: " + Float + ", CharArray: " + new string(CharArray);

                if (NestedSaveable != null)
                    _result += ", nested: " + NestedSaveable.ToString();

                return _result;
            }
        }

        public enum TestMode
        {
            Save,
            Load
        }

        [Header("Settings")]
        public TestMode Mode = TestMode.Save;
        public string SaveFileName = "SaveData_Test";

        private SaveData m_saveData = null;

        private void Awake()
        {
            m_saveData = new SaveData(1.0);
        }

        private void Start()
        {
            SaveableObject _saveableObject;

            switch (Mode)
            {
                case TestMode.Save:

                    m_saveData.RegisterVariable("Int 01", 234);
                    m_saveData.RegisterVariable("Double 01", 532.55);
                    m_saveData.RegisterVariable("String 01", "justabunchofwords");
                    m_saveData.RegisterVariable("Bool 01", true);

                    _saveableObject = new SaveableObject();
                    _saveableObject.Float = 3.33333f;
                    _saveableObject.CharArray = new char[] { '3', 'a', ':' };
                    _saveableObject.NestedSaveable = new SaveableObject();
                    _saveableObject.NestedSaveable.Float = -7.2f;
                    _saveableObject.NestedSaveable.CharArray = new char[] { 'N', 'E', 'S', 'T' };

                    m_saveData.RegisterVariable("Object 01", _saveableObject);

                    SaveManager.SaveToFile(SaveFileName, m_saveData);

                    break;

                case TestMode.Load:

                    bool _loadSuccess = SaveManager.LoadFromFile(SaveFileName, ref m_saveData);

                    if (_loadSuccess)
                    {
                        _saveableObject = new SaveableObject();
                        m_saveData.ReadObject("Object 01", ref _saveableObject);
                    }

                    Debug.Log(m_saveData.ToString());

                    break;
            }
        }
    }
}
