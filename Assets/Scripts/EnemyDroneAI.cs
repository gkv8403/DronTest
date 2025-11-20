using UnityEngine;

public class EnemyDroneAI : MonoBehaviour
{
    [Header("Assign Player")]
    public Transform player;

    [Header("Roaming Points")]
    public Transform[] roamPoints;

    [Header("Propellers")]
    public Transform[] propellers;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float chaseSpeed = 7f;
    public float rotateSpeed = 4f;
    public float hoverHeight = 2f;
    public float bobAmount = 0.2f;
    public float bobSpeed = 2f;
    public float tiltAmount = 10f;

    [Header("Detection")]
    public float detectRadius = 15f;
    public float shootRadius = 10f;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float fireRate = 0.15f;
    private float nextFireTime;

    [Header("Prop Animation")]
    public float propellerRPM = 1800f;

    private int roamIndex = 0;

    private enum AIState { Roam, Chase, Attack }
    private AIState state = AIState.Roam;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = false;

        roamIndex = Random.Range(0, roamPoints.Length);
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        // State switching
        if (dist > detectRadius) state = AIState.Roam;
        else if (dist > shootRadius) state = AIState.Chase;
        else state = AIState.Attack;

        switch (state)
        {
            case AIState.Roam: DoRoam(); break;
            case AIState.Chase: DoChase(); break;
            case AIState.Attack: DoAttack(); break;
        }

        AnimatePropellers();
        HoverBob();
    }

    // -----------------------------
    //      ROAM AROUND MAP
    // -----------------------------
    void DoRoam()
    {
        Transform target = roamPoints[roamIndex];

        MoveTowards(target.position, moveSpeed);

        if (Vector3.Distance(transform.position, target.position) < 2f)
        {
            roamIndex = Random.Range(0, roamPoints.Length);
        }
    }

    // -----------------------------
    //      CHASE PLAYER
    // -----------------------------
    void DoChase()
    {
        MoveTowards(player.position, chaseSpeed);
    }

    // -----------------------------
    //     ATTACK PLAYER
    // -----------------------------
    void DoAttack()
    {
        LookTowards(player.position);

        if (Time.time > nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Shoot();
        }
    }

    // -----------------------------
    //    COMMON MOVEMENT ACTIONS
    // -----------------------------
    void MoveTowards(Vector3 target, float speed)
    {
        LookTowards(target);

        Vector3 dir = (target - transform.position).normalized;

        Vector3 move = dir * speed * Time.deltaTime;
        move.y = 0; // keep level flight

        transform.position += move;
    }

    void LookTowards(Vector3 target)
    {
        Vector3 dir = (target - transform.position);
        dir.y = 0;

        if (dir == Vector3.zero) return;

        Quaternion desiredRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRot, rotateSpeed * Time.deltaTime);

        // tilt animation
        float tiltX = Mathf.Clamp(-dir.z * tiltAmount, -tiltAmount, tiltAmount);
        float tiltZ = Mathf.Clamp(dir.x * tiltAmount, -tiltAmount, tiltAmount);

        transform.localRotation = Quaternion.Euler(tiltX, transform.localEulerAngles.y, tiltZ);
    }


    // -----------------------------
    //     SHOOTING
    // -----------------------------
    void Shoot()
    {
        Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
    }

    // -----------------------------
    //  PROPELLER SPIN ANIMATION
    // -----------------------------
    void AnimatePropellers()
    {
        float rot = propellerRPM * Time.deltaTime;

        foreach (Transform p in propellers)
        {
            if (p)
                p.Rotate(0, 0, rot, Space.Self);
        }
    }

    // -----------------------------
    //    HOVERING / BOB EFFECT
    // -----------------------------
    void HoverBob()
    {
        Vector3 pos = transform.position;
        pos.y = roamPoints[0].position.y + hoverHeight +
                Mathf.Sin(Time.time * bobSpeed) * bobAmount;

        transform.position = new Vector3(transform.position.x, pos.y, transform.position.z);
    }

    // Debug view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, shootRadius);
    }
}
