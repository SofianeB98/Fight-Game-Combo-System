using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hadouken : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;

    [Header("Effect")] 
    [SerializeField] private GameObject ps_Effect;
    
    [Header("Impulsion")] 
    [SerializeField] private float forceSpeed = 50.0f;
    [SerializeField] private Vector3 forceDir;

    public void Initialise(Vector3 forceDir)
    {
        this.forceDir = forceDir;
    }
    
    public void RemoveParent()
    {
        transform.parent = null;
        ps_Effect.SetActive(true);
        rb.AddForce(forceDir * forceSpeed, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision other)
    {
        Destroy(gameObject, 0.1f);
    }

    private void OnDestroy()
    {
        
    }
}
