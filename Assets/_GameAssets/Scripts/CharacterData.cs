using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttributeData
{
    public int level = 1;
    public AnimationCurve valueCurve = new AnimationCurve(new Keyframe(0, 1));
    public AnimationCurve woodCostCurve = new AnimationCurve(new Keyframe(1, 10));
    public AnimationCurve rockCostCurve = new AnimationCurve(new Keyframe(1, 5));

    public float CurrentValue => valueCurve.Evaluate(level);
    
    public ResourceCost CurrentLevelUpCost => new ResourceCost(
        Mathf.FloorToInt(woodCostCurve.Evaluate(level)),
        Mathf.FloorToInt(rockCostCurve.Evaluate(level))
    );
}
    
[System.Serializable]
public class ResourceCost
{
    public int woodCost;
    public int rockCost;

    public ResourceCost(int wood, int rock)
    {
        woodCost = wood;
        rockCost = rock;
    }
}
[CreateAssetMenu(fileName = "NewCharacterStats", menuName = "Character Stats", order = 1)]
public class CharacterData : ScriptableObject
{
    [SerializeField] private float m_maxSpeed = 1f;
    public float MaxSpeed
    {
        get => m_maxSpeed;
        set => m_maxSpeed = value;
    }

    [SerializeField] private AttributeData m_strength = new AttributeData();
    public AttributeData Strength => m_strength;

    [SerializeField] private AttributeData m_attackPerSecond = new AttributeData();
    public AttributeData AttackPerSecond => m_attackPerSecond;

    [SerializeField] private int woodCount;
    public int WoodCounter
    {
        get => woodCount;
        set => woodCount = value;
    }
    
    [SerializeField] private int rockCount;
    public int RockCounter
    {
        get => rockCount;
        set => rockCount = value;
    }

    private bool TryLevelUpAttribute(AttributeData attribute)
    {
        var cost = attribute.CurrentLevelUpCost;

        if (woodCount >= cost.woodCost && rockCount >= cost.rockCost)
        {
            woodCount -= cost.woodCost;
            rockCount -= cost.rockCost;
            attribute.level++;
            return true;
        }
        return false;
    }
    
    public bool TryLevelUpStrength()
    {
        return TryLevelUpAttribute(Strength);
    }
    
    public bool TryLevelUpAttackPerSecond()
    {
        return TryLevelUpAttribute(AttackPerSecond);
    }
    
    public ResourceCost GetNextLevelUpCost(AttributeData attribute)
    {
        return attribute.CurrentLevelUpCost;
    }
}