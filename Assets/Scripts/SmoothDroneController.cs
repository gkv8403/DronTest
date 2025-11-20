using UnityEngine;

[DisallowMultipleComponent]
public class SmoothDroneController : MonoBehaviour
{
    [Header("Hierarchy (assign)")]
    public Transform visualRoot;
    public Transform[] propellers;

    [Header("Engine / Takeoff")]
    public bool engineOn = false;
    public float hoverHeight = 1.6f;
    public float takeoffSpeed = 1.2f;
    public float landingSpeed = 1.5f;
    public float liftRPMThreshold = 400f;
    public float propSpinUpRate = 600f;
    public float propSpinDownRate = 1000f;

    [Header("Prop RPM")]
    public float minPropRPM = 0f;
    public float maxPropRPM = 2000f;

    [Header("Movement (arcade)")]
    public float moveSpeed = 5f;
    public float backSpeedMultiplier = 0.7f;
    public float verticalSpeed = 3f;
    public float yawSpeed = 80f;

    [Header("Tilt (visual only)")]
    public float forwardTiltAmount = 15f;
    public float sideTiltAmount = 12f;
    public float tiltSmooth = 5f;

    [Header("Idle Bob / Wobble (visual)")]
    public float bobSpeed = 1.8f;
    public float bobHeight = 0.06f;
    public float wobbleAmount = 0.8f;
    public float wobbleSpeed = 1.5f;

    [Header("Smoothing")]
    public float movementSmooth = 12f;  // Increased for less jitter
    public float rotationSmooth = 6f;
    public float inputSmooth = 15f;      // Increased for smoother input

    // Internal state
    private float currentPropRPM = 0f;
    private Vector3 velocity = Vector3.zero;
    private float yawVelocity = 0f;
    private Vector3 visualLocalStartPos;
    private Rigidbody rb;

    // Smooth inputs
    public float smoothForward = 0f;
    public float smoothStrafe = 0f;
    public float smoothYaw = 0f;
    public float smoothVertical = 0f;

    // States
    private enum DroneState { Grounded, TakingOff, Flying, Landing }
    private DroneState state = DroneState.Grounded;

    private float groundLevel = 0f;

    void Start()
    {
        if (visualRoot == null)
            Debug.LogWarning("VisualRoot not assigned!");

        if (visualRoot != null)
        {
            visualLocalStartPos = visualRoot.localPosition;
            // Fix model rotation if it's rotated -90 on X axis
            visualRoot.localRotation = Quaternion.Euler(-90f, 0f, 0f);
        }

        rb = GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = true;

        groundLevel = transform.position.y;

        // Ensure we start on ground
        Vector3 pos = transform.position;
        pos.y = groundLevel;
        transform.position = pos;
    }

    void Update()
    {
        float dt = Time.deltaTime;

        // Toggle engine
        if (Input.GetKeyDown(KeyCode.G))
        {
            ToggleEngine();
        }

        // Update propeller RPM
        UpdatePropRPM(dt);

        // Handle state machine
        UpdateDroneState(dt);

        // Update visuals
        UpdateVisuals(dt);
    }

    void ToggleEngine()
    {
        if (state == DroneState.Grounded)
        {
            // Start takeoff
            engineOn = true;
            state = DroneState.TakingOff;
        }
        else if (state == DroneState.Flying || state == DroneState.TakingOff)
        {
            // Start landing
            state = DroneState.Landing;
        }
        else if(state == DroneState.Landing)
        {
            engineOn = true;
            state = DroneState.TakingOff;
        }

    }

    void UpdatePropRPM(float dt)
    {
        float targetRPM = engineOn ? maxPropRPM : minPropRPM;
        float rate = engineOn ? propSpinUpRate : propSpinDownRate;
        currentPropRPM = Mathf.MoveTowards(currentPropRPM, targetRPM, rate * dt);
    }

