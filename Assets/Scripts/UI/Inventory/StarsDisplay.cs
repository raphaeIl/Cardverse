using UnityEngine;
using UnityEngine.UI;

public class StarsDisplay : MonoBehaviour {

    [SerializeField] private GameObject starPrefab;

    public void UpdateStarsDisplay(int amount) {
        Clear();

        for (int i = 0; i < amount; i++)
            Instantiate(starPrefab, transform);

    }

    private void Clear() {
        foreach (Transform transform in transform)
            Destroy(transform.gameObject);
    }

}
