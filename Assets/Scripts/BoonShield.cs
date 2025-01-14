using Fusion;
using Unity.VisualScripting;
using UnityEngine;

public class BoonShield : NetworkBehaviour
{
    [Networked] private TickTimer lifeTimer { get; set; }
    [SerializeField] NetworkObject ShieldPrefab;
    [SerializeField] float lifeTime = 3;
    bool activated = false;

    public override void Spawned()
    {
        lifeTimer = TickTimer.CreateFromSeconds(Runner, lifeTime);
    }

    public override void FixedUpdateNetwork()
    {
        if (lifeTimer.Expired(Runner))
            Runner.Despawn(Object);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (HasStateAuthority) // Only the server can spawn new objects ; otherwise you will get an exception "ClientCantSpawn".
        {
            if (activated)
            {
                return;
            }
            var p = other.GetComponent<Player>();
            if (p.IsUnityNull())
            {
                return;
            }
            activated = true;
            var tmp = Runner.Spawn(ShieldPrefab, Vector3.zero, Quaternion.identity, Object.InputAuthority);
            tmp.GetComponent<Shield>().StartShield(p);
            Runner.Despawn(Object);
        }
    }
}
