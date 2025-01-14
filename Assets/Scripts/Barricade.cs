using Fusion;
using UnityEngine;

public class Barricade : NetworkBehaviour
{
    [Networked] private TickTimer Timer { get; set; }

    [SerializeField][Range(1, 20)] float upTime;
    [SerializeField][Range(1, 20)] float downTime;
    [SerializeField] float maxY = 1;
    [SerializeField] float moveSpeed = 10;

    float minY;
    [Networked] bool up { get; set; }

    public override void Spawned()
    {
        up = true;
        minY = maxY - GetComponent<Collider>().bounds.size.y;
        var tmp = transform.position;
        tmp.y = minY;
        transform.position = tmp;
        Timer = TickTimer.CreateFromSeconds(Runner, downTime * Random.Range(0, 1f));
    }

    public override void FixedUpdateNetwork()
    {
        if (!Timer.Expired(Runner))
        {
            return;
        }
        if (up)
        {
            goUp();
        }
        else
        {
            goDown();
        }
    }

    private void goUp()
    {
        var dest = transform.position;
        dest.y = maxY;
        transform.position = Vector3.MoveTowards(transform.position, dest, moveSpeed * Runner.DeltaTime);
        if (transform.position.y >= maxY)
        {
            up = false;
            //waitTimer = upTime;
            Timer = TickTimer.CreateFromSeconds(Runner, upTime);
        }
    }

    private void goDown()
    {
        var dest = transform.position;
        dest.y = minY;
        transform.position = Vector3.MoveTowards(transform.position, dest, moveSpeed * Runner.DeltaTime);
        if (transform.position.y <= minY)
        {
            up = true;
            //waitTimer = downTime;
            Timer = TickTimer.CreateFromSeconds(Runner, downTime);
        }
    }
}
