using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }
    private Vector3 lastCheckpoint;
    private const string CHECKPOINT_X = "6.0";
    private const string CHECKPOINT_Y = "-4.085";
    private const string CHECKPOINT_Z = "57.6";

    void Awake()
    {
        ResetCheckpoint();
        
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadCheckpoint();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void LoadCheckpoint()
    {
        lastCheckpoint = new Vector3(
            PlayerPrefs.GetFloat(CHECKPOINT_X, 0f),
            PlayerPrefs.GetFloat(CHECKPOINT_Y, 0f),
            PlayerPrefs.GetFloat(CHECKPOINT_Z, 0f)
        );
    }

    public void SetCheckpoint(Vector3 position)
    {
        position = new Vector3(8.55f, -4.1f, 57f);
        lastCheckpoint = position;
        PlayerPrefs.SetFloat(CHECKPOINT_X, position.x);
        PlayerPrefs.SetFloat(CHECKPOINT_Y, position.y);
        PlayerPrefs.SetFloat(CHECKPOINT_Z, position.z);
        PlayerPrefs.Save();
        // log the checkpoint position
        Debug.Log("Checkpoint set at: " + position);
        
    }

    public Vector3 GetLastCheckpoint() => lastCheckpoint;

    public bool HasCheckpoint() => lastCheckpoint != Vector3.zero;

    public void ResetCheckpoint()
    {
        lastCheckpoint = Vector3.zero;
        PlayerPrefs.DeleteKey(CHECKPOINT_X);
        PlayerPrefs.DeleteKey(CHECKPOINT_Y);
        PlayerPrefs.DeleteKey(CHECKPOINT_Z);
        PlayerPrefs.Save();
    }
}