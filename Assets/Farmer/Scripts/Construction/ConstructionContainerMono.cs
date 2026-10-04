using Farmer;
using UnityEngine;
using UnityEngine.AI;
public class ConstructionContainerMono : MonoBehaviour, IInteractable
{
    private const string GROUND_LAYER = "Ground";

    [SerializeField] private ConstructionDefinition constructionDef;
    [SerializeField] private Animation anim;

    const string OPEN_ANIMATION_CLIP_INDEX = "BoxOpen";
    private bool isUnlocking;

    public ConstructionDefinition ConstructionDefinition => constructionDef;

    private void Awake()
    {
        var collider = GetComponent<BoxCollider>();
        if (collider == null)
            return;

        var obstacle = GetComponent<NavMeshObstacle>() ?? gameObject.AddComponent<NavMeshObstacle>();
        obstacle.shape = NavMeshObstacleShape.Box;
        obstacle.center = collider.center;
        obstacle.size = collider.size;
        obstacle.carving = true;
        obstacle.carveOnlyStationary = true;
    }

    public void OnTouch()
    {
        if (!isUnlocking && GameUI.Instance != null)
            GameUI.Instance.ShowConstructionBuildView(this, TryUnlock);
    }

    private bool TryUnlock()
    {
        if (isUnlocking || constructionDef == null || constructionDef.prefab == null ||
            GameUI.Instance == null || !EconomyManager.TrySpend(constructionDef.unlockPrice))
            return false;

        isUnlocking = true;
        GameUI.Instance.ShowConstructionUnlockCounterView(this, OnUnlockDone);
        if (anim != null)
            anim.Play(OPEN_ANIMATION_CLIP_INDEX);

        return true;
    }

    private void OnUnlockDone()
    {
        SpawnConstructionMono();
        Destroy(gameObject);
    }

    private void SpawnConstructionMono()
    {
        var constructionMono = Instantiate(constructionDef.prefab, transform.position, transform.rotation);
        var boxCollider = constructionMono.Collider;

        if (boxCollider == null)
        {
            Debug.LogError($"{constructionMono.name} is missing its BoxCollider reference.", constructionMono);
            return;
        }

        Vector3 rayOrigin = boxCollider.bounds.center + Vector3.up * 100f;

        if (!Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, Mathf.Infinity,
                LayerMask.GetMask(GROUND_LAYER), QueryTriggerInteraction.Ignore))
        {
            Debug.LogWarning($"Cannot find ground below {constructionMono.name}.", constructionMono);
            return;
        }

        constructionMono.transform.position += Vector3.up * (hit.point.y - boxCollider.bounds.min.y);
        if (GameManager.Instance != null)
            GameManager.Instance.GameFX?.UnlockConstructionFx(constructionMono.transform.position);
    }

}
