using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private CharacterData m_characterData;

    [SerializeField] private TMP_Text woodCounterText;
    [SerializeField] private TMP_Text strengthValueText;
    [SerializeField] private TMP_Text attackSpeedValueText;

    private void Start()
    {
        RefreshWoodCounter();
        ActualizeGearPanel();
    }

    public void RefreshWoodCounter()
    {
        woodCounterText.text = m_characterData.WoodCounter.ToString();
    }

    public void ActualizeGearPanel()
    {
        strengthValueText.text = m_characterData.Strength.CurrentValue.ToString("F2");
        attackSpeedValueText.text = m_characterData.AttackPerSecond.CurrentValue.ToString("F2");
    }

    public void IncreaseAttackPerSecondLevel()
    {
        if (!m_characterData.TryLevelUpAttackPerSecond())
        {
            var attackPerSecondUpgradeCost = m_characterData.GetNextLevelUpCost(m_characterData.AttackPerSecond);
            Debug.Log($"Wood required: {attackPerSecondUpgradeCost.woodCost}, Rocks required: {attackPerSecondUpgradeCost.rockCost}");
        }
        ActualizeGearPanel();
        RefreshWoodCounter();
    }
    
    public void IncreaseStrengthLevel()
    {
        if (!m_characterData.TryLevelUpStrength())
        {
            var strengthUpgradeCost = m_characterData.GetNextLevelUpCost(m_characterData.Strength);
            Debug.Log($"Wood required: {strengthUpgradeCost.woodCost}, Rocks required: {strengthUpgradeCost.rockCost}");
        }
        ActualizeGearPanel();
        RefreshWoodCounter();
    }
}
