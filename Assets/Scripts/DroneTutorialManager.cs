using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class DroneTutorialManager : MonoBehaviour
{
    [Header("Tutorial Steps")]
    public TextMeshProUGUI instructionText;

    [Header("Player Setup")]
    public SmoothDroneController playerDrone;
    public Transform playerTransform;
   

    [Header("Camera Setup")]
    public Camera[] cameras; // Assign all camera angles
    private int currentCameraIndex = 0;

    [Header("Path Ring Setup")]
    public GameObject ringPrefab;
    public Transform[] pathPoints; // Points where rings will spawn
    public float ringCheckRadius = 3f;
    private List<GameObject> spawnedRings = new List<GameObject>();
    private int currentRingIndex = 0;

    [Header("Target Practice Setup")]
    public GameObject targetDronePrefab;
    public Transform[] targetSpawnPoints;
    public int[] targetsPerWave = { 3, 4, 5 }; // 3 waves with increasing difficulty
    private List<GameObject> currentTargets = new List<GameObject>();
    private int currentWave = 0;

    [Header("Final Battle Setup")]
    public Transform battleStartPoint;
    public EnemyDroneAI[] finalEnemies; // Assign enemy drones in scene

    [Header("Tutorial State")]
    private TutorialStep currentStep = TutorialStep.Welcome;
    private bool stepCompleted = false;

    private enum TutorialStep
    {
        Welcome,
        StartEngine,
        Movement,
        CameraControl,
        PathFollowing,
        TargetPractice,
        FinalBattle,
        Complete
    }

    void Start()
    {   
        // Disable final enemies
        foreach (var enemy in finalEnemies)
        {
            if (enemy != null)
                enemy.gameObject.SetActive(false);
        }

        StartCoroutine(RunTutorial());
    }

    IEnumerator RunTutorial()
    {
        yield return new WaitForSeconds(1f);

        // Step 1: Welcome
        currentStep = TutorialStep.Welcome;
        UpdateInstruction("Welcome to Drone Flight Training!\nPress SPACE to begin.");
        yield return WaitForInput(KeyCode.Space);

        // Step 2: Start Engine
        currentStep = TutorialStep.StartEngine;
        UpdateInstruction("Press G to start the drone engine.");
        yield return StartCoroutine(WaitForEngineStart());

        // Step 3: Movement Tutorial
        currentStep = TutorialStep.Movement;
        UpdateInstruction("Movement Controls:\nW/S = Forward/Backward\nA/D = Rotate Left/Right\nQ/E = Up/Down\n\nMove in all directions to continue.");
        yield return StartCoroutine(WaitForMovementPractice());

        // Step 4: Camera Control
        currentStep = TutorialStep.CameraControl;
        UpdateInstruction("Press C to cycle through camera angles.\nCycle through all " + cameras.Length + " cameras to continue.");
        yield return StartCoroutine(WaitForCameraCycle());

        // Step 5: Path Following
        currentStep = TutorialStep.PathFollowing;
        SpawnPathRings();
        UpdateInstruction("Fly through all the rings to practice navigation!\nRings passed: 0/" + pathPoints.Length);
        yield return StartCoroutine(WaitForPathCompletion());

        // Step 6: Target Practice (3 waves)
        currentStep = TutorialStep.TargetPractice;
        for (currentWave = 0; currentWave < targetsPerWave.Length; currentWave++)
        {
            UpdateInstruction("Target Practice - Wave " + (currentWave + 1) + "/" + targetsPerWave.Length +
                            "\nDestroy all targets!\nLeft Mouse Button to shoot.");
            SpawnTargetWave(currentWave);
            yield return StartCoroutine(WaitForWaveCompletion());
            yield return new WaitForSeconds(2f);
        }

        // Step 7: Final Battle
        currentStep = TutorialStep.FinalBattle;
        UpdateInstruction("Preparing for final battle...");
        yield return new WaitForSeconds(2f);

        TeleportToStartPoint();
        ActivateFinalEnemies();
        UpdateInstruction("FINAL BATTLE!\nDefeat all enemy drones to complete training!");
        yield return StartCoroutine(WaitForFinalBattle());

        // Step 8: Complete
        currentStep = TutorialStep.Complete;
        UpdateInstruction("TRAINING COMPLETE!\nYou are now a certified drone pilot!");
    }


    IEnumerator WaitForEngineStart()
    {
        while (!playerDrone.engineOn)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        UpdateInstruction("Engine started! Good job!");
        yield return new WaitForSeconds(1.5f);
    }

    IEnumerator WaitForMovementPractice()
    {
        bool movedForward = false;
        bool movedBack = false;
        bool movedUp = false;
        bool movedDown = false;
        bool rotated = false;

        Vector3 startPos = playerTransform.position;
        float startY = startPos.y;
        float startRotY = playerTransform.eulerAngles.y;

        while (!(movedForward && movedBack && movedUp && movedDown && rotated))
        {
            // Check forward movement
            if (!movedForward && Input.GetKey(KeyCode.W))
            {
                movedForward = true;
                UpdateInstruction("✓ Forward\n" +
                    (movedBack ? "✓" : "○") + " Backward\n" +
                    (movedUp ? "✓" : "○") + " Up\n" +
                    (movedDown ? "✓" : "○") + " Down\n" +
                    (rotated ? "✓" : "○") + " Rotation");
            }

            // Check backward movement
            if (!movedBack && Input.GetKey(KeyCode.S))
            {
                movedBack = true;
                UpdateInstruction((movedForward ? "✓" : "○") + " Forward\n✓ Backward\n" +
                    (movedUp ? "✓" : "○") + " Up\n" +
                    (movedDown ? "✓" : "○") + " Down\n" +
                    (rotated ? "✓" : "○") + " Rotation");
            }

            // Check upward movement
            if (!movedUp && playerTransform.position.y > startY + 1f)
            {
                movedUp = true;
                UpdateInstruction((movedForward ? "✓" : "○") + " Forward\n" +
                    (movedBack ? "✓" : "○") + " Backward\n✓ Up\n" +
                    (movedDown ? "✓" : "○") + " Down\n" +
                    (rotated ? "✓" : "○") + " Rotation");
            }

            // Check downward movement
            if (!movedDown && Input.GetKey(KeyCode.Q))
            {
                movedDown = true;
                UpdateInstruction((movedForward ? "✓" : "○") + " Forward\n" +
                    (movedBack ? "✓" : "○") + " Backward\n" +
                    (movedUp ? "✓" : "○") + " Up\n✓ Down\n" +
                    (rotated ? "✓" : "○") + " Rotation");
            }

            // Check rotation
            if (!rotated && Mathf.Abs(Mathf.DeltaAngle(startRotY, playerTransform.eulerAngles.y)) > 45f)
            {
                rotated = true;
                UpdateInstruction((movedForward ? "✓" : "○") + " Forward\n" +
                    (movedBack ? "✓" : "○") + " Backward\n" +
                    (movedUp ? "✓" : "○") + " Up\n" +
                    (movedDown ? "✓" : "○") + " Down\n✓ Rotation");
            }

            yield return null;
        }

        yield return new WaitForSeconds(1f);
        UpdateInstruction("Movement complete! Well done!");
        yield return new WaitForSeconds(1.5f);
    }

    IEnumerator WaitForCameraCycle()
    {
        int camerasViewed = 1; // assume starting cam is already seen
        int lastIndex = currentCameraIndex;

        while (camerasViewed < cameras.Length)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                currentCameraIndex = (currentCameraIndex + 1) % cameras.Length;

                if (currentCameraIndex != lastIndex)
                {
                    camerasViewed++;
                    lastIndex = currentCameraIndex;
                    UpdateInstruction("Camera " + (currentCameraIndex + 1) + "/" + cameras.Length +
                                      "\nPress C to continue cycling.");
                }
            }

            yield return null;
        }

        yield return new WaitForSeconds(1f);
        UpdateInstruction("Camera control mastered!");
        yield return new WaitForSeconds(1.5f);
    }


    void SpawnPathRings()
    {
        int ringNumber = 1;

        foreach (Transform point in pathPoints)
        {
            GameObject ring = Instantiate(ringPrefab, point.position, point.rotation);

            // Assign number inside canvas
            TextMeshProUGUI numberText = ring.GetComponentInChildren<TextMeshProUGUI>(true);

            if (numberText != null)
            {
                numberText.text = ringNumber.ToString();
            }
            else
            {
                Debug.LogWarning("Ring number text not found!");
            }

            spawnedRings.Add(ring);
            ringNumber++;
        }

        currentRingIndex = 0;
    }

    IEnumerator WaitForPathCompletion()
    {
        while (currentRingIndex < pathPoints.Length)
        {
            // Check if player is near current ring
            float dist = Vector3.Distance(playerTransform.position, pathPoints[currentRingIndex].position);

            if (dist < ringCheckRadius)
            {
                // Ring collected!
                if (spawnedRings[currentRingIndex] != null)
                {
                    Destroy(spawnedRings[currentRingIndex]);
                }

                currentRingIndex++;
                UpdateInstruction("Fly through all the rings to practice navigation!\nRings passed: " +
                                currentRingIndex + "/" + pathPoints.Length);
            }

            yield return null;
        }

        yield return new WaitForSeconds(1f);
        UpdateInstruction("Path complete! Excellent flying!");
        yield return new WaitForSeconds(1.5f);
    }

    void SpawnTargetWave(int wave)
    {
        currentTargets.Clear();
        int targetCount = targetsPerWave[wave];

        for (int i = 0; i < targetCount; i++)
        {
            if (i < targetSpawnPoints.Length)
            {
                GameObject target = Instantiate(targetDronePrefab,
                    targetSpawnPoints[i].position,
                    targetSpawnPoints[i].rotation);
                currentTargets.Add(target);
            }
        }
    }

    IEnumerator WaitForWaveCompletion()
    {
        while (true)
        {
            // Remove destroyed targets from list
            currentTargets.RemoveAll(target => target == null);

            if (currentTargets.Count == 0)
                break;

            yield return null;
        }

        UpdateInstruction("Wave " + (currentWave + 1) + " cleared!");
        yield return new WaitForSeconds(1f);
    }

    void TeleportToStartPoint()
    {
        playerTransform.position = battleStartPoint.position;
        playerTransform.rotation = battleStartPoint.rotation;
    }

    void ActivateFinalEnemies()
    {
        foreach (var enemy in finalEnemies)
        {
            if (enemy != null)
                enemy.gameObject.SetActive(true);
        }
    }

    IEnumerator WaitForFinalBattle()
    {
        while (true)
        {
            bool allDead = true;

            foreach (var enemy in finalEnemies)
            {
                if (enemy != null && enemy.gameObject.activeInHierarchy)
                {
                    allDead = false;
                    break;
                }
            }

            if (allDead)
                break;

            yield return null;
        }

        yield return new WaitForSeconds(2f);
    }

    IEnumerator WaitForInput(KeyCode key)
    {
        while (!Input.GetKeyDown(key))
        {
            yield return null;
        }
    }

    void UpdateInstruction(string text)
    {
        if (instructionText != null)
            instructionText.text = text;
    }
}