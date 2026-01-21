using UnityEngine;
using System.Collections;

/// <summary>
/// TLS Allocator hatalarını düzeltmek için agresif memory cleanup
/// </summary>
public class SimpleTLSFix : MonoBehaviour
{
    public static SimpleTLSFix Instance { get; private set; }
    
    [Header("Ayarlar")]
    public bool enableAggressiveCleanup = true;
    public float cleanupInterval = 0.1f; // 0.1 saniyede bir
    
    private int frameCounter = 0;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        if (enableAggressiveCleanup)
        {
            StartCoroutine(AggressiveCleanupCoroutine());
        }
    }
    
    private void Update()
    {
        if (!enableAggressiveCleanup) return;
        
        frameCounter++;
        
        // Her 10 frame'de bir hafif cleanup
        if (frameCounter % 10 == 0)
        {
            System.GC.Collect(0, System.GCCollectionMode.Optimized);
        }
        
        // Her 60 frame'de bir orta cleanup
        if (frameCounter % 60 == 0)
        {
            System.GC.Collect(1, System.GCCollectionMode.Optimized);
        }
        
        // Her 300 frame'de bir tam cleanup
        if (frameCounter % 300 == 0)
        {
            ManualFix();
            frameCounter = 0; // Reset counter
        }
    }
    
    private IEnumerator AggressiveCleanupCoroutine()
    {
        while (enableAggressiveCleanup)
        {
            yield return new WaitForSeconds(cleanupInterval);
            
            // Agresif GC çağrısı
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            System.GC.Collect();
            
            // Thread memory barrier
            System.Threading.Thread.MemoryBarrier();
        }
    }
    
    /// <summary>
    /// Manuel TLS fix - En agresif temizlik
    /// </summary>
    public static void ManualFix()
    {
        // Tüm generation'ları zorla temizle
        System.GC.Collect(0, System.GCCollectionMode.Forced);
        System.GC.Collect(1, System.GCCollectionMode.Forced);
        System.GC.Collect(2, System.GCCollectionMode.Forced);
        
        // Finalizer'ları bekle
        System.GC.WaitForPendingFinalizers();
        
        // Bir kez daha temizle
        System.GC.Collect();
        
        // Unity Resources temizliği
        Resources.UnloadUnusedAssets();
        
        // Thread memory barrier
        System.Threading.Thread.MemoryBarrier();
        
        // TLS cleanup (platform specific)
        TLSCleanup();
    }
    
    /// <summary>
    /// Scene değişiminde çağır
    /// </summary>
    public static void OnSceneChange()
    {
        ManualFix();
        
        // Ek temizlik
        System.GC.Collect();
        Resources.UnloadUnusedAssets();
    }
    
    /// <summary>
    /// TLS temizliği (platform specific)
    /// </summary>
    private static void TLSCleanup()
    {
        try
        {
            // Thread Local Storage temizliği
            System.Threading.Thread.MemoryBarrier();
            
            // Unity'nin internal memory pool'larını temizle
            if (Application.isPlaying)
            {
                System.GC.Collect();
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"[SimpleTLSFix] TLS cleanup failed: {ex.Message}");
        }
    }
    
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
    
    private void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus && enableAggressiveCleanup)
        {
            ManualFix();
        }
    }
    
    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus && enableAggressiveCleanup)
        {
            ManualFix();
        }
    }
}