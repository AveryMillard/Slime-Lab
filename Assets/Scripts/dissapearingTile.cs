using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dissapearingTile : MonoBehaviour
{

    private Collider collider;
    private bool dissapearing = false;
    private Renderer renderer;
    private Color objectColor;

    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<Collider>();
        renderer = GetComponent<Renderer>();
        objectColor = renderer.material.color;
    }

    // Update is called once per frame
    void Update()
    {

        if (dissapearing)
        {
            objectColor.a -= 0.005f;
            renderer.material.color = objectColor;

            if (objectColor.a < 0.000f)
            {
                Destroy(gameObject); // Destroy the object when fully transparent
            }
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Player") {


            dissapearing = true;
        
        
        }
    }
}
