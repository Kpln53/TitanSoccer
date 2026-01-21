using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// DataPack Item UI - Liste içindeki her bir DataPack item'ı için UI script'i
/// </summary>
public class DataPackItemUI : MonoBehaviour
{
    [Header("UI Elementleri")]
    public TextMeshProUGUI packNameText;        // Pack adı
    public TextMeshProUGUI packVersionText;     // Pack versiyonu
    public TextMeshProUGUI packAuthorText;      // Pack yazarı
    public Image packLogoImage;                 // Pack logosu (opsiyonel)
    public Button itemButton;                   // Tıklama butonu (genelde kendisi)

    private DataPack currentPack;

    /// <summary>
    /// DataPack bilgilerini göster
    /// </summary>
    public void Setup(DataPack pack)
    {
        currentPack = pack;

        if (pack == null)
        {
            Debug.LogWarning("[DataPackItemUI] Trying to setup with null DataPack!");
            return;
        }

        // Pack adı
        if (packNameText != null)
            packNameText.text = pack.packName ?? "Unknown Pack";

        // Pack versiyonu
        if (packVersionText != null)
            packVersionText.text = $"v{pack.packVersion ?? "1.0.0"}";

        // Pack yazarı
        if (packAuthorText != null)
            packAuthorText.text = pack.packAuthor ?? "Unknown Author";

        // Pack logosu
        if (packLogoImage != null)
        {
            if (pack.packLogo != null)
            {
                packLogoImage.sprite = pack.packLogo;
                packLogoImage.gameObject.SetActive(true);
            }
            else
            {
                packLogoImage.gameObject.SetActive(false);
            }
        }

        // Buton event'i (eğer itemButton bu GameObject'te değilse, parent'tan al)
        if (itemButton == null)
            itemButton = GetComponent<Button>();

        if (itemButton != null)
        {
            itemButton.onClick.RemoveAllListeners();
            // Event, DataPackMenuUI tarafından bağlanacak
        }
    }

    /// <summary>
    /// Pack'i getir
    /// </summary>
    public DataPack GetPack()
    {
        return currentPack;
    }

    /// <summary>
    /// Tıklama event'ini bağla (DataPackMenuUI'dan çağrılacak)
    /// </summary>
    public void SetOnClickCallback(System.Action<DataPack> callback)
    {
        if (itemButton == null)
            itemButton = GetComponent<Button>();

        if (itemButton != null && callback != null)
        {
            itemButton.onClick.RemoveAllListeners();
            itemButton.onClick.AddListener(() => callback(currentPack));
        }
    }
}








