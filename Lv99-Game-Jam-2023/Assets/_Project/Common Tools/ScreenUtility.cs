using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public static class ScreenUtility
{
    private static List<Resolution> m_cachedResolutionInfoList = new List<Resolution>();
    private static Dictionary<Vector2Int, int> m_resolutionListMapping = new Dictionary<Vector2Int, int>();

    public static List<Resolution> GetSupportedResolutions()
    {
        m_cachedResolutionInfoList.Clear();
        m_resolutionListMapping.Clear();

        var _screenResolutions = Screen.resolutions;

        for (int i = 0; i < _screenResolutions.Length; i++)
        {
            Resolution _currentResolution = _screenResolutions[i];
            Vector2Int _currentResolutionDimensions = new Vector2Int(_currentResolution.width, _currentResolution.height);

            if (m_resolutionListMapping.TryGetValue(_currentResolutionDimensions, out int _listIndex))
            {
                Resolution _listResolution = m_cachedResolutionInfoList[_listIndex];

                if (_currentResolution.refreshRate > _listResolution.refreshRate)
                    m_cachedResolutionInfoList[_listIndex] = _currentResolution;
            }
            else
            {
                m_cachedResolutionInfoList.Add(_currentResolution);
                m_resolutionListMapping.Add(_currentResolutionDimensions, m_cachedResolutionInfoList.IndexOf(_currentResolution));
            }
        }

        return m_cachedResolutionInfoList;
    }
}
