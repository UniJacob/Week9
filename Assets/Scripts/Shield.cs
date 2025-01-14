using Fusion;
using UnityEngine;

public class Shield : NetworkBehaviour
{
    [Networked] private TickTimer lifeTimer { get; set; }
    [SerializeField] float lifeTime = 3;
    Player ShieldedPlayer = null;

    public override void Spawned()
    {
        lifeTimer = TickTimer.CreateFromSeconds(Runner, lifeTime);
    }

    public override void FixedUpdateNetwork()
    {
        if (lifeTimer.Expired(Runner))
        {
            EndShield();
            return;
        }
        ShieldedPlayer.Shielded = true;
        transform.position = ShieldedPlayer.transform.position;
    }

    public void StartShield(Player toFollow)
    {
        //gameObject.SetActive(true);
        ShieldedPlayer = toFollow;
        transform.position = toFollow.transform.position;
        transform.parent = toFollow.gameObject.transform;
    }

    public void EndShield()
    {
        //ToFollow = null;
        //gameObject.SetActive(false);
        ShieldedPlayer.Shielded = false;
        Runner.Despawn(Object);
    }
}
