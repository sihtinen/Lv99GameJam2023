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
}
