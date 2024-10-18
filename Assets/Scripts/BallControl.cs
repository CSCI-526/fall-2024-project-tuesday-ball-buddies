using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallControl : MonoBehaviour
{
    private float jumpForce = 4000f;  
    private float moveForce = 40000f;  
    private float fallThreshold = -50f;  
    public bool onBridge = false;  
    private bool canJump = true;
    private int collisionCount = 0;

    private PlatformControl currentPlatform;
    private BridgeControl currentBridge;
    private Rigidbody rb;
    private Renderer ballRenderer; 
    private HUDManager hudManager; 
    private CheckpointManager checkpointManager;
    private StageTimeManager stageTimeManager;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.sleepThreshold = 0f;
        ballRenderer = GetComponent<Renderer>();
        hudManager = FindObjectOfType<HUDManager>();
        stageTimeManager = FindObjectOfType<StageTimeManager>();

        checkpointManager = CheckpointManager.Instance;
        if (checkpointManager == null)
        {
            Debug.LogError("CheckpointManager not found in the scene!");
        }
    }

    void Update()
    {
        if (onBridge)
        {
            HandleMovement();
        } 
        else 
        {
            if (Input.GetKeyDown(KeyCode.Space) && canJump)
            {
                ApplyJump();
                canJump = false;  // Prevent double jump
            }
        }
        if (transform.position.y < fallThreshold || Input.GetKeyDown(KeyCode.R))
        {
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
        }
    }

    void ApplyJump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
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
                ChangeColor(platformChild.gameObject, new Color(0.6f, 1f, 0.6f)); // Slightly more green
            }

            if (ballRenderer != null)
            {
                ballRenderer.material.color = Color.white;
            }
            
            if (currentBridge != null)
            {
                ChangeColor(currentBridge.gameObject, Color.white);
            }
        }

        //BRIDGE
        BridgeControl bridge = collision.gameObject.GetComponentInParent<BridgeControl>();
        if (bridge != null)
        {
            onBridge = true;
            canJump = false;
            transform.SetParent(null);

            if (currentBridge != null && currentBridge != bridge)
            {
                ChangeColor(currentBridge.gameObject, Color.white);  
                currentBridge.SetActive(false);
            }

            currentBridge = bridge;
            currentBridge.SetActive(true);

            if (ballRenderer != null)
            {
                ballRenderer.material.color = new Color(0.7f, 1f, 0.7f); // Slightly more green
            }

            // ChangeColor(currentBridge.gameObject, Color.magenta);  /

            if (currentPlatform != null)
            {
                ChangeColor(currentPlatform.gameObject, Color.white);  // White
            }
        }
        
        //GOAL
        if (collision.gameObject.CompareTag("Goal"))
        {
            if (hudManager != null)
            {
                Debug.Log("player won");
                // upload the stage time to firestore
                stageTimeManager.GameEnd();

                hudManager.ShowWinMessage();
                Time.timeScale = 0;
                rb.velocity = Vector3.zero;
            }
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
            if (!renderer.gameObject.CompareTag("Goal") && !renderer.gameObject.CompareTag("Checkpoint") && !renderer.gameObject.CompareTag("Enemy") && !renderer.gameObject.CompareTag("Star"))
            {
                renderer.material.color = color;
            }
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
            // Reset ball position and physics
            transform.position = checkpointManager.GetLastCheckpoint();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Reset platforms
            PlatformControl[] platforms = FindObjectsOfType<PlatformControl>();
            foreach (PlatformControl platform in platforms)
            {
                platform.ResetPlatform();
            }

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
            transform.position = new Vector3(-23, 12, 42);  // Adjust this to your ball's starting position
            print("!");
        }
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
}
