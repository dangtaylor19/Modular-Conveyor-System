using System.Collections;
using UnityEngine;

public class ItemGiver : MonoBehaviour
{
    public GameObject boxPrefab;
    public GameObject canPrefab;

    private void Start() {
        StartCoroutine(SpawnItem());
    }

    private IEnumerator SpawnItem(){

        yield return new WaitForSeconds(2.0f);

        Instantiate(Random.Range(0, 2) == 0 ? boxPrefab : canPrefab, transform.position, transform.rotation);

        StartCoroutine(SpawnItem());
    }
}
