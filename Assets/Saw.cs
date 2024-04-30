using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saw : MonoBehaviour
{
    public float rotationSpeed;
    public GameObject sawBlade;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate() {
        sawBlade.transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

    }
}
