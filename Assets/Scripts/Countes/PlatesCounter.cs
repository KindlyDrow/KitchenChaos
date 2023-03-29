using System;
using System.Collections;
using System.Collections.Generic;
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
        spawnPlateTimer += Time.deltaTime;
        if (spawnPlateTimer > maxSpawnPlateTimer && GameManager.Instance.IsGamePlaying())
        {
            if(currentPlatesAmount < maxPlatesAmount )
            {
                currentPlatesAmount++;

                OnPlateSpawned?.Invoke(this, EventArgs.Empty);
            }
            
            spawnPlateTimer = 0f;
        }

    }

    public override void Interact(Player player)
    {
        if (player.KitchenObject == null)
        {
            //Player empty handed
            if(currentPlatesAmount > 0)
            {
                currentPlatesAmount--;

                KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, player);

                OnPlateRemoved?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
