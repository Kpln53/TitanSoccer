using UnityEngine;
using System.IO;
using System;
using System.Linq;

/// <summary>
/// Kayıt/Yükleme sistemi (Static class)
/// </summary>
public static class SaveSystem
{
    private static string GetSaveDirectory()
    {
        return Path.Combine(Application.persistentDataPath, "Saves");
    }

    private static string GetSavePath(int slot)
    {
        return Path.Combine(GetSaveDirectory(), $"save_slot_{slot}.json");
    }

    /// <summary>
    /// Kayıt dizinini oluştur (yoksa)
    /// </summary>
    private static void EnsureSaveDirectory()
    {
        string dir = GetSaveDirectory();
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
            Debug.Log($"[SaveSystem] Created save directory: {dir}");
        }
    }

    /// <summary>
    /// Belirli bir slot için kayıt var mı kontrol et
    /// </summary>
    public static bool HasSave(int slot)
    {
        string path = GetSavePath(slot);
        bool exists = File.Exists(path);
        
        if (exists)
        {
            FileInfo fileInfo = new FileInfo(path);
            Debug.Log($"[SaveSystem] HasSave({slot}): TRUE - File size: {fileInfo.Length} bytes, Path: {path}");
        }
        else
        {
            Debug.Log($"[SaveSystem] HasSave({slot}): FALSE - Path: {path}");
        }
        
        return exists;
    }

    /// <summary>
    /// Oyunu kaydet
    /// </summary>
    public static void SaveGame(SaveData data, int slot)
    {
        Debug.Log($"[SaveSystem] SaveGame called for slot {slot}");
        
        if (data == null)
        {
            Debug.LogError("[SaveSystem] Cannot save null SaveData!");
            return;
        }

        Debug.Log($"[SaveSystem] SaveData is valid. Player: {data.playerProfile?.playerName ?? "NULL"}, Club: {data.clubData?.clubName ?? "NULL"}");

        try
        {
            EnsureSaveDirectory();
            string path = GetSavePath(slot);
            
            Debug.Log($"[SaveSystem] Save path: {path}");
            
            // DateTime'ı string'e çevir (JsonUtility DateTime'ı serialize edemez)
            if (data.saveDate == default(DateTime))
            {
                data.saveDate = DateTime.Now;
                Debug.Log("[SaveSystem] SaveDate was default, set to now");
            }
            data.saveDateString = data.saveDate.ToString("yyyy-MM-dd HH:mm:ss");
            
            Debug.Log($"[SaveSystem] Serializing SaveData to JSON...");
            
            // JSON'a çevir
            string json = JsonUtility.ToJson(data, true);
            
            if (string.IsNullOrEmpty(json))
            {
                Debug.LogError("[SaveSystem] JSON serialization resulted in empty string!");
                return;
            }
            
            Debug.Log($"[SaveSystem] JSON length: {json.Length} characters");
            Debug.Log($"[SaveSystem] Writing to file: {path}");
            
            // Dosyaya yaz
            File.WriteAllText(path, json);
            
            // Dosyanın gerçekten oluşturulduğunu kontrol et
            if (File.Exists(path))
            {
                FileInfo fileInfo = new FileInfo(path);
                Debug.Log($"[SaveSystem] ✓ Game saved successfully to slot {slot}: {path}");
                Debug.Log($"[SaveSystem] Save file size: {fileInfo.Length} bytes");
            }
            else
            {
                Debug.LogError($"[SaveSystem] ✗ File was not created! Path: {path}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveSystem] ✗ Error saving game to slot {slot}: {e.Message}");
            Debug.LogError($"[SaveSystem] StackTrace: {e.StackTrace}");
        }
    }

    /// <summary>
    /// Oyunu yükle
    /// </summary>
    public static SaveData LoadGame(int slot)
    {
        string path = GetSavePath(slot);
        
        Debug.Log($"[SaveSystem] LoadGame called for slot {slot}");
        Debug.Log($"[SaveSystem] Save path: {path}");

        if (!File.Exists(path))
        {
            Debug.LogWarning($"[SaveSystem] Save file not found: {path}");
            return null;
        }

        try
        {
            Debug.Log($"[SaveSystem] Reading file: {path}");
            string json = File.ReadAllText(path);
            
            if (string.IsNullOrEmpty(json))
            {
                Debug.LogError($"[SaveSystem] JSON file is empty for slot {slot}!");
                return null;
            }
            
            Debug.Log($"[SaveSystem] JSON length: {json.Length} characters");
            Debug.Log($"[SaveSystem] Deserializing JSON...");
            
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            
            if (data == null)
            {
                Debug.LogError($"[SaveSystem] Failed to deserialize SaveData from JSON!");
                return null;
            }
            
            // DateTime'ı string'den parse et
            if (!string.IsNullOrEmpty(data.saveDateString))
            {
                if (DateTime.TryParse(data.saveDateString, out DateTime parsedDate))
                {
                    data.saveDate = parsedDate;
                }
            }
            
            Debug.Log($"[SaveSystem] ✓ Game loaded successfully from slot {slot}: {path}");
            Debug.Log($"[SaveSystem] Player: {data.playerProfile?.playerName ?? "NULL"}, Club: {data.clubData?.clubName ?? "NULL"}");
            
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveSystem] ✗ Error loading game from slot {slot}: {e.Message}");
            Debug.LogError($"[SaveSystem] StackTrace: {e.StackTrace}");
            return null;
        }
    }

    /// <summary>
    /// Kayıt dosyasını sil
    /// </summary>
    public static void DeleteSave(int slot)
    {
        string path = GetSavePath(slot);

        if (!File.Exists(path))
        {
            Debug.LogWarning($"[SaveSystem] Save file not found for deletion: {path}");
            return;
        }

        try
        {
            File.Delete(path);
            Debug.Log($"[SaveSystem] Save file deleted: {path}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveSystem] Error deleting save file from slot {slot}: {e.Message}");
        }
    }

    /// <summary>
    /// Kayıt dosyasının tarihini getir
    /// </summary>
    public static DateTime GetSaveDate(int slot)
    {
        string path = GetSavePath(slot);

        if (!File.Exists(path))
        {
            return DateTime.MinValue;
        }

        try
        {
            return File.GetLastWriteTime(path);
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveSystem] Error getting save date for slot {slot}: {e.Message}");
            return DateTime.MinValue;
        }
    }

    /// <summary>
    /// Tüm kayıt slotlarını kontrol et ve dolu olanları döndür
    /// </summary>
    public static int[] GetUsedSlots(int maxSlots = 10)
    {
        System.Collections.Generic.List<int> usedSlots = new System.Collections.Generic.List<int>();

        for (int i = 0; i < maxSlots; i++)
        {
            if (HasSave(i))
            {
                usedSlots.Add(i);
            }
        }

        return usedSlots.ToArray();
    }
}

