using UnityEngine;
using System.IO;

/// <summary>
/// Genişletilmiş Save System - Tüm verileri kaydeder
/// </summary>
public static class SaveSystem
{
    private static string GetPath(int slot)
    {
        return Path.Combine(Application.persistentDataPath, $"save_slot_{slot}.json");
    }

    public static bool HasSave(int slot)
    {
        return File.Exists(GetPath(slot));
    }

    /// <summary>
    /// Oyunu kaydet
    /// </summary>
    public static void SaveGame(SaveData data, int slot)
    {
        try
        {
            string json = JsonUtility.ToJson(data, true);
            string path = GetPath(slot);
            
            // Klasör yoksa oluştur
            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            File.WriteAllText(path, json);
            Debug.Log($"Oyun kaydedildi: Slot {slot}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Kayıt hatası: {e.Message}");
        }
    }

    /// <summary>
    /// Oyunu yükle
    /// </summary>
    public static SaveData LoadGame(int slot)
    {
        string path = GetPath(slot);

        if (!File.Exists(path))
        {
            Debug.LogWarning($"Kayıt dosyası bulunamadı: Slot {slot}");
            return null;
        }

        try
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            
            // Eski kayıt formatını yeni formata dönüştür (uyumluluk)
            MigrateOldSave(data);
            
            Debug.Log($"Oyun yüklendi: Slot {slot}");
            return data;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Yükleme hatası: {e.Message}");
            return null;
        }
    }

    /// <summary>
    /// Eski kayıt formatını yeni formata dönüştür
    /// </summary>
    private static void MigrateOldSave(SaveData data)
    {
        // Eğer playerProfile boşsa ama eski alanlar doluysa, dönüştür
        if (data.playerProfile == null || string.IsNullOrEmpty(data.playerProfile.playerName))
        {
            if (!string.IsNullOrEmpty(data.playerName))
            {
                // Eski verileri yeni formata aktar
                if (data.playerProfile == null)
                    data.playerProfile = new PlayerProfile();
                
                data.playerProfile.playerName = data.playerName;
                data.playerProfile.overall = data.overall;
                
                // Position string'ini enum'a çevir (basit)
                // Bu kısım daha detaylı yapılabilir
            }
        }
        
        // ClubData için de aynı şey
        if (data.clubData == null || string.IsNullOrEmpty(data.clubData.clubName))
        {
            if (!string.IsNullOrEmpty(data.clubName))
            {
                if (data.clubData == null)
                    data.clubData = new ClubData();
                
                data.clubData.clubName = data.clubName;
                data.clubData.leagueName = data.leagueName;
            }
        }
        
        // SeasonData için
        if (data.seasonData == null)
        {
            data.seasonData = new SeasonData();
            data.seasonData.seasonNumber = data.season;
            data.seasonData.currentWeek = 1;
        }
    }

    /// <summary>
    /// Kayıt sil
    /// </summary>
    public static void DeleteSave(int slot)
    {
        string path = GetPath(slot);
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"Kayıt silindi: Slot {slot}");
        }
    }

    /// <summary>
    /// Otomatik kayıt (maç sonu, transfer vb.)
    /// </summary>
    public static void AutoSave()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentSave != null && GameManager.Instance.CurrentSaveSlotIndex >= 0)
        {
            SaveGame(GameManager.Instance.CurrentSave, GameManager.Instance.CurrentSaveSlotIndex);
        }
    }
}
