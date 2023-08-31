using System.Collections;
using UnityEngine;

public class FallTrigger : MonoBehaviour {
    [SerializeField] private Transform spawnPoint, fallPoint;
    [SerializeField] private Collider2D fallCollider;
    public float distanceForDespawn = 10;
    private Coroutine coroutine;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player")) 
        {
            coroutine ??= StartCoroutine(CheckFall(other.transform));
        }
    }

    private IEnumerator CheckFall(Transform player)
    {
        while (true)
        {
            yield return new WaitForSeconds(1);

            if (player.position.y < fallPoint.position.y - distanceForDespawn)
            {
                player.GetComponent<PlayerMovement>().Respawn(spawnPoint.position);
                break;
            } else if (player.position.y > fallPoint.position.y + 1)
            {
                Debug.Log("Free");
                break;
            }
        }

        coroutine = null;
    }
}