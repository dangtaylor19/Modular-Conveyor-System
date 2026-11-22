using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Product : MonoBehaviour
{
    public Rigidbody rb { get; private set; }

    private Dictionary<ConveyorBelt, List<ContactPoint>> belts = new Dictionary<ConveyorBelt, List<ContactPoint>>();

    /* Maximum amount of contact points for this object */
    [SerializeField] private int contactPointsCount;

    private void Start() {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate() {
        if (belts.Count == 0) return;
        
        /* Loop through each belt and apply the relevant force for each contact point */
        foreach (var belt in belts) {
            int size = Mathf.Min(contactPointsCount, belt.Value.Count);
            for (int i = 0; i < size && belt.Value.Count != 0; i++) {
                //Take the current velocity in the direction of the conveyor belt
                float currentSpeed = Vector3.Dot(rb.linearVelocity, belt.Key.direction);

                float velocityDelta = Mathf.Max(0, belt.Key.conveyorSpeed - currentSpeed);

                Vector3 force = belt.Key.direction * velocityDelta * rb.mass / size;

                rb.AddForceAtPosition(force, belt.Value[i].point, ForceMode.Impulse);
                
                Debug.DrawRay(belt.Value[i].point, belt.Key.direction * velocityDelta, Color.red);
            }
        }
    }

    /* Assigns the contact points of each belt this object is touching */
    private void OnCollisionStay(Collision other) {
        if (other.transform.parent == null) return;
        if(other.transform.parent.TryGetComponent(out ConveyorBelt belt)){
            belts[belt] = other.contacts.ToList();
        }
    }

    /* Clears the contact points when exiting a belt */
    private void OnCollisionExit(Collision other) {
        if (other.transform.parent == null) return;
        if (other.transform.parent.TryGetComponent(out ConveyorBelt belt)) {
            belts.Remove(belt);
        }
    }

    /* Debug function for checking center of mass */
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(GetComponent<Rigidbody>().worldCenterOfMass, 0.05f);
    }
}
