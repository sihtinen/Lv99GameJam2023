using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public static class IntStringUtility
{
    private static bool m_initialized = false;
    private static Dictionary<int, string> m_preAllocatedStrings = null;
    private static Dictionary<int, string> m_alphabetStrings = null;

    private static void initialize()
    {
        m_preAllocatedStrings = new Dictionary<int, string>();
        m_alphabetStrings = new Dictionary<int, string>();
        
        for (int i = -10000; i < 10000; i++)
        {
            m_preAllocatedStrings.Add(i, i.ToString());
        }

        m_initialized = true;
    }

    public static string ToStringMinimalAlloc(this int value)
    {
        if (m_initialized == false)
            initialize();

        if (m_preAllocatedStrings.TryGetValue(value, out string _result))
            return _result;

        string _newString = value.ToString();
        m_preAllocatedStrings.Add(value, _newString);

        return _newString;
    }

    public static string ToAlphabetMinimalAlloc(this int value)
    {
        if (m_initialized == false)
            initialize();
        
        value++;
        int _originalValue = value;

        if (m_alphabetStrings.TryGetValue(value, out string _existingString))
            return _existingString;

        string _result = string.Empty;

        while (--value >= 0)
        {
            _result = (char)('A' + value % 26) + _result;
            value /= 26;
        }

        m_alphabetStrings.Add(_originalValue, _result);
        return _result;
    }
}
