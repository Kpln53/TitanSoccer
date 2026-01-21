using UnityEngine;

namespace TitanSoccer.ChanceGameplay
{
    /// <summary>
    /// Kaleci AI
    /// Şutlara tepki verir ve kurtarış yapar
    /// </summary>
    public class GoalkeeperAI : MonoBehaviour
    {
        [Header("Kaleci Ayarları")]
        [SerializeField] private float saveSkill = 70f;         // Kurtarış yeteneği (0-100)
        [SerializeField] private float reflexSpeed = 8f;        // Tepki hızı
        [SerializeField] private float diveRange = 3f;          // Dalış mesafesi
        [SerializeField] private float positioningSpeed = 5f;   // Pozisyon alma hızı

        [Header("Kale Boyutları")]
        [SerializeField] private float goalWidth = 5f;
        [SerializeField] private float goalLineY = 10f;

        [Header("Durum")]
        [SerializeField] private bool isDiving = false;
        [SerializeField] private bool hasSaved = false;

        private Rigidbody2D rb;
        private Vector2 basePosition;
        private Vector2 diveTarget;
        private float diveTimer;
        private float diveDuration = 0.4f;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody2D>();
                rb.gravityScale = 0f;
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            }

            basePosition = transform.position;
        }

        private void Update()
        {
            if (isDiving)
            {
                UpdateDive();
            }
            else
            {
                UpdatePositioning();
            }
        }

        /// <summary>
        /// Pozisyon güncelle (topa göre)
        /// </summary>
        private void UpdatePositioning()
        {
            if (ChanceController.Instance?.Ball == null) return;

            Vector2 ballPos = ChanceController.Instance.Ball.transform.position;
            
            // Topun X pozisyonuna göre kaleci X pozisyonu
            float targetX = Mathf.Clamp(ballPos.x * 0.4f, -goalWidth / 2f + 0.5f, goalWidth / 2f - 0.5f);
            Vector2 targetPos = new Vector2(targetX, basePosition.y);

            // Yumuşak hareket
            transform.position = Vector2.Lerp(transform.position, targetPos, Time.deltaTime * positioningSpeed);
        }

        /// <summary>
        /// Dalış güncelle
        /// </summary>
        private void UpdateDive()
        {
            diveTimer += Time.deltaTime;

            if (diveTimer < diveDuration)
            {
                // Dalış hareketi
                float t = diveTimer / diveDuration;
                Vector2 divePos = Vector2.Lerp(basePosition, diveTarget, t);
                transform.position = divePos;
            }
            else
            {
                // Dalış bitti - geri dön
                isDiving = false;
                hasSaved = false;
            }
        }

        /// <summary>
        /// Kurtarış dene
        /// </summary>
        public bool TrySave(Vector2 shotPosition, float shotAccuracy)
        {
            if (isDiving) return hasSaved;

            // Şutun kale içinde olup olmadığını kontrol et
            float shotX = shotPosition.x;
            float shotY = shotPosition.y;

            bool isOnTarget = Mathf.Abs(shotX) <= goalWidth / 2f && 
                             Mathf.Abs(shotY - goalLineY) < 2f;

            if (!isOnTarget)
            {
                // Kale dışı - kaleci müdahale etmez
                return false;
            }

            // Kurtarış şansı hesapla
            float distToShot = Vector2.Distance(transform.position, shotPosition);
            bool reachable = distToShot <= diveRange;

            if (!reachable)
            {
                // Ulaşamaz
                Debug.Log("[Goalkeeper] Can't reach!");
                return false;
            }

            // Kurtarış olasılığı - reflexSpeed'i dahil et
            float baseSaveChance = saveSkill / 100f;
            float distancePenalty = (distToShot / diveRange) * 0.3f;
            float accuracyPenalty = shotAccuracy * 0.4f; // İyi şut = düşük kurtarış şansı
            
            // Reflex speed bonus - hızlı refleks = daha iyi kurtarış
            float reflexBonus = (reflexSpeed / 10f) * 0.15f; // Max %15 bonus
            
            float saveChance = Mathf.Clamp01(baseSaveChance - distancePenalty - accuracyPenalty + reflexBonus);

            Debug.Log($"[Goalkeeper] Save chance: {saveChance:F2} (dist: {distToShot:F1}, accuracy: {shotAccuracy:F2}, reflex: +{reflexBonus:F2})");

            // Dalış başlat
            StartDive(shotPosition);

            // Sonuç
            bool saved = Random.value < saveChance;
            hasSaved = saved;

            if (saved)
            {
                Debug.Log("[Goalkeeper] SAVED!");
            }
            else
            {
                Debug.Log("[Goalkeeper] Couldn't save!");
            }

            return saved;
        }

        /// <summary>
        /// Dalış başlat
        /// </summary>
        private void StartDive(Vector2 target)
        {
            isDiving = true;
            diveTimer = 0f;
            diveTarget = new Vector2(
                Mathf.Clamp(target.x, basePosition.x - diveRange, basePosition.x + diveRange),
                basePosition.y
            );

            // Görsel feedback
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = Color.cyan;
            }

            Debug.Log($"[Goalkeeper] Diving to {diveTarget}");
        }

        /// <summary>
        /// Kaleci skill ayarla
        /// </summary>
        public void SetSkill(float skill)
        {
            saveSkill = Mathf.Clamp(skill, 30f, 99f);
        }

        /// <summary>
        /// Kaleci pozisyonunu sıfırla
        /// </summary>
        public void ResetPosition()
        {
            transform.position = basePosition;
            isDiving = false;
            hasSaved = false;

            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = Color.green;
            }
        }
    }
}

