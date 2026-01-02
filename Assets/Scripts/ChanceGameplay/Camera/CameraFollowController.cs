using UnityEngine;

/// <summary>
/// Camera Follow Controller - Kontrollü oyuncuyu takip et
/// </summary>
public class CameraFollowController : MonoBehaviour
{
    [Header("Settings")]
    public float followSpeed = 5f;
    public Vector3 offset = new Vector3(0, 0, -10f);

    private Transform target;

    private void Start()
    {
        // Camera'yı 2D ortografik yap
        Camera cam = GetComponent<Camera>();
        if (cam != null)
        {
            cam.orthographic = true;
            cam.orthographicSize = 10f;
        }
    }

    private void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desired = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, desired, Time.deltaTime * followSpeed);
        }
    }

    /// <summary>
    /// Takip edilecek hedefi ayarla
    /// </summary>
    public void SetTarget(GameObject targetObject)
    {
        if (targetObject != null)
        {
            target = targetObject.transform;
        }
    }
}

