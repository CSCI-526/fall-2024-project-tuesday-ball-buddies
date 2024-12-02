using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class BallControl : MonoBehaviour
{
    public float jumpForce = 4000f;  
    private float moveForce = 40000f;  
    private float fallThreshold = -200f;  
    public bool onBridge = false;  
    private bool canJump = true;
    private int collisionCount = 0;

    private PlatformControl currentPlatform;
    private BridgeControl currentBridge;
    private Rigidbody rb;
    private Renderer ballRenderer;
    private CheckpointManager checkpointManager;
    private Vector3 initialPosition;
    private Timer timer;

    private PauseManager pauseManager;
    private GameEndManager gameEndManager;
    private StageTimeManager stageTimeManager;

    private StarAnalysis starAnalysis; // Reference to StarAnalysis
    public DeathAnalysis deathAnalysis;
    public Vector3 lastKnownPosition = new Vector3(0, 0, 0); // Used to store the last known position of the ball
    public string lastKnownLevel = ""; 
    public string userID = "test"; // Change this to the user's ID
    
    public GameObject directionIndicator_1; // Assign DirectionArrow in inspector
    public GameObject directionIndicator_2; // Assign DirectionArrow in inspector
    public GameObject directionIndicator_3; // Assign DirectionArrow in inspector
    private float indicatorOffset = 0.1f; // Reduced to 0.05 to be very close to ball surface
    private float indicatorOffsetStep = 0.5f; // Reduced to 0.05 to be very close to ball surface
    private float previousSpeed = 0.0f;
    private Color previousColor = Color.white;

    void Start()
    {
        initialPosition = transform.position;
        rb = GetComponent<Rigidbody>();
        rb.sleepThreshold = 0f;
        ballRenderer = GetComponent<Renderer>();
        pauseManager = FindAnyObjectByType<PauseManager>();
        gameEndManager = FindAnyObjectByType<GameEndManager>();
        stageTimeManager = FindAnyObjectByType<StageTimeManager>();
        starAnalysis = FindObjectOfType<StarAnalysis>();
        
        if (deathAnalysis == null)
        {
            GameObject deathAnalysisObject = new GameObject("DeathAnalysis");
            deathAnalysis = deathAnalysisObject.AddComponent<DeathAnalysis>();
        }

        checkpointManager = CheckpointManager.Instance;

        timer = FindObjectOfType<Timer>();

        if (directionIndicator_1 == null)
        {
            Debug.LogWarning("Direction Indicator not assigned! Please assign it in the Inspector.");
        }
    }

    void Update()
    {
        if (collisionCount > 0)
        {
            lastKnownPosition = transform.localPosition;
            // Debug.Log("Last known position: " + lastKnownPosition);
            // Debug.Log("Last known level: " + lastKnownLevel);
        }

        if (onBridge)
        {
            //HandleMovement();
        } 
        else 
        {
            if (Input.GetKeyDown(KeyCode.Space) && canJump)
            {
                ApplyJump();
                canJump = false;  
            }
        }

        if (transform.position.y < fallThreshold || (Input.GetKeyDown(KeyCode.R) && !pauseManager.IsPaused() && !gameEndManager.IsGameEnded()))
        {
            if (transform.position.y < fallThreshold)
            {
                Debug.Log("Fell off the map");
                deathAnalysis.uploadDeathCause(userID, "", lastKnownLevel, "falling", lastKnownPosition);
            }

            if (checkpointManager.HasCheckpoint())
            {
                Debug.Log("Respawn from checkpoint");
                RestartFromCheckpoint();
            }
            else
            {
                Debug.Log("Respawn game");
                RestartGame();
            }
        }
    }

    void LateUpdate()
    {
        if (directionIndicator_1 != null && rb != null)
        {
            Vector3 velocity = rb.velocity;
            velocity.y = 0; // Ignore vertical movement
        
            //Debug.Log($"current velocity: {velocity.magnitude}");

            if (velocity.magnitude > 0.05f)
            {
                ArrowDisplay(directionIndicator_1, velocity, 0);
            }
            else
            {
                directionIndicator_1.SetActive(false);
            }

            if (velocity.magnitude > 4.0f)
            {
                ArrowDisplay(directionIndicator_2, velocity, 1);
            }
            else
            {
                directionIndicator_2.SetActive(false);
            }

            if (velocity.magnitude > 8.0f)
            {
                ArrowDisplay(directionIndicator_3, velocity, 2);
            }
            else
            {
                directionIndicator_3.SetActive(false);
            }
        
            previousSpeed = velocity.magnitude;
        }
    }

    void ArrowDisplay(GameObject arrow, Vector3 velocity, float distance){
        arrow.SetActive(true);

        // Calculate angle from velocity
        float angle = Mathf.Atan2(velocity.x, velocity.z) * Mathf.Rad2Deg;

        // Position the indicator around the ball based on the angle
        Vector3 indicatorPosition = transform.position + new Vector3(
            Mathf.Sin(angle * Mathf.Deg2Rad) * (indicatorOffset + distance * indicatorOffsetStep),
            0,
            Mathf.Cos(angle * Mathf.Deg2Rad) * (indicatorOffset + distance * indicatorOffsetStep)
        );

        arrow.transform.position = indicatorPosition;

        // Rotate to face outward from the ball
        arrow.transform.rotation = Quaternion.Euler(0, angle, 0);

        /* ----------- 1. Determine color based on velocity magnitude ----------- */
        float maxSpeed = 20.0f; // Adjust this value based on your desired maximum speed
        float speedFraction = Mathf.Clamp(velocity.magnitude / maxSpeed, 0f, 1f);
        //Debug.Log("Velocity magnitude: " + velocity.magnitude);
        Color arrowColor = Color.Lerp(Color.green, Color.red, speedFraction);;

        /* ----------- 2. Determine color based on acceraltion ----------- */
        // float maxAcceleration = 0.08f;
        // float bufferZone = 0.005f;
        // float changeInSpeed = (velocity.magnitude - previousSpeed) / Time.deltaTime;
        // float sensitivity = 10f;

        // Color arrowColor = previousColor;
        
        // if (changeInSpeed > 0.005f)
        // {
        //     float speedFraction = Mathf.Clamp(changeInSpeed / sensitivity, 0f, 1f);
        //     Color targetColor = Color.Lerp(Color.white, Color.red, speedFraction);
        //     arrowColor = Color.Lerp(previousColor, targetColor, Time.deltaTime / 0.5f);
        // }
        // else if (changeInSpeed < -0.005f)
        // {
        //     float speedFraction = Mathf.Clamp(changeInSpeed / sensitivity, -1.0f, 0f);
        //     Color targetColor = Color.Lerp(Color.white, Color.green, -speedFraction);
        //     arrowColor = Color.Lerp(previousColor, targetColor, Time.deltaTime / 0.5f);
        // }

        Renderer[] childRenderers = arrow.GetComponentsInChildren<Renderer>();
        foreach (Renderer childRenderer in childRenderers)
        {
            childRenderer.material.color = arrowColor; 
            // previousColor = arrowColor;
        }
    }

    public void HandleRestart()
    {
        Debug.Log("Restarting game");
        Debug.Log($"HasCheckpoint: {checkpointManager.HasCheckpoint()}, LastCheckpoint: {checkpointManager.GetLastCheckpoint()}");
        RestartGame();
    }

    void ApplyJump()
    {
        Vector3 velocity = rb.velocity;
        velocity.y = 0; // Ignore vertical movement
        
        // Get normalized horizontal direction
        Vector3 horizontalDirection = velocity.normalized;
        
        // If we're moving, combine vertical jump with horizontal direction
        if (velocity.magnitude > 0.1f)
        {
            Vector3 jumpDirection = (Vector3.up + horizontalDirection * 0.025f).normalized; // Adjust 0.5f to control horizontal influence
            rb.AddForce(jumpDirection * jumpForce, ForceMode.Impulse);
        }
        else
        {
            // Regular vertical jump if not moving
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
    
    void HandleMovement()
    {
        float moveHorizontal = Input.GetKey(KeyCode.D) ? 1f : Input.GetKey(KeyCode.A) ? -1f : 0f;
        float moveVertical = Input.GetKey(KeyCode.W) ? 1f : Input.GetKey(KeyCode.S) ? -1f : 0f;

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        rb.AddForce(movement * moveForce * Time.deltaTime * 1000);  
    }
    
    void OnCollisionEnter(Collision collision)
    {
        collisionCount++;
        lastKnownLevel = collision.gameObject.name;
        //TIMER: start time on level 1
        if (collision.gameObject.name == "Level1" || collision.gameObject.name == "Level1-1") 
        {
            timer.StartTimer();
        }

        //PLATFORM
        PlatformControl platform = collision.gameObject.GetComponent<PlatformControl>();
        if (platform != null)
        {
            onBridge = false;
            canJump = true;
            transform.SetParent(collision.gameObject.transform);
            
            if (currentPlatform != null)
            {
                ChangeColor(currentPlatform.gameObject, Color.white);
                currentPlatform.SetActive(false);
            }

            currentBridge = null;
            currentPlatform = platform;
            currentPlatform.SetActive(true);
            
            Transform platformChild = currentPlatform.transform.Find("Platform");

            if (platformChild != null && platformChild.CompareTag("Platform"))
            {
                ChangeColor(platformChild.gameObject, new Color(0.7f, 1.0f, 0.7f, 1.0f)); // Green tint
            }
        }

        //BRIDGE
        BridgeControl bridge = collision.gameObject.GetComponentInParent<BridgeControl>();
        if (bridge != null)
        {
            // Reset ball size and jump force to original values
            transform.SetParent(null);   
            transform.localScale = Vector3.one * 1.554f;
            rb.mass = 100f;
            jumpForce = 4000f;

            if (ballRenderer != null)
            {
                ballRenderer.material.color = Color.white;
            }
            if (currentPlatform != null)
            {
                ChangeColor(currentPlatform.gameObject, Color.white);  // White
                currentPlatform.SetActive(false);
                currentPlatform = null;
            }

            currentBridge = bridge;
            onBridge = true;
            canJump = false;

            /*if (currentBridge != null && currentBridge != bridge)
            {
                ChangeColor(currentBridge.gameObject, Color.white);  
            }*/
        }
        
        // PowerUp
        PowerUp powerUp = collision.gameObject.GetComponent<PowerUp>();
        if (powerUp != null)
        {
            powerUp.ActivatePowerUp(this);
            Destroy(powerUp.gameObject); 
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        //GOAL
        if (collider.CompareTag("Goal"))
        {
            // Upload the stage time to Firestore
            Time.timeScale = 0;
            rb.velocity = Vector3.zero;

            stageTimeManager.AddTimestamp();
            gameEndManager.HandleGameEnd();

            // Call the UploadStarData method
            if (SceneManager.GetActiveScene().name != "Tutorial")
                starAnalysis.SubmitCollectedStar();
        }
    }


    void OnCollisionExit(Collision collision)
    {
        collisionCount--;
        if (collisionCount == 0)
            transform.SetParent(null);
        
    }

    void ChangeColor(GameObject obj, Color color)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            if (!renderer.gameObject.CompareTag("Goal") && 
                !renderer.gameObject.CompareTag("Checkpoint") && 
                !renderer.gameObject.CompareTag("Enemy") && 
                !renderer.gameObject.CompareTag("Star") && 
                !renderer.gameObject.CompareTag("Hidden") &&
                !renderer.gameObject.CompareTag("Ball") &&
                !renderer.gameObject.CompareTag("PowerUp") &&
                !renderer.gameObject.CompareTag("Arrow") &&
                !renderer.gameObject.CompareTag("Button"))
            {
                renderer.material.color = color;
            }
            if (renderer.gameObject.CompareTag("Hidden"))
                GameObject.Find("Main Camera").GetComponent<CameraController>().ChangeMat(renderer.gameObject, color);
        }
    }

    public void SetCheckpoint(Vector3 position)
    {
        checkpointManager.SetCheckpoint(position);
        Debug.Log("Checkpoint set at: " + position);
    }

    public void RestartFromCheckpoint()
    {
        if (checkpointManager != null && checkpointManager.HasCheckpoint())
        {
            // Reset platforms
            PlatformControl[] platforms = FindObjectsOfType<PlatformControl>();
            foreach (PlatformControl platform in platforms)
            {
                platform.ResetPlatform();
            }

            // Reset ball position and physics
            transform.position = checkpointManager.GetLastCheckpoint();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Reset bridges
            BridgeControl[] bridges = FindObjectsOfType<BridgeControl>();
            foreach (BridgeControl bridge in bridges)
            {
                bridge.ResetBridge();
            }

            // Reset the color of the current platform and bridge
            ResetCurrentPlatformAndBridgeColor();

            // Reset current platform and bridge
            currentPlatform = null;
            currentBridge = null;
            onBridge = false;
            canJump = true;

            Debug.Log("Restarting from checkpoint: " + transform.position);
        }
        else
        {
            Debug.LogError("No checkpoint set or CheckpointManager is null");
            RestartGame(); 
        }
    }

    void ResetCurrentPlatformAndBridgeColor()
    {
        if (currentPlatform != null)
        {
            ChangeColor(currentPlatform.gameObject, Color.white);
        }

        /*if (currentBridge != null)
        {
            ChangeColor(currentBridge.gameObject, Color.white);
        }*/
    }

    public void RestartGame()
    {
        // Reset ball's velocity and angular velocity
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Reset platforms
        PlatformControl[] platforms = FindObjectsOfType<PlatformControl>();
        foreach (PlatformControl platform in platforms)
        {
            platform.ResetPlatform();
        }

        // Reset bridges
        BridgeControl[] bridges = FindObjectsOfType<BridgeControl>(true);  // Ensure this is finding all bridges, including inactive ones
        Debug.Log("Found " + bridges.Length + " bridges.");

        foreach (BridgeControl bridge in bridges)
        {
            Debug.Log("Calling ResetBridge on " + bridge.gameObject.name);
            bridge.ResetBridge();
        }


        // Reset the color of the current platform and bridge
        ResetCurrentPlatformAndBridgeColor();

        // Reset current platform and bridge references
        currentPlatform = null;
        currentBridge = null;
        onBridge = false;
        canJump = true;

        // Check if the checkpoint manager is available
        if (checkpointManager != null && checkpointManager.HasCheckpoint())
        {
            // Use the last checkpoint if it exists
            transform.position = checkpointManager.GetLastCheckpoint();
        }
        else
        {
            // If no checkpoint, reset to the starting position (you can set this as the original spawn position)
            transform.position = initialPosition;  
            
        }
    }

    public void collideWithEnemies(string enemyName)
    {
        deathAnalysis.uploadDeathCause(userID, enemyName, lastKnownLevel, "killed_by_enemies", lastKnownPosition);
    }

    public void SetCheckpoint()
    {
        if (checkpointManager != null)
        {
            checkpointManager.SetCheckpoint(transform.position);
            Debug.Log("Checkpoint set at: " + transform.position);
        }
        else
        {
            Debug.LogError("CheckpointManager is null");
        }
    }

    public void SetJumpForce(float newJumpForce)
    {
        jumpForce = newJumpForce;
    }
    
    public PlatformControl GetCurrentPlatformControl()
    {
        return currentPlatform;
    }
}
