using Fusion;
using UnityEngine;
using GetItem.Game;
public class
CollectibleItem
:
NetworkBehaviour

{
    [Networked] public bool IsCollected { get; set; }

    [Networked] public int Value { get; set; } = 10;


    private void OnTriggerEnter(Collider other)

    {

        if (IsCollected) return;


        var player = other.GetComponent<PlayerController>();

        if (player != null && player..HasInputAuthority)

        {

            RPC_CollectItem(player.Object.InputAuthority);

        }

    }

}
