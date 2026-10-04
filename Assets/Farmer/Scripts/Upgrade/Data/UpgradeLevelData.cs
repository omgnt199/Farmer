using System;
using UnityEngine;
[Serializable]
public class UpgradeLevelData
{
    public int level;
    public BigNumber upgradePrice;
    public float duration;
    [Min(0f)] public float effectValue = 2f;
}