    void UpdateDroneState(float dt)
    {
        Vector3 pos = transform.position;

        switch (state)
        {
            case DroneState.Grounded:
                // Stay on ground, props spinning down
                pos.y = groundLevel;
                transform.position = pos;
                velocity = Vector3.zero;
                smoothForward = smoothStrafe = smoothYaw = smoothVertical = 0f;

                // CRITICAL: Ensure engine is OFF and RPM is 0 when grounded
                if (engineOn && currentPropRPM <= 0f)
                {
                    engineOn = false;
                }
                break;

            case DroneState.TakingOff:
                // Wait for RPM, then lift to hover height
                if (currentPropRPM >= liftRPMThreshold)
                {
                    pos.y = Mathf.MoveTowards(pos.y, groundLevel + hoverHeight, takeoffSpeed * dt);
                    transform.position = pos;

                    if (Mathf.Abs(pos.y - (groundLevel + hoverHeight)) < 0.05f)
                    {
                        state = DroneState.Flying;
                    }
                }
                break;

            case DroneState.Flying:
                // Full flight controls
                HandleFlightInput(dt);
                break;

            case DroneState.Landing:
                // Descend to ground, limited controls
                HandleLandingInput(dt);

                pos.y = Mathf.MoveTowards(pos.y, groundLevel, landingSpeed * dt);
                transform.position = pos;

                // Check if landed
                if (Mathf.Abs(pos.y - groundLevel) < 0.01f)
                {
                    pos.y = groundLevel;
                    transform.position = pos;

                    // FIXED: Turn off engine immediately when touching ground
                    engineOn = false;

                    // Wait for props to fully stop before completing landing
                    if (currentPropRPM <= 1f)
                    {

                        state = DroneState.Grounded;
                        currentPropRPM = 0f;
                        velocity = Vector3.zero;
                    }
                }
                break;
        }
    }

    void HandleFlightInput(float dt)
    {
        // Get raw input - REVERSED for model orientation
        float rawForwardInput = 0f;
        if (Input.GetKey(KeyCode.W)) rawForwardInput = -1f;  // W = forward (was backward)
        if (Input.GetKey(KeyCode.S)) rawForwardInput = 1f;    // S = backward (was forward)

        float rawStrafeInput = 0f;
        if (Input.GetKey(KeyCode.D)) rawStrafeInput = 1f;
        if (Input.GetKey(KeyCode.A)) rawStrafeInput = -1f;

        float rawYawInput = 0f;
        if (Input.GetKey(KeyCode.D)) rawYawInput = 1f;
        if (Input.GetKey(KeyCode.A)) rawYawInput = -1f;

        float rawVerticalInput = 0f;
        if (Input.GetKey(KeyCode.E)) rawVerticalInput = 1f;
        if (Input.GetKey(KeyCode.Q)) rawVerticalInput = -1f;

        // Apply the back speed multiplier to the raw input *before* smoothing.
        float inputToSmoothForward = rawForwardInput;
        if (rawForwardInput > 0f) // If moving backward (S key)
        {
            inputToSmoothForward *= backSpeedMultiplier;
        }

        // Smooth inputs
        smoothForward = Mathf.Lerp(smoothForward, inputToSmoothForward, inputSmooth * dt);
        smoothStrafe = Mathf.Lerp(smoothStrafe, rawStrafeInput, inputSmooth * dt);
        smoothYaw = Mathf.Lerp(smoothYaw, rawYawInput, inputSmooth * dt);
        smoothVertical = Mathf.Lerp(smoothVertical, rawVerticalInput, inputSmooth * dt);

        // Calculate movement
        Vector3 targetVelocity = Vector3.zero;

        // Forward/back movement. smoothForward is now pre-scaled for backward movement.
        targetVelocity += transform.forward * smoothForward * moveSpeed;

        // Vertical movement
        targetVelocity.y = smoothVertical * verticalSpeed;

        // Smooth velocity MORE to eliminate jitter
        velocity = Vector3.Lerp(velocity, targetVelocity, movementSmooth * dt);

        // Apply movement
        Vector3 newPos = transform.position + velocity * dt;
        newPos.y = Mathf.Max(newPos.y, groundLevel); // Never go below ground
        transform.position = newPos;

        // Yaw rotation
        float targetYawVel = smoothYaw * yawSpeed;
        yawVelocity = Mathf.Lerp(yawVelocity, targetYawVel, rotationSmooth * dt);
        transform.Rotate(0f, yawVelocity * dt, 0f, Space.World);
    }

