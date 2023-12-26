using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(NetworkObject))]
public class AutoDestroyPortals : NetworkBehaviour
{
    public float delayBeforeDestroy;
    public RickAbilities parent;

    void Start()
    {
        StartCoroutine(DestroyPortals());
    }

    IEnumerator DestroyPortals()
    {
        yield return new WaitForSeconds(delayBeforeDestroy);
        DestroyPortalsServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyPortalsServerRpc()
    {
        DestroyPortalsClientRpc();
        parent.entrancePortal.gameObject.GetComponent<NetworkObject>().Despawn();
        Destroy(parent.entrancePortal.gameObject);
        GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject);
    }

    // not all clients have access to parent so need to do this way :(
    [ClientRpc]
    private void DestroyPortalsClientRpc()
    {
        foreach (GameObject player in GameManager.Instance.playerPrefabs)
        {
            if (player.GetComponent("RickAbilities") as RickAbilities != null)
            {
                player.GetComponent<RickAbilities>().entrancePortalExists = false;
                player.GetComponent<RickAbilities>().exitPortalExists = false;
            }
        }
    }
}
