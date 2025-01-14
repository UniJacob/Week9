using Fusion;
using UnityEngine;

public class Player : NetworkBehaviour
{
    private CharacterController _cc;
    [Networked] public bool Shielded { get; set; }

    [SerializeField] float speed = 5f;
    [SerializeField] GameObject ballPrefab;
    [SerializeField] GameObject Body;
    [SerializeField] float minY = 0.6f;

    private Camera firstPersonCamera;
    public override void Spawned()
    {
        _cc = GetComponent<CharacterController>();
        if (HasStateAuthority)
        {
            firstPersonCamera = Camera.main;
            var firstPersonCameraComponent = firstPersonCamera.GetComponent<FirstPersonCamera>();
            if (firstPersonCameraComponent && firstPersonCameraComponent.isActiveAndEnabled)
                firstPersonCameraComponent.SetTarget(this.transform);
        }
        Shielded = false;
    }

    private Vector3 moveDirection;
    public override void FixedUpdateNetwork()
    {
        Vector3 groundlvl = new(transform.position.x, minY, transform.position.z);
        transform.position = groundlvl;
        if (GetInput(out NetworkInputData inputData))
        {
            if (inputData.moveActionValue.magnitude > 0)
            {
                inputData.moveActionValue.Normalize();   //  Ensure that the vector magnitude is 1, to prevent cheating.
                moveDirection = new Vector3(inputData.moveActionValue.x, 0, inputData.moveActionValue.y);
                Body.transform.rotation = Quaternion.LookRotation(moveDirection);
                Vector3 DeltaX = speed * moveDirection * Runner.DeltaTime;
                //Debug.Log($"{speed} * {moveDirection} * {Runner.DeltaTime} = {DeltaX}");
                _cc.Move(DeltaX);
            }

            if (HasStateAuthority)
            { // Only the server can spawn new objects ; otherwise you will get an exception "ClientCantSpawn".
                if (inputData.shootActionValue)
                {
                    Runner.Spawn(ballPrefab,
                        transform.position + moveDirection, Quaternion.LookRotation(Body.transform.forward),
                        Object.InputAuthority);
                }
            }
        }
    }
}
