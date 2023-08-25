using UnityEngine;

public class PlayerInfo : MonoBehaviour {
    public static Transform Instance;
    

    private void Awake() {
        if (Instance == null) {
            Instance = gameObject.transform;
        } else {
            Destroy(gameObject);
        }
    }
}