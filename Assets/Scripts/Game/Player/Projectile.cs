using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Bolt.EntityEventListener<IProjectileState>
{
    float remaining = 0;

    [SerializeField]
    float time = 3;

    [SerializeField]
    float speed = 20f;

    public override void Attached()
    {
        state.SetTransforms(state.transform, transform);
        remaining = time;
    }

    public override void SimulateOwner()
    {
        if ((remaining -= Time.deltaTime) <= 0)
        {
            BoltNetwork.Detach(entity);
            Destroy(gameObject);
        }
        else
        {
            transform.position = transform.position + transform.right * speed * BoltNetwork.FrameDeltaTime;
        }
    }
}