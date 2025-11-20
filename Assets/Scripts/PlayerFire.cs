using UnityEngine;

public class PlayerFire : MonoBehaviour
{
    public GameObject missilePrefab;
    public Transform firePoint;
    public GameObject crosshairWorld;       // 3D or world-space UI
    public float maxShootDistance = 200f;

    void Update()
    {
        HandleCrosshair();

        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void HandleCrosshair()
    {
        Ray ray = new Ray(firePoint.position, firePoint.forward);

        // Debug ray
        Debug.DrawRay(firePoint.position, firePoint.forward * maxShootDistance, Color.red);

        if (Physics.Raycast(ray, out RaycastHit hit, maxShootDistance))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                crosshairWorld.SetActive(true);

                // Move crosshair to the exact hit position
                crosshairWorld.transform.position = hit.point;

                // Make sure it faces towards the player or camera (optional)
                crosshairWorld.transform.LookAt(firePoint);
            }
            else
            {
                crosshairWorld.SetActive(false);
            }
        }
        else
        {
            crosshairWorld.SetActive(false);
        }
    }

    void Shoot()
    {
        Instantiate(missilePrefab, firePoint.position, firePoint.rotation);
    }
}
