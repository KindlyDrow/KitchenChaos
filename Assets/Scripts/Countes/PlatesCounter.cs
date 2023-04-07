using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlatesCounter : BaseCounter
{

    public event EventHandler OnPlateSpawned;
    public event EventHandler OnPlateRemoved;

    [SerializeField] private KitchenObjectSO plateKitchenObjectSO;

    private float spawnPlateTimer;
    private float maxSpawnPlateTimer = 4f;
    private int currentPlatesAmount;
    private int maxPlatesAmount = 4;

    private void Update()
    {
        if (!IsServer) return;
        spawnPlateTimer += Time.deltaTime;
        if (spawnPlateTimer > maxSpawnPlateTimer && GameManager.Instance.IsGamePlaying())
        {
            if(currentPlatesAmount < maxPlatesAmount )
            {
                SpawnPlateServerRpc();
            }
            
            spawnPlateTimer = 0f;
        }

    }

    [ServerRpc]
    private void SpawnPlateServerRpc()
    {
        SpawnPlateClientRpc();
    }

    [ClientRpc]
    private void SpawnPlateClientRpc()
    {
        currentPlatesAmount++;

        OnPlateSpawned?.Invoke(this, EventArgs.Empty);
    }

    public override void Interact(Player player)
    {
        if (player.KitchenObject == null)
        {
            //Player empty handed
            if(currentPlatesAmount > 0)
            {
                KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, player);

                InteractLogicServerRpc();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc()
    {
        InteractLogicClientRpc();
    }

    [ClientRpc]
    private void InteractLogicClientRpc()
    {
        currentPlatesAmount--;

        OnPlateRemoved?.Invoke(this, EventArgs.Empty);
    }
}
