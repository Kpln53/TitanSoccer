using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

/// <summary>
/// 2D pozisyon sahnesi için dokunmatik input handler
/// </summary>
public class MatchChanceInputHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("Pas Kontrolü")]
    public float doubleTapTime = 0.3f; // Çift dokunma zamanı
    private float lastTapTime = 0f;
    private Vector2 lastTapPosition;
    private bool waitingForSecondTap = false;

    [Header("Şut Kontrolü")]
    private bool isDragging = false;
    private Vector2 dragStartPosition;
    private PlayerController controlledPlayer;

    private void Start()
    {
        // Kontrol edilen oyuncuyu bul
        if (FieldManager.Instance != null)
        {
            controlledPlayer = FieldManager.Instance.GetControlledPlayer();
        }
    }

    private void Update()
    {
        // Çift dokunma timeout
        if (waitingForSecondTap && Time.time - lastTapTime > doubleTapTime)
        {
            // Tek dokunma - yerden pas
            HandleSingleTap(lastTapPosition);
            waitingForSecondTap = false;
        }
    }

    /// <summary>
    /// Dokunma başladı
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
        
        // Oyuncuya mı dokunuldu yoksa boş sahaya mı?
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
        
        if (hit.collider != null && hit.collider.tag == "Player")
        {
            // Oyuncuya dokunuldu - şut modu başlat
            isDragging = true;
            dragStartPosition = worldPos;
        }
        else
        {
            // Boş sahaya dokunuldu - çift dokunma kontrolü
            float timeSinceLastTap = Time.time - lastTapTime;
            
            if (timeSinceLastTap < doubleTapTime && Vector2.Distance(worldPos, lastTapPosition) < 0.5f)
            {
                // Çift dokunma - havadan pas
                HandleDoubleTap(worldPos);
                waitingForSecondTap = false;
            }
            else
            {
                // İlk dokunma - bekle
                lastTapTime = Time.time;
                lastTapPosition = worldPos;
                waitingForSecondTap = true;
            }
        }
    }

    /// <summary>
    /// Sürükleme (şut için)
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging || controlledPlayer == null)
            return;

        Vector2 currentPos = Camera.main.ScreenToWorldPoint(eventData.position);
        Vector2 direction = currentPos - dragStartPosition;
        
        // Şut yönü ve gücü hesaplanıyor (UI'da gösterilebilir)
    }

    /// <summary>
    /// Dokunma bitti
    /// </summary>
    public void OnPointerUp(PointerEventData eventData)
    {
        if (isDragging)
        {
            // Şut at
            Vector2 endPos = Camera.main.ScreenToWorldPoint(eventData.position);
            HandleShoot(dragStartPosition, endPos);
            isDragging = false;
        }
    }

    /// <summary>
    /// Tek dokunma - yerden pas veya koşu
    /// </summary>
    private void HandleSingleTap(Vector2 position)
    {
        if (controlledPlayer == null)
            return;

        // En yakın takım arkadaşını bul
        List<PlayerController> teammates = FieldManager.Instance.GetTeammates(controlledPlayer);
        PlayerController nearestTeammate = GetNearestPlayer(position, teammates);

        if (nearestTeammate != null && Vector2.Distance(position, nearestTeammate.transform.position) < 3f)
        {
            // Pas yap (yerden)
            controlledPlayer.PassTo(nearestTeammate, false);
        }
        else
        {
            // Koşu komutu
            controlledPlayer.MoveToPosition(position);
        }
    }

    /// <summary>
    /// Çift dokunma - havadan pas
    /// </summary>
    private void HandleDoubleTap(Vector2 position)
    {
        if (controlledPlayer == null)
            return;

        List<PlayerController> teammates = FieldManager.Instance.GetTeammates(controlledPlayer);
        PlayerController nearestTeammate = GetNearestPlayer(position, teammates);

        if (nearestTeammate != null)
        {
            // Havadan pas
            controlledPlayer.PassTo(nearestTeammate, true);
        }
    }

    /// <summary>
    /// Şut at
    /// </summary>
    private void HandleShoot(Vector2 startPos, Vector2 endPos)
    {
        if (controlledPlayer == null)
            return;

        Vector2 direction = endPos - startPos;
        float power = Mathf.Clamp01(direction.magnitude / 5f); // Güç 0-1 arası
        
        bool isHighShot = direction.y > 0; // Yukarı sürüklenirse havadan şut
        
        controlledPlayer.Shoot(direction.normalized, power, isHighShot);
    }

    /// <summary>
    /// En yakın oyuncuyu bul
    /// </summary>
    private PlayerController GetNearestPlayer(Vector2 position, List<PlayerController> players)
    {
        PlayerController nearest = null;
        float minDistance = float.MaxValue;

        foreach (var player in players)
        {
            if (player == controlledPlayer)
                continue;

            float distance = Vector2.Distance(position, player.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = player;
            }
        }

        return nearest;
    }
}