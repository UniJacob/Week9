using Fusion;
using UnityEngine;

/**
 * This component represents a ball moving at a constant speed.
 */
public class Bullet : NetworkBehaviour
{
    [Networked] private TickTimer lifeTimer { get; set; }

    [SerializeField] float lifeTime = 5.0f;
    [SerializeField] float speed = 5.0f;
    [SerializeField] int damagePerHit = 1;

    bool destroy = false;

    public override void Spawned()
    {
        lifeTimer = TickTimer.CreateFromSeconds(Runner, lifeTime);
    }

    public override void FixedUpdateNetwork()
    {
        //Debug.Log(lifeTimer);
        if (lifeTimer.Expired(Runner) || destroy)
            Runner.Despawn(Object);
        else
        {
            //Vector3 direction = transform.rotation * Vector3.forward;
            //direction.y = 0f;
            //direction = direction.normalized;
            transform.position += speed * transform.forward * Runner.DeltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter " + other.gameObject.name + " " + other.gameObject.tag);
        Health health = other.GetComponent<Health>();
        if (health != null)
        {
            if (other.GetComponent<Player>()?.Shielded == false)
            {
                health.DealDamageRpc(damagePerHit);
            }
        }
        //if (!Object.IsUnityNull())
        //{
        //    Runner.Despawn(Object);
        //}
        destroy = true;
    }
}