using UnityEngine;

public struct DamageInfo
{
    public float amount;
    public DamageType damageType;
    public GameObject source;

    public DamageInfo(
        float amount,
        DamageType damageType,
        GameObject source)
    {
        this.amount = amount;
        this.damageType = damageType;
        this.source = source;
    }
}