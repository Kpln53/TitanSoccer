using UnityEngine;

namespace TitanSoccer.ChanceGameplay
{
    /// <summary>
    /// Saha Ayarları
    /// Sahanın boyutlarını, kale çizgilerini ve bölgeleri görsel olarak ayarlamayı sağlar.
    /// </summary>
    public class FieldSettings : MonoBehaviour
    {
        [Header("Saha Boyutları")]
        public float width = 30f;           // Saha genişliği
        public float length = 40f;          // Saha uzunluğu (Y ekseni)
        
        [Header("Kale Ayarları")]
        public float goalWidth = 7.32f;     // Kale genişliği
        public float goalDepth = 2f;        // Kale derinliği
        public float goalHeight = 2.44f;    // Kale yüksekliği (Görsel/Fizik için)
        
        [Header("Ceza Sahası")]
        public float penaltyAreaWidth = 16.5f;
        public float penaltyAreaLength = 16.5f;

        // Hesaplanan Özellikler
        public float GoalLineY => length / 2f;
        public float TopGoalY => GoalLineY;
        public float BottomGoalY => -GoalLineY;

        private void OnValidate()
        {
            var generator = GetComponent<FieldGenerator>();
            if (generator != null)
            {
                // Notify generator to update
                #if UNITY_EDITOR
                UnityEditor.EditorApplication.delayCall += () => {
                    if (generator != null) generator.GenerateField();
                };
                #endif
            }
        }

        private void OnDrawGizmos()
        {
            // 1. Saha Sınırları (Yeşil)
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(width, length, 0));

            // 2. Orta Saha Çizgisi
            Gizmos.color = new Color(0, 1, 0, 0.5f);
            Gizmos.DrawLine(new Vector3(-width/2, 0, 0), new Vector3(width/2, 0, 0));
            Gizmos.DrawWireSphere(Vector3.zero, 3f); // Orta yuvarlak

            // 3. Kaleler (Kırmızı)
            Gizmos.color = Color.red;
            
            // Üst Kale
            Vector3 topGoalPos = new Vector3(0, GoalLineY + goalDepth/2, 0);
            Gizmos.DrawWireCube(topGoalPos, new Vector3(goalWidth, goalDepth, 0));
            
            // Alt Kale
            Vector3 bottomGoalPos = new Vector3(0, -GoalLineY - goalDepth/2, 0);
            Gizmos.DrawWireCube(bottomGoalPos, new Vector3(goalWidth, goalDepth, 0));

            // 4. Ceza Sahaları (Sarı)
            Gizmos.color = Color.yellow;
            
            // Üst Ceza Sahası
            Vector3 topPenaltyPos = new Vector3(0, GoalLineY - penaltyAreaLength/2, 0);
            Gizmos.DrawWireCube(topPenaltyPos, new Vector3(penaltyAreaWidth, penaltyAreaLength, 0));
            
            // Alt Ceza Sahası
            Vector3 bottomPenaltyPos = new Vector3(0, -GoalLineY + penaltyAreaLength/2, 0);
            Gizmos.DrawWireCube(bottomPenaltyPos, new Vector3(penaltyAreaWidth, penaltyAreaLength, 0));

            // 5. Kale Çizgileri (Mavi - Kritik)
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(new Vector3(-goalWidth/2, GoalLineY, 0), new Vector3(goalWidth/2, GoalLineY, 0));
            Gizmos.DrawLine(new Vector3(-goalWidth/2, -GoalLineY, 0), new Vector3(goalWidth/2, -GoalLineY, 0));
        }
    }
}
