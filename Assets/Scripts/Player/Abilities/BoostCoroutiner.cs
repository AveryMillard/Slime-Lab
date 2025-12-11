using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostCoroutiner : MonoBehaviour
{
    // Start is called before the first frame update
    float boostForce = 350f; // Initial boost force
    float decayRate = 3f;   // How quickly the boost decays
    float boostDuration = 2f; // Duration of the boost
    float maxSpeed = 35f;

    GameObject playerObject;
    Rigidbody rb;
    private bool isBoostActive = true;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(BoostCoroutine());
    }

    // Update is called once per frame
    private IEnumerator BoostCoroutine()
    {
        float timeElapsed = 0f;

        while (timeElapsed < boostDuration && isBoostActive)
        {
            float currentForce = boostForce * Mathf.Exp(-decayRate * timeElapsed);

            // Limit maximum speed
            if (rb.velocity.magnitude < maxSpeed)
            {
                rb.AddForce(rb.transform.forward * currentForce, ForceMode.Force); // Boost forward
            }

            timeElapsed += Time.deltaTime;
            yield return null; 
        }
        Destroy(this);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("collision");
        // Check if the collided object has the "Obstacle" tag
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Bomb"))
        {
            // Stop the boost effect
            StartCoroutine(DelayAction(0.01f));
            
        }
    }

    IEnumerator DelayAction(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        isBoostActive = false;
    }

}
