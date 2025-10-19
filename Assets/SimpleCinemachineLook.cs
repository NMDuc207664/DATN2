using Cinemachine;
using UnityEngine;

public class SimpleCinemachineLook : MonoBehaviour
{
    public static SimpleCinemachineLook Instance { get; set; }
    public float sensY = 150f;
    public float minPitch = -45f;
    public float maxPitch = 60f;

    public float sensX = 150f;
    public float minYawOffset = -60f;
    public float maxYawOffset = 60f;

    private float pitch;
    private float yawOffset;
    public CinemachineVirtualCamera vcam;

    [Header("Input Control")]
    [SerializeField] private bool enableMouseInput = true;

    [Header("Head Tracking")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private bool followHeadRotation = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
        Vector3 euler = transform.localEulerAngles;
        pitch = euler.x;
        yawOffset = 0f;
        ApplyMouseInputState();
    }

    void LateUpdate() // ✅ Dùng LateUpdate để update sau khi player đã xoay xong
    {
        if (!enableMouseInput)
        {
            ApplyMouseInputState();
            // Khi tắt mouse input, follow hoàn toàn rotation của player
            if (followHeadRotation && playerTransform != null)
            {
                transform.rotation = playerTransform.rotation;
                // Cập nhật pitch từ rotation hiện tại
                pitch = transform.rotation.eulerAngles.x;
                if (pitch > 180f) pitch -= 360f;
            }
            return;
        }

        ApplyMouseInputState();

        // ✅ Xử lý pitch khi enable mouse input
        float mouseX = Input.GetAxis("Mouse X") * sensX * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensY * Time.deltaTime;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);


        if (followHeadRotation && playerTransform != null)
        {
            yawOffset += mouseX;
            yawOffset = Mathf.Clamp(yawOffset, minYawOffset, maxYawOffset);

            // Lấy yaw từ player transform (chỉ trục Y)
            float playerYaw = playerTransform.eulerAngles.y;
            transform.rotation = Quaternion.Euler(pitch, playerYaw + yawOffset, 0f);
        }
        else
        {
            // Nếu không follow, chỉ dùng pitch
            // if (stableHead)
            transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        ApplyMouseInputState();
    }
#endif

    private void ApplyMouseInputState()
    {
        if (enableMouseInput)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void EnableMouseInput() => enableMouseInput = true;
    public void DisableMouseInput() => enableMouseInput = false;
    public bool IsMouseInputEnabled() => enableMouseInput;

    public void SetPlayerHead(Transform head)
    {
        playerTransform = head;
    }

    public void SetFollowHeadRotation(bool follow)
    {
        followHeadRotation = follow;
    }

    public void ResetYawOffset()
    {
        yawOffset = 0f;
    }

    public void ResetPitch()
    {
        pitch = 0f;
    }
}