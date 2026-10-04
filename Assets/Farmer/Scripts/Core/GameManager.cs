using Farmer;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameFX gameFX;

    public Camera MainCamera => mainCamera;
    public GameFX GameFX => gameFX;
    public static bool SkipConstructionHarvestSpeed { get; private set; }

    [SerializeField] private bool freeSpend;
    [Tooltip("Skip the construction harvest interval; after collection, fruit respawns in 0.5–1 seconds.")]
    [SerializeField] private bool skipConstructionHarvestSpeed;

    private static readonly BigNumber CheatAmount = new(10_000_000);

    protected override void Awake()
    {
        base.Awake();
        if (Instance == this)
        {
            EconomyManager.FreeSpend = freeSpend;
            SkipConstructionHarvestSpeed = skipConstructionHarvestSpeed;
        }
    }

    void Start()
    {
        Application.targetFrameRate = 60;
    }

    void OnValidate()
    {
        if (Application.isPlaying && Instance == this)
        {
            EconomyManager.FreeSpend = freeSpend;
            SkipConstructionHarvestSpeed = skipConstructionHarvestSpeed;
        }
    }

    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.F1))
            return;

        EconomyManager.AddMoney(CheatAmount, CurrencyType.Coin);
        EconomyManager.AddMoney(CheatAmount, CurrencyType.Diamond);
    }
}
