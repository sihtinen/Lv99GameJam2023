using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class AbilityCanvas : SingletonBehaviour<AbilityCanvas>
{
    [Header("Object References")]
    [SerializeField] private AvailableAbilitySlotElement m_slotElementPrefab = null;
    [SerializeField] private AvailableAbilityElement m_abilityElementPrefab = null;
    [Space]
    [SerializeField] private RectTransform m_slotGroupTransform = null;

    private List<AvailableAbilitySlotElement> m_activeSlots = new();
    private Stack<AvailableAbilitySlotElement> m_slotElementPool = new();
    private Stack<AvailableAbilityElement> m_abilityElementPool = new();

    private void Start()
    {
        for (int i = 0; i < 12; i++)
        {
            var _slotObj = Instantiate(m_slotElementPrefab.gameObject, parent: transform);
            _slotObj.SetActive(false);
            _slotObj.TryGetComponent(out AvailableAbilitySlotElement _slotComponent);
            m_slotElementPool.Push(_slotComponent);

            var _abilityObj = Instantiate(m_abilityElementPrefab.gameObject, parent: transform);
            _abilityObj.SetActive(false);
            _abilityObj.TryGetComponent(out AvailableAbilityElement _abilityComponent);
            m_abilityElementPool.Push(_abilityComponent);
        }
    }

    public void Clear()
    {
        for (int i = 0; i < m_activeSlots.Count; i++)
            m_activeSlots[i].Clear();

        m_activeSlots.Clear();
    }

    public void ReturnToPool(AvailableAbilitySlotElement element)
    {
        m_slotElementPool.Push(element);
    }

    public void ReturnToPool(AvailableAbilityElement element)
    {
        m_abilityElementPool.Push(element);
    }

    public void Initialize(int slotCount)
    {
        Clear();

        for (int i = 0; i < slotCount; i++)
        {
            var _newSlot = m_slotElementPool.Pop();
            _newSlot.transform.SetParent(m_slotGroupTransform);
            _newSlot.gameObject.SetActiveOptimized(true);
            m_activeSlots.Add(_newSlot);
        }
    }

    public void AddAbilityElement(AbilityTypes abilityType)
    {
        for (int i = 0; i < m_activeSlots.Count; i++)
        {
            var _slot = m_activeSlots[i];

            if (_slot.AbilityElementBinding == null)
            {
                var _newAbilityElement = m_abilityElementPool.Pop();
                _slot.BindToAbilityElement(_newAbilityElement, abilityType);
                break;
            }
        }

        reorderAbilityElements();
    }

    public void OnAbilityUsed(AbilityTypes abilityType)
    {
        for (int i = m_activeSlots.Count; i --> 0;)
        {
            var _slot = m_activeSlots[i];

            if (_slot.AbilityElementBinding == null)
                continue;

            if (_slot.AbilityElementBinding.AbilityType == abilityType)
            {
                _slot.Clear();
                m_activeSlots.RemoveAt(i);
                break;
            }
        }
    }

    private void reorderAbilityElements()
    {
        for (int abilityIndex = 0; abilityIndex < 4; abilityIndex++)
        {
            for (int slotIndex = 0; slotIndex < m_activeSlots.Count; slotIndex++)
            {
                var _slot = m_activeSlots[slotIndex];

                if (_slot.AbilityElementBinding == null)
                    continue;

                if (abilityIndex == (int)_slot.AbilityElementBinding.AbilityType)
                    _slot.transform.SetAsLastSibling();
            }
        }

        for (int slotIndex = 0; slotIndex < m_activeSlots.Count; slotIndex++)
        {
            var _slot = m_activeSlots[slotIndex];

            if (_slot.AbilityElementBinding == null)
                _slot.transform.SetAsLastSibling();
        }
    }
}
