using Fusion;
using UnityEngine;
using static Unity.Collections.Unicode;

//namespace GetItem.Game
//{ // This namespace is used to organize the code related to the item spawning functionality

    public class ItemSpawner : NetworkBehaviour
    {
        public GameObject itemPrefab;
        public float spawnInterval = 3f;
        private float timer;

        public void SpawnItemes()
        {
            if (!Object.HasStateAuthority) return;
            if (itemPrefab == null) Debug.LogError("itemPrefab is null!");
            if (Runner == null) Debug.LogError("Runner is null!");
            Vector3 pos = new Vector3(Random.Range(-5f, 5f), 1.5f, Random.Range(-5f, 5f));
            Runner.Spawn(itemPrefab, pos, Quaternion.identity);
        }




        /*             
        void Update()
        {
            if (!Object.HasStateAuthority) return;

            timer += Time.deltaTime;
            if (timer >= spawnInterval)
            {
                timer = 0f;
                Vector3 pos = new Vector3(Random.Range(-5f, 5f), 1.5f, Random.Range(-5f, 5f));
                Runner.Spawn(itemPrefab, pos, Quaternion.identity);
            }
        }
        */
    }
//}

