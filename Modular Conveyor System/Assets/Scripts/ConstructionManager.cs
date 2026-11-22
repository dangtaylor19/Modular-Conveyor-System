using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ConstructionManager : MonoBehaviour {
    private PlacementParticles placementParticles;

    public LayerMask groundMask;
    public LayerMask beltMask;

    public GameObject[] beltPrefabs;
    public GameObject[] beltPlaceholderPrefabs;

    private BoxCollider placeholderBox;

    public GameObject cancelButton;

    private int selectedBeltIndex = -1;

    private float rotation = 0;

    private void Start() {
        placementParticles = FindAnyObjectByType<PlacementParticles>();
    }

    void Update() {

        RaycastHit hit;

        /* Finds the position of conveyor belt placement. Moves a placeholder conveyor to the mouse, and constructs it on click */
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, groundMask) && placeholderBox) {
            Vector3 beltPos = new Vector3(0, 1000, 0);

            /* Snaps conveyor to currently hovered over snapping point */
            if (hit.transform.tag == "SnapPoint" && !Input.GetKey(KeyCode.LeftShift)) {
                Vector3 direction = (hit.transform.position - hit.transform.parent.position);
                direction.y = 0;
                direction.Normalize();

                float distance;
                 
                /* Handles rotation of currently held conveyor */
                if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z)) {
                    distance = placeholderBox.bounds.extents.x;
                } else {
                    distance = placeholderBox.bounds.extents.z;
                }

                Vector3 snappingPos = hit.transform.position;
                snappingPos.y = hit.transform.parent.position.y;

                beltPos = snappingPos + direction * distance;
            } else {
                beltPos = hit.point;
            }

            placeholderBox.transform.position = beltPos;

            /* Builds a new conveyor on click input */
            if (!IsOverlapping()) {
                placeholderBox.transform.Find("Cube").gameObject.SetActive(false);
                if (selectedBeltIndex != -1 && Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject()) {
                    GameObject newBelt = Instantiate(beltPrefabs[selectedBeltIndex], beltPos, beltPrefabs[selectedBeltIndex].transform.rotation);
                    newBelt.transform.Rotate(new Vector3(0, rotation, 0));
                    newBelt.GetComponentInChildren<ConveyorBelt>().direction = Quaternion.Euler(0, rotation, 0) * newBelt.GetComponentInChildren<ConveyorBelt>().direction;

                    placementParticles.TriggerParticles(beltPos);

                    CancelBuild();
                }
            }else{
                placeholderBox.transform.Find("Cube").gameObject.SetActive(true);
            }
        }

        /* Rotates currently held conveyor by 90 degrees */
        if (Input.GetKeyDown(KeyCode.R)) {
            rotation = (rotation + 90) % 360;

            if (placeholderBox) placeholderBox.transform.eulerAngles = new Vector3(0, rotation, 0);
        }
    }

    /* Returns true if placeholder conveyor is overlapping with any objects */
    private bool IsOverlapping() {
        if (!placeholderBox) return true;

        Vector3 center = placeholderBox.transform.TransformPoint(placeholderBox.center);

        return Physics.OverlapBox(center, placeholderBox.size * 0.475f, placeholderBox.transform.rotation, beltMask, QueryTriggerInteraction.Ignore).Length > 0;
    }

    /* Initializes the construction system and assigns a belt based on index */
    public void SelectBelt(int index) {
        if (placeholderBox) {
            Destroy(placeholderBox.gameObject);
            placeholderBox = null;
        }

        if (index >= beltPrefabs.Length) return;

        selectedBeltIndex = index;

        placeholderBox = Instantiate(beltPlaceholderPrefabs[index], new Vector3(0, 1000, 0), beltPlaceholderPrefabs[index].transform.rotation).GetComponent<BoxCollider>();
        placeholderBox.transform.Rotate(new Vector3(0, rotation, 0));

        cancelButton.SetActive(true);
    }

    /* Cancel the build and destroy the current palceholder belt */
    public void CancelBuild() {
        selectedBeltIndex = -1;
        if (placeholderBox) {
            Destroy(placeholderBox.gameObject);
            placeholderBox = null;
        }

        cancelButton.SetActive(false);
    }
}
