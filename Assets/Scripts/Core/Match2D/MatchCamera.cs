using UnityEngine;
using System.Collections;

/// <summary>
/// 2D pozisyon sahnesi kamera kontrolü - Oyuncu merkezli, taktik görüş
/// </summary>
public class MatchCamera : MonoBehaviour
{
    [Header("Kamera Ayarları")]
    public float followSpeed = 5f;
    public float cameraDistance = 10f;
    public float cameraAngle = 30f; // Taktik görüş açısı (derece)
    public float orthographicSize = 60f; // 2D kamera için görüş alanı (120x120 saha için uygun)
    
    [Header("Sınırlar")]
    public float minX = 480.6165f;  // Saha merkezi (540.6165) - 60
    public float maxX = 600.6165f;  // Saha merkezi (540.6165) + 60
    public float minY = 903.9995f;  // Saha merkezi (963.9995) - 60
    public float maxY = 1023.9995f; // Saha merkezi (963.9995) + 60
    
    [Header("Saha Pozisyonu")]
    public Vector2 fieldCenter = new Vector2(540.6165f, 963.9995f); // Sahanın merkez pozisyonu

    private Transform target; // Kontrol edilen oyuncu
    private Camera cam;
    private bool isSearchingForPlayer = false;

    private void Start()
    {
        cam = GetComponent<Camera>();
        
        // 2D kamera için orthographic mode'u ayarla
        if (cam != null)
        {
            cam.orthographic = true;
            cam.orthographicSize = orthographicSize;
            Debug.Log($"[MatchCamera] Camera set to orthographic mode with size: {orthographicSize}");
        }
        
        // Oyuncular oluşturulana kadar bekle (coroutine ile)
        StartCoroutine(FindControlledPlayerCoroutine());
    }

    /// <summary>
    /// Kontrol edilen oyuncuyu bul (coroutine ile güvenli arama)
    /// </summary>
    private IEnumerator FindControlledPlayerCoroutine()
    {
        isSearchingForPlayer = true;
        
        // FieldManager hazır olana kadar bekle
        while (FieldManager.Instance == null)
        {
            yield return null;
        }

        // Oyuncular oluşturulana kadar bekle (birkaç frame)
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        
        // Oyuncuyu bul
        int maxAttempts = 100; // Maksimum 100 deneme
        int attempts = 0;
        
        while (target == null && attempts < maxAttempts)
        {
            if (FieldManager.Instance != null)
            {
                PlayerController controlledPlayer = FieldManager.Instance.GetControlledPlayer();
                if (controlledPlayer != null && controlledPlayer.transform != null)
                {
                    target = controlledPlayer.transform;
                    Debug.Log($"[MatchCamera] Found controlled player at position: {target.position}, Camera will follow.");
                    
                    // Kamerayı hemen oyuncuya odakla (ilk frame'de)
                    Vector3 initialPosition = target.position;
                    initialPosition.z = transform.position.z;
                    
                    // Sınırları kontrol et
                    initialPosition.x = Mathf.Clamp(initialPosition.x, minX, maxX);
                    initialPosition.y = Mathf.Clamp(initialPosition.y, minY, maxY);
                    
                    transform.position = initialPosition;
                    Debug.Log($"[MatchCamera] Camera initial position set to: {initialPosition} (player at: {target.position})");
                    
                    isSearchingForPlayer = false;
                    yield break;
                }
            }
            
            attempts++;
            yield return new WaitForSeconds(0.05f); // Her 0.05 saniyede bir dene
        }
        
        // Eğer hala oyuncu bulunamazsa, topu takip et (fallback)
        if (target == null)
        {
            if (BallController.Instance != null && BallController.Instance.transform != null)
            {
                target = BallController.Instance.transform;
                Debug.LogWarning("[MatchCamera] Player not found, using ball as camera target (fallback).");
            }
            else
            {
                Debug.LogError("[MatchCamera] No player or ball found! Camera will stay at origin.");
            }
        }
        
        isSearchingForPlayer = false;
    }

    private void LateUpdate()
    {
        // Eğer hedef yoksa ve arama yapılmıyorsa tekrar ara
        if (target == null && !isSearchingForPlayer)
        {
            StartCoroutine(FindControlledPlayerCoroutine());
        }
        
        // Hedef yoksa takip etme
        if (target == null)
            return;
        
        // Hedef GameObject yoksa (destroy edilmişse) temizle
        if (target.gameObject == null)
        {
            target = null;
            return;
        }

        // Oyuncu merkezli takip
        Vector3 targetPosition = target.position;
        // 2D kamera için Z pozisyonunu sabit tut (orthographic kamera için Z önemli değil ama transform için gerekli)
        targetPosition.z = transform.position.z; // Kameranın mevcut Z pozisyonunu koru

        // Sınırları kontrol et (2D sahne sınırlarına göre)
        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

        // Yumuşak takip (unscaled time kullan - zaman kırılmasından etkilenmesin)
        float speed = followSpeed * Time.unscaledDeltaTime;
        transform.position = Vector3.Lerp(transform.position, targetPosition, speed);
    }
}
