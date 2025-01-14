using Fusion;
using UnityEngine;

public class Barricade : NetworkBehaviour
{
    [Networked] private bool up { get; set; }
    [Networked] private TickTimer Timer { get; set; }
    [SerializeField][Range(1, 20)] private float upTime;
    [SerializeField][Range(1, 20)] private float downTime;
    [SerializeField] private float maxY = 1;
    [SerializeField] private float moveSpeed = 10;
    private float minY;

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