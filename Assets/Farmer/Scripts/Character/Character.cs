using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class Character : MonoBehaviour
{
    [SerializeField] private CharacterDefinition definition;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;

    private readonly Queue<CharacterTask> tasks = new();
    private readonly List<GameObject> carriedProducts = new();
    private readonly List<Transform> carryPoints = new();
    private CharacterNavigation navigation;
    private CharacterView characterView;
    private float workTimer;
    private GameObject carriedProductPrefab;
    private Transform carryRoot;
    private bool useFallbackCarryPoints;
    private Sequence carryTween;

    public CharacterEntity Entity { get; private set; }
    public CharacterTask CurrentTask => Entity?.CurrentTask;

    protected NavMeshAgent Agent => agent;

    protected void SetCarryingProduct(GameObject productPrefab)
    {
        SetCarryingProduct(productPrefab, transform.position, null);
    }

    protected void SetCarryingProduct(GameObject productPrefab, Vector3 startPosition)
    {
        SetCarryingProduct(productPrefab, startPosition, null);
    }

    protected void SetCarryingProduct(GameObject productPrefab, Vector3 startPosition, System.Action onComplete)
    {
        carryTween?.Kill();
        carryTween = null;

        if (productPrefab == null)
        {
            onComplete?.Invoke();
            return;
        }

        if (carriedProductPrefab != productPrefab)
        {
            foreach (var oldProduct in carriedProducts)
            {
                if (oldProduct != null)
                    Destroy(oldProduct);
            }

            carriedProducts.Clear();
            carriedProductPrefab = productPrefab;
        }

        CacheCarryPoints();
        var sequence = DOTween.Sequence();
        for (int i = 0; i < carryPoints.Count; i++)
        {
            GameObject product;
            if (i < carriedProducts.Count && carriedProducts[i] != null)
            {
                product = carriedProducts[i];
                product.transform.SetParent(carryPoints[i], false);
            }
            else
            {
                product = Instantiate(productPrefab, carryPoints[i], false);
                if (i < carriedProducts.Count)
                    carriedProducts[i] = product;
                else
                    carriedProducts.Add(product);
            }

            product.transform.position = startPosition;
            product.transform.localRotation = Quaternion.identity;
            product.transform.localScale = productPrefab.transform.localScale;

            var capturedProduct = product;
            if (i == 0)
                capturedProduct.SetActive(true);
            else
            {
                capturedProduct.SetActive(false);
                sequence.AppendCallback(() =>
                {
                    if (capturedProduct != null)
                        capturedProduct.SetActive(true);
                });
            }

            var targetPosition = useFallbackCarryPoints
                ? new Vector3(0f, 0.35f + i * 0.2f, 0f)
                : Vector3.zero;
            sequence.Append(capturedProduct.transform.DOLocalJump(targetPosition, 0.25f, 1, 0.3f));
        }

        sequence.OnComplete(() =>
        {
            carryTween = null;
            onComplete?.Invoke();
        });
        carryTween = sequence;
        UpdateCharacterView();
    }

    protected void ClearCarriedProduct()
    {
        carryTween?.Kill();
        carryTween = null;

        foreach (var product in carriedProducts)
        {
            if (product != null)
                product.SetActive(false);
        }

        UpdateCharacterView();
    }

    private void CacheCarryPoints()
    {
        if (carryPoints.Count > 0)
            return;

        foreach (var child in GetComponentsInChildren<Transform>(true))
        {
            if (!string.Equals(child.name, "head", System.StringComparison.OrdinalIgnoreCase))
                continue;

            carryRoot = child;
            var stackRoot = child.Find("Tomato");
            if (stackRoot != null)
            {
                foreach (Transform point in stackRoot)
                {
                    if (point.name.StartsWith("Point", System.StringComparison.Ordinal))
                        carryPoints.Add(point);
                }
            }

            break;
        }

        if (carryPoints.Count > 0)
            return;

        useFallbackCarryPoints = true;
        carryRoot ??= transform;
        carryPoints.Add(carryRoot);
    }

    protected virtual void Awake()
    {
        agent ??= GetComponent<NavMeshAgent>();
        animator ??= GetComponentInChildren<Animator>();
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        agent.autoRepath = true;

        Entity = new CharacterEntity(definition);
        navigation = new CharacterNavigation(agent);
        characterView = new CharacterView(animator);
        UpdateCharacterView();

        if (definition != null)
            agent.speed = definition.moveSpeed;
    }

    protected virtual void Update()
    {
        if (Entity.CurrentTask == null)
        {
            if (!TryTakeNextTask(out var nextTask))
            {
                SetState(CharacterState.Idle);
                return;
            }

            BeginTask(nextTask);
        }

        TickCurrentTask();
    }

    public void EnqueueTask(CharacterTask task)
    {
        if (task != null)
            tasks.Enqueue(task);
    }

    protected virtual bool TryTakeNextTask(out CharacterTask task)
    {
        if (tasks.Count > 0)
        {
            task = tasks.Dequeue();
            return true;
        }

        task = null;
        return false;
    }

    private void BeginTask(CharacterTask task)
    {
        Entity.CurrentTask = task;
        workTimer = task.Duration;

        if (task.Target != null)
            agent.updateRotation = true;

        if (task.Target != null && navigation.TryMoveTo(task.Target.position))
            SetState(CharacterState.Moving);
        else
            BeginWork();
    }

    private void TickCurrentTask()
    {
        var task = Entity.CurrentTask;
        if (task == null)
            return;

        if (Entity.State == CharacterState.Moving)
        {
            if (!navigation.HasArrived())
                return;

            BeginWork();
        }

        if (workTimer > 0f)
        {
            workTimer -= Time.deltaTime;
            return;
        }

        task.Complete(this);
        Entity.CurrentTask = null;
        SetState(CharacterState.Idle);
    }

    private void BeginWork()
    {
        if (Entity.CurrentTask.Target != null)
        {
            agent.updateRotation = false;
            transform.rotation = Entity.CurrentTask.Target.rotation;
        }

        SetState(Entity.CurrentTask.StateWhileWorking);
    }

    private void SetState(CharacterState state)
    {
        if (Entity.State == state)
            return;

        Entity.State = state;
        UpdateCharacterView();
    }

    private void UpdateCharacterView()
    {
        characterView?.SetState(
            Entity.State,
            carriedProducts.Count > 0 && carriedProducts[0] != null && carriedProducts[0].activeSelf);
    }
}
