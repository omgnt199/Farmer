using UnityEngine;
public class GameFX : MonoBehaviour
{
    public ParticleSystem Fx_Upgrade_Tier;
    public ParticleSystem Fx_Unlock_Construction;
    public ParticleSystem Fx_Coin;
    public void UpgradeTierFx(Vector3 spawnPos)
    {
        Instantiate(Fx_Upgrade_Tier, spawnPos, Quaternion.identity);
    }
    public void UnlockConstructionFx(Vector3 spawnPos)
    {
        Instantiate(Fx_Unlock_Construction, spawnPos, Quaternion.identity);
    }
    public void CoinEffect(Vector3 spawnPos)
    {
        Instantiate(Fx_Coin, spawnPos, Quaternion.identity);
    }
}