using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
namespace Farmer
{
    public class ConstructionMono : MonoBehaviour, IInteractable
    {
        private static readonly Vector3[] DefaultFruitOffsets =
        {
            new(0.28f, 1.28f, -0.18f),
            new(-0.22f, 1.52f, -0.15f),
            new(0.10f, 1.78f, -0.12f)
        };

        [SerializeField] private ConstructionDefinition definition;
        [SerializeField] private BoxCollider col;
        [SerializeField] private Transform harvestPoint;
        [SerializeField] private Transform deliveryPoint;
        private ConstructionEntity constructionEntity;
        private float productionTimer;
        private bool waitingForInstantHarvestRespawn;
        private bool hasProduct;
        private HarvestItem readyProduct;
        private readonly List<GameObject> productVisuals = new();

        public ConstructionEntity ConstructionEntity => constructionEntity;
        public BoxCollider Collider => col;
        public Transform HarvestPoint => harvestPoint != null ? harvestPoint : transform;
        public Transform DeliveryPoint => deliveryPoint != null ? deliveryPoint : transform;
        public bool HasProduct => hasProduct;
        public Vector3 ProductVisualStartPosition => productVisuals.Count > 0 && productVisuals[0] != null
            ? productVisuals[0].transform.position
            : HarvestPoint.position;

        public static event System.Action<ConstructionMono> ProductReady;

        void Awake()
        {
            col ??= GetComponent<BoxCollider>();
            ConfigureNavigationObstacle();

            if (definition == null)
            {
                Debug.LogError($"{name} is missing its ConstructionDefinition.", this);
                enabled = false;
                return;
            }

            constructionEntity = new ConstructionEntity(definition);
            deliveryPoint ??= transform.Find("Delivery");
            ConstructionManager.Register(constructionEntity);
            ResetProductionTimer();
        }

        private void ConfigureNavigationObstacle()
        {
            if (col == null)
                return;

            var obstacle = GetComponent<NavMeshObstacle>() ?? gameObject.AddComponent<NavMeshObstacle>();
            obstacle.shape = NavMeshObstacleShape.Box;
            obstacle.center = col.center;
            obstacle.size = col.size;
            obstacle.carving = true;
            obstacle.carveOnlyStationary = true;
        }

        void Start()
        {
            if (GameUI.Instance != null)
                GameUI.Instance.ShowConstructionInformation(this);
        }

        void OnDestroy()
        {
            ConstructionManager.Unregister(constructionEntity);
        }

        void Update()
        {
            if (hasProduct || constructionEntity == null)
                return;

            if (!GameManager.SkipConstructionHarvestSpeed || waitingForInstantHarvestRespawn)
            {
                productionTimer -= Time.deltaTime;
                if (productionTimer > 0f)
                    return;

                waitingForInstantHarvestRespawn = false;
            }

            readyProduct = constructionEntity.Harvest();
            hasProduct = true;
            ShowProductVisual();
            ProductReady?.Invoke(this);
        }

        public void OnTouch()
        {
            GameUI.Instance.ShowConstructionUpgradeLevelView(this, Upgrade);
        }

        private void Upgrade()
        {
            if (ConstructionUpgradeService.TryUpgrade(constructionEntity) && GameManager.Instance != null)
                GameManager.Instance.GameFX?.UpgradeTierFx(transform.position);
        }

        public HarvestItem Harvest() => constructionEntity.Harvest();

        public bool TryTakeReadyProduct(out HarvestItem product)
        {
            if (!hasProduct)
            {
                product = default;
                return false;
            }

            product = readyProduct;
            hasProduct = false;
            HideProductVisual();
            ResetProductionTimer(afterCollection: true);
            return true;
        }

        private void ShowProductVisual()
        {
            if (definition.productPrefab == null || productVisuals.Count > 0 && productVisuals[0] != null && productVisuals[0].activeSelf)
                return;

            var plant = transform.Find("Graphic/Content/Plant") ?? transform;
            for (int i = 0; i < DefaultFruitOffsets.Length; i++)
            {
                GameObject fruit = i < productVisuals.Count ? productVisuals[i] : null;
                if (fruit == null)
                {
                    fruit = Instantiate(definition.productPrefab, plant);
                    if (i < productVisuals.Count)
                        productVisuals[i] = fruit;
                    else
                        productVisuals.Add(fruit);
                }

                fruit.transform.localPosition = DefaultFruitOffsets[i];
                fruit.transform.localRotation = Quaternion.identity;
                fruit.transform.localScale = definition.productPrefab.transform.localScale;
                fruit.SetActive(true);
            }
        }

        private void HideProductVisual()
        {
            foreach (var fruit in productVisuals)
            {
                if (fruit != null)
                    fruit.SetActive(false);
            }
        }

        private void ResetProductionTimer(bool afterCollection = false)
        {
            waitingForInstantHarvestRespawn = afterCollection && GameManager.SkipConstructionHarvestSpeed;
            productionTimer = waitingForInstantHarvestRespawn
                ? Random.Range(0.5f, 1f)
                : GameManager.SkipConstructionHarvestSpeed
                    ? 0f
                    : Mathf.Max(0.1f, constructionEntity.GetHarvestDuration());
        }
    }
}
