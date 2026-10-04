using System;
using System.Collections.Generic;
using EditorAttributes;
using UnityEngine;
using UnityEngine.Serialization;
[Serializable]
public class ConstructionLevelData
{
    public int Level;
    public BigNumber UpgradeCost;
    public BigNumber HarvestValue;
    [FormerlySerializedAs("HarvestSpeed")]
    public float HarvestDurationPerCycle; // Time to harvest
    public float Duration;
}
[CreateAssetMenu(fileName = "Construction_UpgradePriceDefine", menuName = "Farmer/UpgradePriceDefine")]
public class ConstructionUpgradePriceDefine : ScriptableObject
{
    public List<ConstructionLevelData> upgradeLevelDatas;

    [Button("Create Upgrade Level Data")]
    public void CreateUpgradeLevelData(int levelCount = 10)
    {
        levelCount = Mathf.Max(0, levelCount);
        upgradeLevelDatas = new List<ConstructionLevelData>(levelCount);
        float harvestSpeed = 0f;
        float duration = 0f;

        for (int level = 1; level <= levelCount; level++)
        {
            harvestSpeed += UnityEngine.Random.Range(1, 4);
            duration += UnityEngine.Random.Range(1, 6);

            upgradeLevelDatas.Add(new ConstructionLevelData
            {
                Level = level,
                UpgradeCost = new BigNumber(UnityEngine.Random.Range(1, 10), level + 2),
                HarvestValue = new BigNumber(UnityEngine.Random.Range(1, 10), level),
                HarvestDurationPerCycle = harvestSpeed,
                Duration = duration
            });
        }
    }
}
