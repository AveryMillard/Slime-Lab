using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrapple : MonoBehaviour
{
    public GameObject grappleAnchorPrefab;
    public GameObject grapplePrefab;
    private GameObject grappleCylinder;

    private bool isGrappling = false;
    private Vector3 grapplePoint;
    public float maxGrappleLength = 5f;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("FireGrapple") && !isGrappling)
        {
            StartGrapple();
            Debug.Log("Grapple Clicked");
        }

        if (Input.GetButtonUp("FireGrapple") && isGrappling)
        {
            StopGrapple();
            Debug.Log("Grapple Released");
        }
        if (isGrappling)
        {
            ConstrainMovement();
            UpdateGrappleCylinder();
        }

    }

    void StartGrapple()
    {

        //Firing the raycast to determine grapple point
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        //Run only if RayCast hits a surface
        //(Can be further configed to check for a specific surface type
        if(Physics.Raycast(ray, out hit))
        {
            //Creates a point for the grapple to connect to
            grapplePoint = hit.point;

            float distanceToGrapple = Vector3.Distance(transform.position, grapplePoint);

            if (distanceToGrapple > maxGrappleLength) return;

            grappleCylinder = Instantiate(grapplePrefab, transform.position, Quaternion.identity);



            //Helper parameter
            isGrappling = true;
        }


    }

    void StopGrapple()
    {

        if (grappleCylinder != null)
        {
            Destroy(grappleCylinder);
        }

        isGrappling = false;
    }

    void ConstrainMovement()
    {
        float distanceToGrapple = Vector3.Distance(transform.position, grapplePoint);

        if (distanceToGrapple > maxGrappleLength)
        {
            Vector3 directionToGrapple = (transform.position - grapplePoint).normalized;
            transform.position = grapplePoint + directionToGrapple * maxGrappleLength;
        }
    }

    void UpdateGrappleCylinder()
    {
        if (grappleCylinder != null)
        {
            // Calculate the direction and distance between the player and the grapple point
            Vector3 direction = grapplePoint - transform.position;
            float distance = Vector3.Distance(transform.position, grapplePoint);

            // Limit the stretch distance
            distance = Mathf.Min(distance, maxGrappleLength);

            // Position the cylinder halfway between the player and the grapple point
            grappleCylinder.transform.position = transform.position + direction.normalized * (distance / 2);

            // Rotate the cylinder to align with the direction to the grapple point
            grappleCylinder.transform.rotation = Quaternion.LookRotation(direction);

            // Scale the cylinder to match the distance
            grappleCylinder.transform.localScale = new Vector3(
                grappleCylinder.transform.localScale.x,
                grappleCylinder.transform.localScale.y,
                distance
            );
        }

    }
}
