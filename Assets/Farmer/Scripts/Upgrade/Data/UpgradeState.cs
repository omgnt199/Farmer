using System;
[Serializable]
public class UpgradeState
{
    public string upgradeId;
    public int level;

    public UpgradeState() { }

    public UpgradeState(string upgradeId)
    {
        this.upgradeId = upgradeId;
    }
}
