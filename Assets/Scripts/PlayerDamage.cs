using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    public int maxHealth = 100;
    private int health;

    public GameObject dieVfx;

    private Rigidbody rb;
    private SmoothDroneController controller;

    void Start()
    {
        health = maxHealth;

        rb = GetComponent<Rigidbody>();
        controller = GetComponent<SmoothDroneController>();

        rb.useGravity = false;
        rb.isKinematic = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyBullet"))
        {
            TakeDamage(10);
            Destroy(other.gameObject);
        }
    }

    void TakeDamage(int dmg)
    {
        health -= dmg;
        Debug.Log("PLAYER HP = " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("PLAYER DEAD!");

        // Disable drone control
        controller.enabled = false;

        // Stop propellers visually
        controller.engineOn = false;

        // Enable physics
        rb.isKinematic = false;
        rb.useGravity = true;

        // Add fall force + random spin
        rb.AddForce(Vector3.down * 10f, ForceMode.Impulse);
        rb.AddTorque(
            new Vector3(
                Random.Range(-5, 5),
                Random.Range(-5, 5),
                Random.Range(-5, 5)
            ),
            ForceMode.Impulse
        );

        // Crash VFX
        if (dieVfx != null)
            Instantiate(dieVfx, transform.position, Quaternion.identity);

        // OPTIONAL: destroy after 5 sec
        Destroy(gameObject, 5f);
    }
}
