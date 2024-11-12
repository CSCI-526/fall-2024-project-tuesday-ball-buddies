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
    private HUDManager hudManager; 
    private CheckpointManager checkpointManager;
    private Vector3 initialPosition = new Vector3(-22, 40, 40); // Adjust this to your ball's starting position
    private Timer timer;

    private GameEndManager gameEndManager;
    private StageTimeManager stageTimeManager;

    private StarAnalysis starAnalysis; // Reference to StarAnalysis
    public DeathAnalysis deathAnalysis;
    public Vector3 lastKnownPosition = new Vector3(0, 0, 0); // Used to store the last known position of the ball
    public string lastKnownLevel = ""; 
    public string userID = "test"; // Change this to the user's ID
    
    public GameObject directionIndicator; // Assign DirectionArrow in inspector
    private float indicatorOffset = 0.1f; // Reduced to 0.05 to be very close to ball surface

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.sleepThreshold = 0f;
        ballRenderer = GetComponent<Renderer>();
        hudManager = FindObjectOfType<HUDManager>();
        gameEndManager = FindAnyObjectByType<GameEndManager>();
        stageTimeManager = FindAnyObjectByType<StageTimeManager>();
        starAnalysis = FindObjectOfType<StarAnalysis>();
        
        if (deathAnalysis == null)
        {
            GameObject deathAnalysisObject = new GameObject("DeathAnalysis");
            deathAnalysis = deathAnalysisObject.AddComponent<DeathAnalysis>();
        }

       

        checkpointManager = CheckpointManager.Instance;
        if (checkpointManager == null)
        {
            Debug.LogError("CheckpointManager not found in the scene!");
        }

        timer = FindObjectOfType<Timer>();

        if (directionIndicator == null)
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
        if (transform.position.y < fallThreshold || (Input.GetKeyDown(KeyCode.R) && !hudManager.getGameWon()))
        {
            if (transform.position.y < fallThreshold)
            {
                Debug.Log("Fell off the map");
                deathAnalysis.uploadDeathCause(userID, "", lastKnownLevel, "falling", lastKnownPosition);
            }
            
            // if (gameEndManager.GetIfGameEnded())
            //     return;

            Debug.Log("Respawn");
            if (checkpointManager == null)
            {
                Debug.LogError("CheckpointManager is null");
            }
            else
            {
                Debug.Log($"HasCheckpoint: {checkpointManager.HasCheckpoint()}, LastCheckpoint: {checkpointManager.GetLastCheckpoint()}");
            }


            if (checkpointManager != null && checkpointManager.HasCheckpoint())
            {
                Debug.Log("Restarting from checkpoint");
                RestartFromCheckpoint();
            }
            else
            {
                Debug.Log("Restarting game");
                RestartGame();
            }
        } else if (Input.GetKeyDown(KeyCode.R) && hudManager.getGameWon())
        {
            Debug.Log("Already won, NOT restarting game");
        }
    }

    void LateUpdate()
    {
        if (directionIndicator != null && rb != null)
        {
            Vector3 velocity = rb.velocity;
            velocity.y = 0; // Ignore vertical movement

            if (velocity.magnitude > 0.1f) // Only show/rotate when moving
            {
                directionIndicator.SetActive(true);
                
                // Calculate angle from velocity
                float angle = Mathf.Atan2(velocity.x, velocity.z) * Mathf.Rad2Deg;
                
                // Position the indicator around the ball based on the angle
                float ballRadius = transform.localScale.x / 2f;
                Vector3 indicatorPosition = transform.position + new Vector3(
                    Mathf.Sin(angle * Mathf.Deg2Rad) * (indicatorOffset),
                    0,
                    Mathf.Cos(angle * Mathf.Deg2Rad) * (indicatorOffset)
                );
                
                directionIndicator.transform.position = indicatorPosition;

                // Rotate to face outward from the ball
                directionIndicator.transform.rotation = Quaternion.Euler(0, angle, 0);
            }
            else
            {
                directionIndicator.SetActive(false);
            }
        }
    }

    public void HandleRestart()
    {
        Debug.Log("Restarting game");

        if (checkpointManager == null)
        {
            Debug.LogError("CheckpointManager is null");
        }
        else
        {
            Debug.Log($"HasCheckpoint: {checkpointManager.HasCheckpoint()}, LastCheckpoint: {checkpointManager.GetLastCheckpoint()}");
        }

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

            currentPlatform = platform;
            currentPlatform.SetActive(true);
            
            Transform platformChild = currentPlatform.transform.Find("Platform");
            if (platformChild != null && platformChild.CompareTag("Platform"))
            {
                ChangeColor(platformChild.gameObject, new Color(0.7f, 1f, 0.7f, 0.25f)); // Green tint
            }
        }

        //BRIDGE
        BridgeControl bridge = collision.gameObject.GetComponentInParent<BridgeControl>();
        if (bridge != null)
        {
            // Reset ball size and jump force to original values
            transform.localScale = Vector3.one * 1.5f;
            rb.mass = 100f;
            jumpForce = 4000f;

            if (ballRenderer != null)
            {
                ballRenderer.material.color = Color.white;
            }
            if (currentPlatform != null)
            {
                ChangeColor(currentPlatform.gameObject, Color.white);  // White
            }

            currentPlatform.SetActive(false);
            onBridge = true;
            canJump = false;
            transform.SetParent(null);

            if (currentBridge != null && currentBridge != bridge)
            {
                ChangeColor(currentBridge.gameObject, Color.white);  
            }

            currentBridge = bridge;

        }
        
        //GOAL
        if (collision.gameObject.CompareTag("Goal"))
        {
            if (hudManager != null)
            {
                Debug.Log("Player won");
                // Upload the stage time to Firestore
                hudManager.setGameWon(true);

                Time.timeScale = 0;
                rb.velocity = Vector3.zero;

                stageTimeManager.AddTimestamp();
                gameEndManager.HandleGameEnd();

                // Call the UploadStarData method
                if (SceneManager.GetActiveScene().name != "Tutorial")
                    starAnalysis.SubmitCollectedStar();
            }
        }
        
        // PowerUp
        PowerUp powerUp = collision.gameObject.GetComponent<PowerUp>();
        if (powerUp != null)
        {
            powerUp.ActivatePowerUp(this);
            Destroy(powerUp.gameObject); 
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
                !renderer.gameObject.CompareTag("PowerUp"))  // Added PowerUp check
            {
                renderer.material.color = color;
            }
            if (renderer.gameObject.CompareTag("Hidden"))
                GameObject.Find("Main Camera").GetComponent<FollowPlayer>().ChangeMat(renderer.gameObject, color);
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

        if (currentBridge != null)
        {
            ChangeColor(currentBridge.gameObject, Color.white);
        }
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
    

}
