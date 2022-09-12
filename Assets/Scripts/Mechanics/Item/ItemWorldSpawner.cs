using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWorldSpawner : MonoBehaviour {

    #region Singleton

    public static ItemWorldSpawner Instance { get; private set; }

    void Awake() {

        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

    }

    #endregion

    [SerializeField] private GameObject itemWorldPrefab;
    [SerializeField] private Transform worldItemsContainer;

    public void SpawnItemWorld(Item item, Vector2 location) {
        Vector3 randomDir = Utils.GetRandomDirection();

        ItemWorld itemWorldSpawned = Instantiate(itemWorldPrefab, location + (Vector2)randomDir, Quaternion.identity, worldItemsContainer).GetComponent<ItemWorld>();
        itemWorldSpawned.UpdateItemWorld(item);


        itemWorldSpawned.GetComponent<Rigidbody2D>().AddForce(randomDir, ForceMode2D.Impulse);
    }
}
