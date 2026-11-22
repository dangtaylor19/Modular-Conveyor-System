using UnityEngine;

public class ItemHopper : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        if(other.TryGetComponent(out Product product)){
            Destroy(product.gameObject);
        }
    }
}
