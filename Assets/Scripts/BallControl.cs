using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallControl : MonoBehaviour
{
    private float jumpForce = 4000f;  
    private float moveForce = 20000f;  
    private float fallThreshold = -50f;  
    public bool onBridge = false;  
    private bool canJump = true;  

    private PlatformControl currentPlatform;
    private BridgeControl currentBridge;
    private Rigidbody rb;
    private Renderer ballRenderer; 
    private HUDManager hudManager; 
    private CheckpointManager checkpointManager;
    

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ballRenderer = GetComponent<Renderer>();
        hudManager = FindObjectOfType<HUDManager>();

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
        if (transform.position.y < fallThreshold)
        {
            Debug.Log("Ball fell off");
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
        //PLATFORM
        PlatformControl platform = collision.gameObject.GetComponent<PlatformControl>();
        if (platform != null)
        {
            onBridge = false;
            canJump = true;
            
            if (currentPlatform != null)
            {
                ChangeColor(currentPlatform.gameObject, Color.white);  // White
                currentPlatform.SetActive(false);
            }

            currentPlatform = platform;
            currentPlatform.SetActive(true);
            ChangeColor(currentPlatform.gameObject, new Color(0.4f, 0.8f, 0.4f));  // Green

            if (ballRenderer != null)
            {
                ballRenderer.material.color = Color.white;  // White 
            }
            
            if (currentBridge != null)
            {
                ChangeColor(currentBridge.gameObject, Color.white);  // White
            }
        }

        //BRIDGE
        BridgeControl bridge = collision.gameObject.GetComponentInParent<BridgeControl>();
        if (bridge != null)
        {
            onBridge = true;
            canJump = false;

            if (currentBridge != null && currentBridge != bridge)
            {
                ChangeColor(currentBridge.gameObject, Color.white);  
                currentBridge.SetActive(false);
            }

            currentBridge = bridge;
            currentBridge.SetActive(true);

            if (ballRenderer != null)
            {
                ballRenderer.material.color = new Color(0.25f, 0.41f, 0.88f);  // Blue
            }

            ChangeColor(currentBridge.gameObject, Color.magenta);  // Gray

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
                hudManager.ShowWinMessage();
                Time.timeScale = 0;
                rb.velocity = Vector3.zero;
            }
        }
        
    }

    void ChangeColor(GameObject obj, Color color)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            if (!renderer.gameObject.CompareTag("Goal"))
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

    void RestartFromCheckpoint()
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

    void RestartGame()
    {
        if (checkpointManager != null)
        {
            checkpointManager.ResetCheckpoint();
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
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