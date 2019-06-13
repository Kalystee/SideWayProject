using System.ComponentModel;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }
    public static Vector2 WorldMousePosition;
    
    [Header("Settings"), SerializeField] private Camera gameCamera;
    [SerializeField, Range(0f, 5f)] private float smoothSpeed = 1.45f;
    [SerializeField, Range(0f, 1f)] private float distanceModifier = 0.65f;
    [SerializeField, Range(0f, 15f)] private float distanceCamera = 10f;

    [Header("Debug Values"), SerializeField, ReadOnly(true)] private Vector2 lastKnownFollowingPosition;
    [SerializeField] private Transform followTransform;
    [SerializeField] private Vector2 lookOffset;

    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogError("There is two CameraController ! Disabling duplicates !");
            this.enabled = false;
        }
        if(gameCamera == null)
        {
            Debug.LogWarning("No Camera assigned, assignating Main Camera by default. You should consider assignating it manually for security");
            gameCamera = Camera.main;
        }
        Instance = this;
    }

    public static Camera Camera
    {
        get
        {
            return Instance.gameCamera;
        }
    }

    public void SetFollowTransform(Transform transform)
    {
        this.followTransform = transform;
    }

    public void SetLookPosition(Vector2 position)
    {
        if (gameCamera != null)
        {
            this.lookOffset = position - lastKnownFollowingPosition;
            if (lookOffset.magnitude > gameCamera.orthographicSize)
            {
                lookOffset.Normalize();
                lookOffset *= gameCamera.orthographicSize;
            }
            lookOffset *= distanceModifier;
        }
    }

    public void SetLookOffset(Vector2 offset)
    {
        if (gameCamera != null)
        {
            this.lookOffset = offset;
            if (lookOffset.magnitude > gameCamera.orthographicSize)
            {
                lookOffset.Normalize();
                lookOffset *= gameCamera.orthographicSize;
            }
            lookOffset *= distanceModifier;
        }
    }

    private void Update()
    {
        WorldMousePosition = Camera.ScreenToWorldPoint(Input.mousePosition);
    }

    private void FixedUpdate()
    {
        if(followTransform != null)
        {
            lastKnownFollowingPosition = followTransform.position;
        }
        Vector3 position = Vector3.Lerp(transform.position, lastKnownFollowingPosition + lookOffset, smoothSpeed * Time.fixedDeltaTime);
        position.z = -distanceCamera;
        transform.position = position;
    }
}
