using UnityEngine;
using System.IO;
using System;

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
        return File.Exists(GetSavePath(slot));
    }

    /// <summary>
    /// Oyunu kaydet
    /// </summary>
    public static void SaveGame(SaveData data, int slot)
    {
        if (data == null)
        {
            Debug.LogError("[SaveSystem] Cannot save null SaveData!");
            return;
        }

        try
        {
            EnsureSaveDirectory();
            string path = GetSavePath(slot);
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(path, json);
            
            Debug.Log($"[SaveSystem] Game saved to slot {slot}: {path}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveSystem] Error saving game to slot {slot}: {e.Message}");
        }
    }

    /// <summary>
    /// Oyunu yükle
    /// </summary>
    public static SaveData LoadGame(int slot)
    {
        string path = GetSavePath(slot);

        if (!File.Exists(path))
        {
            Debug.LogWarning($"[SaveSystem] Save file not found: {path}");
            return null;
        }

        try
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            
            Debug.Log($"[SaveSystem] Game loaded from slot {slot}: {path}");
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveSystem] Error loading game from slot {slot}: {e.Message}");
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

