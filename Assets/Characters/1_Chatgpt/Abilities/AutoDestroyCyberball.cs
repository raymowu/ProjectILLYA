using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class AutoDestroyCyberball : NetworkBehaviour
{
    public float delayBeforeDestroy = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyCyberball());
    }

    IEnumerator DestroyCyberball()
    {
        yield return new WaitForSeconds(delayBeforeDestroy);
        DestroyCyberballServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyCyberballServerRpc()
    {
        GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject);
    }



}
