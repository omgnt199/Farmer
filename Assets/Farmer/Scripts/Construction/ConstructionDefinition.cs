using UnityEngine;
using System;
namespace Farmer
{
    [CreateAssetMenu(fileName = "Construction_ConstructionDefinition", menuName = "Farmer/ConstructionDefinition")]
    public class ConstructionDefinition : ScriptableObject
    {
        public string id;
        public string displayName;
        public Sprite icon;
        public GameObject productPrefab;
        public ConstructionMono prefab;
        public BigNumber unlockPrice;
        public float unlockDuration;
        public ConstructionUpgradePriceDefine constructionUpgradePriceDefine;

    }
}