    void HandleLandingInput(float dt)
    {
        // Limited controls during landing (horizontal movement only, reduced speed)
        // REVERSED for model orientation
        float forwardInput = 0f;
        if (Input.GetKey(KeyCode.W)) forwardInput = -1f;
        if (Input.GetKey(KeyCode.S)) forwardInput = 1f;

        float yawInput = 0f;
        if (Input.GetKey(KeyCode.D)) yawInput = 1f;
        if (Input.GetKey(KeyCode.A)) yawInput = -1f;

        // Apply backSpeedMultiplier to landing input as well for consistency
        float inputToSmoothForward = forwardInput;
        if (forwardInput > 0f)
        {
            inputToSmoothForward *= backSpeedMultiplier;
        }


        smoothForward = Mathf.Lerp(smoothForward, inputToSmoothForward, inputSmooth * dt);
        smoothYaw = Mathf.Lerp(smoothYaw, yawInput, inputSmooth * dt);

        // Reduced movement during landing
        Vector3 targetVelocity = transform.forward * smoothForward * moveSpeed * 0.5f;
        targetVelocity.y = 0f; // No vertical control during landing

        velocity = Vector3.Lerp(velocity, targetVelocity, movementSmooth * dt);

        Vector3 newPos = transform.position + new Vector3(velocity.x, 0f, velocity.z) * dt;
        newPos.y = transform.position.y; // Y handled by landing logic
        transform.position = newPos;

        // Yaw (reduced)
        float targetYawVel = smoothYaw * yawSpeed * 0.5f;
        yawVelocity = Mathf.Lerp(yawVelocity, targetYawVel, rotationSmooth * dt);
        transform.Rotate(0f, yawVelocity * dt, 0f, Space.World);
    }

    void UpdateVisuals(float dt)
    {
        if (visualRoot == null) return;

        // Spin propellers horizontally (Z axis) based on RPM
        float propRotation = currentPropRPM * dt;
        foreach (Transform prop in propellers)
        {
            if (prop != null)
                prop.Rotate(0f, 0f, propRotation, Space.Self);
        }

        // Idle bob when airborne
        Vector3 bobPos = visualLocalStartPos;
        if (state == DroneState.Flying || state == DroneState.TakingOff)
        {
            bobPos.y += Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        }
        visualRoot.localPosition = Vector3.Lerp(visualRoot.localPosition, bobPos, 6f * dt);

        // Tilt based on movement
        float forwardTilt = -smoothForward * forwardTiltAmount;
        float sideTilt = smoothYaw * sideTiltAmount;

        // Subtle wobble
        float wobbleX = Mathf.Sin(Time.time * wobbleSpeed) * wobbleAmount;
        float wobbleZ = Mathf.Cos(Time.time * wobbleSpeed * 1.3f) * wobbleAmount;

        // --- FIXED JITTER USING QUATERNIONS ---

        // 1. Calculate the dynamic tilt/wobble as an Euler angle Vector
        Vector3 tiltEuler = new Vector3(
            forwardTilt + wobbleX, // X-axis (Pitch - forward/back tilt)
            0f,
            sideTilt + wobbleZ     // Z-axis (Roll - side-to-side tilt)
        );

        // 2. Convert the dynamic tilt to a Quaternion
        Quaternion dynamicTilt = Quaternion.Euler(tiltEuler);

        // 3. Define the base rotation that fixes the model's orientation (-90 on X)
        // This is the fixed, initial rotation set in Start().
        Quaternion baseFixedRotation = Quaternion.Euler(-90f, 0f, 0f);

        // 4. Combine the base orientation with the dynamic tilt to get the final target rotation.
        // Rotation order matters: baseFixedRotation * dynamicTilt
        Quaternion targetRotation = baseFixedRotation * dynamicTilt;

        // 5. Smoothly interpolate (Slerp) the visualRoot's local rotation towards the target.
        visualRoot.localRotation = Quaternion.Slerp(
            visualRoot.localRotation,
            targetRotation,
            tiltSmooth * dt
        );

        // --- END FIXED JITTER ---
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (visualRoot != null)
        {
            visualLocalStartPos = visualRoot.localPosition;
            // Ensure model stays at -90 rotation in editor
            visualRoot.localRotation = Quaternion.Euler(-90f, 0f, 0f);
        }
    }
#endif
}