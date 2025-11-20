using UnityEngine;

public class TutorialRing : MonoBehaviour
{
    [Header("Visual Settings")]
    public float rotationSpeed = 50f;
    public Color ringColor = Color.green;
    public float pulseSpeed = 2f;
    public float pulseAmount = 0.2f;

    [Header("Effects")]
    public GameObject collectEffect;
   
    private Renderer ringRenderer;
    private Vector3 originalScale;
   

    void Start()
    {
        ringRenderer = GetComponent<Renderer>();
        originalScale = transform.localScale;

        // Set ring color
        if (ringRenderer != null && ringRenderer.material.HasProperty("_Color"))
        {
            ringRenderer.material.color = ringColor;
        }

        
    }

    void Update()
    {
        // Rotate the ring
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // Pulse effect
        float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        transform.localScale = originalScale * pulse;
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if player passed through
        if (other.CompareTag("Player"))
        {
            CollectRing();
        }
    }

    void CollectRing()
    {
        // Spawn collection effect
        if (collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, Quaternion.identity);
        }

        
        // The tutorial manager will destroy this ring
        // So we just disable visuals here
        if (ringRenderer != null)
            ringRenderer.enabled = false;

        GetComponent<Collider>().enabled = false;
    }
}