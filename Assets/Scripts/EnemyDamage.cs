using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public int maxHealth = 100;
    private int health;
    public GameObject dieVfx;
    void Start()
    {
        health = maxHealth;
    }

    private void OnTriggerEnter(Collider other)
    {
        print(11);
        if (other.CompareTag("PlayerMissile"))
        {
            print(121);
            TakeDamage(30);   // missile damage
            Destroy(other.gameObject);
        }
    }

    public void TakeDamage(int dmg)
    {
        health -= dmg;

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Spawn hit VFX
        if (dieVfx != null)
        {
            Instantiate(dieVfx, transform.position, Quaternion.identity);
        }
        // add explosion here later
        Destroy(gameObject);
    }
}
