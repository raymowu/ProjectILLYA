using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MoveRockHurl : NetworkBehaviour
{
    [SerializeField] private float shootForce;
    private Rigidbody rb;
    public DarthogAbilities parent;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) { return; }
        // Move projectile forward based on the player facing direction
        rb.velocity = rb.transform.forward * shootForce;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsOwner) { return;  }
        GameManager.Instance.Stun(other.gameObject, parent.ROCK_HURL_STUN_DURATION);
        DestroyAbility1ServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyAbility1ServerRpc()
    {
        GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject);
    }
}
