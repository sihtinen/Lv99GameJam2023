using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class RadialMenuDebug : MonoBehaviour
{
    [SerializeField] private RadialMenu m_radialMenu = null;

    private void Start()
    {
        GenerateOptions();
    }

    [ContextMenu(itemName: "Generate Options")]
    public void GenerateOptions()
    {
        if (m_radialMenu == null)
        {
            Debug.LogError("RadialMenuDebug.GenerateOptions(): radial menu reference is null!");
            return;
        }

        var _optionsList = new List<IRadialMenuOption>();

        int _optionsCount = Random.Range(2, 9);

        for (int i = 0; i < _optionsCount; i++)
        {
            string _numberString = (i+1).ToStringMinimalAlloc();

            _optionsList.Add(new TestOption
            {
                UILabel = $"Test Option {_numberString}",
            });
        }

        m_radialMenu.PopulateMenu(_optionsList);
    }

    public class TestOption : IRadialMenuOption
    {
        public string UILabel;

        string IRadialMenuOption.UILabel => this.UILabel;
        GUIDWrapper IRadialMenuOption.GUID => this.m_guid;

        private GUIDWrapper m_guid = new GUIDWrapper();

        public void OnSelected()
        {
            Debug.Log($"RadialMenuDebug: {UILabel} selected");

            var _debugComponent = FindObjectOfType<RadialMenuDebug>();
            if (_debugComponent != null)
                _debugComponent.GenerateOptions();
        }
    }
}
