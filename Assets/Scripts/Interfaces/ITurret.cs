using UnityEngine;

public interface ITurret
{
    TurretAttribute TurretAttribute { get; set; }

    public abstract void Shoot();
}
