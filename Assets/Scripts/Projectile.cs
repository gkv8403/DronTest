using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 40f;
    public float lifeTime = 10f;
    public GameObject hitVFX;   // assign HitVFX prefab

    void Start()
    {
        // Destroy automatically after lifetime if no collision
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Prevent hitting itself
        if (other.gameObject == this.gameObject) return;

        // Spawn hit VFX
        if (hitVFX != null)
        {
            Instantiate(hitVFX, transform.position, Quaternion.identity);
        }

        // Destroy projectile on impact
        Destroy(gameObject);
    }
}
