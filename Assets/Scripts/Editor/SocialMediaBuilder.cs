using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class SocialMediaBuilder : MonoBehaviour
{
    [MenuItem("TitanSoccer/UI/📱 Build Social Media UI")]
    public static void BuildUI()
    {
        GameObject canvasObj = GameObject.Find("CareerHubCanvas");
        if (!canvasObj) return;

        Transform mainPanel = canvasObj.transform.Find("MainPanel/ContentArea");
        if (!mainPanel) return;

        // 1. Clean old
        Transform old = mainPanel.Find("SocialMediaPanel");
        if (old) DestroyImmediate(old.gameObject);

        // 2. Create Panel
        GameObject panel = new GameObject("SocialMediaPanel");
        panel.transform.SetParent(mainPanel, false);
        RectTransform pr = panel.AddComponent<RectTransform>();
        pr.anchorMin = Vector2.zero;
        pr.anchorMax = Vector2.one;
        pr.offsetMin = Vector2.zero;
        pr.offsetMax = Vector2.zero;

        // Background
        Image bg = panel.AddComponent<Image>();
        bg.color = new Color(0.05f, 0.05f, 0.05f, 1f); // Dark

        SocialMediaUI uiScript = panel.AddComponent<SocialMediaUI>();

        // --- HEADER (Followers) ---
        GameObject header = new GameObject("Header");
        header.transform.SetParent(panel.transform, false);
        RectTransform hr = header.AddComponent<RectTransform>();
        hr.anchorMin = new Vector2(0.1f, 0.9f);
        hr.anchorMax = new Vector2(0.9f, 0.98f);
        
        // Frame
        Image hImg = header.AddComponent<Image>();
        hImg.color = new Color(0.15f, 0.15f, 0.15f, 0.8f);
        Outline hOut = header.AddComponent<Outline>();
        hOut.effectColor = new Color(1f, 0.8f, 0.2f, 0.5f);

        GameObject fTextObj = new GameObject("FollowersText");
        fTextObj.transform.SetParent(header.transform, false);
        RectTransform ftr = fTextObj.AddComponent<RectTransform>();
        ftr.anchorMin = Vector2.zero;
        ftr.anchorMax = Vector2.one;
        TextMeshProUGUI fText = fTextObj.AddComponent<TextMeshProUGUI>();
        fText.text = "TAKİPÇİ: 1.4M";
        fText.alignment = TextAlignmentOptions.Center;
        fText.fontSize = 32;
        fText.color = new Color(1f, 0.9f, 0.7f);
        uiScript.followersText = fText;

        // --- FEED SCROLL VIEW ---
        GameObject sv = new GameObject("PostScrollView");
        sv.transform.SetParent(panel.transform, false);
        RectTransform svr = sv.AddComponent<RectTransform>();
        svr.anchorMin = new Vector2(0, 0.15f); // Leave space for bottom button
        svr.anchorMax = new Vector2(1, 0.88f); // Below header
        svr.offsetMin = new Vector2(20, 0);
        svr.offsetMax = new Vector2(-20, 0);

        ScrollRect sr = sv.AddComponent<ScrollRect>();
        sr.horizontal = false;
        sr.vertical = true;

        GameObject vp = new GameObject("Viewport");
        vp.transform.SetParent(sv.transform, false);
        RectTransform vpr = vp.AddComponent<RectTransform>();
        vpr.anchorMin = Vector2.zero;
        vpr.anchorMax = Vector2.one;
        vp.AddComponent<Image>().color = new Color(0,0,0,0.01f);
        vp.AddComponent<Mask>().showMaskGraphic = false;
        sr.viewport = vpr;

        GameObject content = new GameObject("Content");
        content.transform.SetParent(vp.transform, false);
        RectTransform cr = content.AddComponent<RectTransform>();
        cr.anchorMin = new Vector2(0, 1);
        cr.anchorMax = new Vector2(1, 1);
        cr.pivot = new Vector2(0.5f, 1);
        
        VerticalLayoutGroup vlg = content.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 30;
        vlg.padding = new RectOffset(0, 0, 20, 20);
        vlg.childControlHeight = true;
        vlg.childControlWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.childForceExpandWidth = true;

        ContentSizeFitter csf = content.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        sr.content = cr;
        uiScript.feedContent = content.transform;

        // --- CREATE POST BUTTON (Bottom Center) ---
        GameObject createBtn = new GameObject("CreatePostButton");
        createBtn.transform.SetParent(panel.transform, false);
        RectTransform cbr = createBtn.AddComponent<RectTransform>();
        cbr.anchorMin = new Vector2(0.5f, 0.02f);
        cbr.anchorMax = new Vector2(0.5f, 0.02f);
        cbr.pivot = new Vector2(0.5f, 0);
        cbr.sizeDelta = new Vector2(100, 100); // Circle button
        cbr.anchoredPosition = new Vector2(0, 20);

        Image cbImg = createBtn.AddComponent<Image>();
        cbImg.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd"); // Circle shape
        cbImg.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);
        Outline cbOut = createBtn.AddComponent<Outline>();
        cbOut.effectColor = new Color(1f, 0.8f, 0.2f);
        cbOut.effectDistance = new Vector2(3, 3);

        Button cb = createBtn.AddComponent<Button>();
        cb.targetGraphic = cbImg;
        uiScript.createPostButton = cb;

        GameObject plusTxt = new GameObject("Text");
        plusTxt.transform.SetParent(createBtn.transform, false);
        RectTransform ptr = plusTxt.AddComponent<RectTransform>();
        ptr.anchorMin = Vector2.zero;
        ptr.anchorMax = Vector2.one;
        TextMeshProUGUI pt = plusTxt.AddComponent<TextMeshProUGUI>();
        pt.text = "+";
        pt.fontSize = 60;
        pt.alignment = TextAlignmentOptions.Center;
        pt.color = new Color(1f, 0.8f, 0.2f);

        // --- PREFABS ---
        uiScript.postPrefab = CreatePostPrefab();
        uiScript.commentItemPrefab = CreateCommentPrefab();
        uiScript.optionButtonPrefab = CreateOptionButtonPrefab();

        // --- POPUPS ---
        CreateCommentsPopup(panel.transform, uiScript);
        CreateCreatePostPopup(panel.transform, uiScript);

        // Connect to CareerHubUI
        CareerHubUI hubUI = canvasObj.GetComponent<CareerHubUI>();
        if (hubUI)
        {
            hubUI.socialMediaPanel = panel;
            hubUI.socialMediaButton = canvasObj.transform.Find("MainPanel/BottomPanel/SocialMediaButton")?.GetComponent<Button>();
            EditorUtility.SetDirty(hubUI);
        }

        Debug.Log("✅ Social Media UI Built!");
    }

    private static GameObject CreatePostPrefab()
    {
        GameObject root = new GameObject("SocialPostItem");
        RectTransform rr = root.AddComponent<RectTransform>();
        
        // Glassy BG
        Image bg = root.AddComponent<Image>();
        bg.color = new Color(0.15f, 0.15f, 0.18f, 0.9f);
        Outline outl = root.AddComponent<Outline>();
        outl.effectColor = new Color(1f, 0.8f, 0.2f, 0.3f);

        SocialPostUI ui = root.AddComponent<SocialPostUI>();

        VerticalLayoutGroup vlg = root.AddComponent<VerticalLayoutGroup>();
        vlg.padding = new RectOffset(20, 20, 20, 20);
        vlg.spacing = 15;
        vlg.childControlHeight = false;
        vlg.childForceExpandHeight = false;

        // Header (Avatar + Name)
        GameObject header = new GameObject("Header");
        header.transform.SetParent(root.transform, false);
        RectTransform hr = header.AddComponent<RectTransform>();
        hr.sizeDelta = new Vector2(0, 60);
        HorizontalLayoutGroup hlg = header.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 15;
        hlg.childControlWidth = false;
        hlg.childForceExpandWidth = false;

        // Avatar
        GameObject av = new GameObject("Avatar");
        av.transform.SetParent(header.transform, false);
        RectTransform ar = av.AddComponent<RectTransform>();
        ar.sizeDelta = new Vector2(60, 60);
        Image avImg = av.AddComponent<Image>();
        avImg.color = Color.white; // Placeholder
        av.AddComponent<Mask>().showMaskGraphic = true;
        ui.avatarImage = avImg;

        // Names
        GameObject names = new GameObject("Names");
        names.transform.SetParent(header.transform, false);
        RectTransform nr = names.AddComponent<RectTransform>();
        nr.sizeDelta = new Vector2(400, 60);
        VerticalLayoutGroup nlg = names.AddComponent<VerticalLayoutGroup>();
        nlg.childAlignment = TextAnchor.MiddleLeft;
        
        TextMeshProUGUI nTxt = CreateText(names.transform, "Author Name", 24, Color.white, FontStyles.Bold);
        nTxt.alignment = TextAlignmentOptions.Left;
        ui.authorNameText = nTxt;

        TextMeshProUGUI hTxt = CreateText(names.transform, "@handle", 18, Color.gray, FontStyles.Normal);
        hTxt.alignment = TextAlignmentOptions.Left;
        ui.handleText = hTxt;

        // Content
        TextMeshProUGUI cTxt = CreateText(root.transform, "Post content goes here...", 22, Color.white, FontStyles.Normal);
        cTxt.alignment = TextAlignmentOptions.TopLeft;
        cTxt.enableWordWrapping = true;
        ui.contentText = cTxt;

        // Image (Optional)
        GameObject pImgObj = new GameObject("PostImage");
        pImgObj.transform.SetParent(root.transform, false);
        RectTransform pir = pImgObj.AddComponent<RectTransform>();
        pir.sizeDelta = new Vector2(0, 300); // Fixed height for image
        Image pImg = pImgObj.AddComponent<Image>();
        pImg.color = Color.gray;
        ui.postImage = pImg;
        pImgObj.SetActive(false);

        // Footer (Likes, Comments)
        GameObject footer = new GameObject("Footer");
        footer.transform.SetParent(root.transform, false);
        RectTransform fr = footer.AddComponent<RectTransform>();
        fr.sizeDelta = new Vector2(0, 40);
        HorizontalLayoutGroup flg = footer.AddComponent<HorizontalLayoutGroup>();
        flg.spacing = 30;
        flg.childControlWidth = false;
        flg.childForceExpandWidth = false;

        // Like Button
        GameObject lBtn = new GameObject("LikeBtn");
        lBtn.transform.SetParent(footer.transform, false);
        RectTransform lbr = lBtn.AddComponent<RectTransform>();
        lbr.sizeDelta = new Vector2(100, 40);
        TextMeshProUGUI lTxt = CreateText(lBtn.transform, "15 Like", 18, new Color(1f, 0.8f, 0.2f), FontStyles.Bold);
        ui.likesText = lTxt;
        ui.likeButton = lBtn.AddComponent<Button>();

        // Comment Button
        GameObject cBtn = new GameObject("CommentBtn");
        cBtn.transform.SetParent(footer.transform, false);
        RectTransform cbr = cBtn.AddComponent<RectTransform>();
        cbr.sizeDelta = new Vector2(150, 40);
        TextMeshProUGUI ccTxt = CreateText(cBtn.transform, "20 Comment", 18, Color.gray, FontStyles.Normal);
        ui.commentsCountText = ccTxt;
        ui.commentButton = cBtn.AddComponent<Button>();

        // Time (Right aligned manually or via layout)
        // For simplicity, adding to footer
        TextMeshProUGUI tTxt = CreateText(footer.transform, "2s önce", 16, Color.gray, FontStyles.Italic);
        ui.timeText = tTxt;

        // Save Prefab
        string path = "Assets/Prefabs/UI/SocialPostItem.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        DestroyImmediate(root);
        return prefab;
    }

    private static GameObject CreateCommentPrefab()
    {
        GameObject root = new GameObject("CommentItem");
        RectTransform rr = root.AddComponent<RectTransform>();
        
        Image bg = root.AddComponent<Image>();
        bg.color = new Color(1,1,1,0.05f); // Very subtle bg

        VerticalLayoutGroup vlg = root.AddComponent<VerticalLayoutGroup>();
        vlg.padding = new RectOffset(10, 10, 10, 10);
        
        TextMeshProUGUI txt = CreateText(root.transform, "User: Comment", 20, Color.white, FontStyles.Normal);
        txt.alignment = TextAlignmentOptions.Left;
        txt.enableWordWrapping = true;

        string path = "Assets/Prefabs/UI/SocialCommentItem.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        DestroyImmediate(root);
        return prefab;
    }

    private static GameObject CreateOptionButtonPrefab()
    {
        GameObject root = new GameObject("OptionButton");
        RectTransform rr = root.AddComponent<RectTransform>();
        rr.sizeDelta = new Vector2(0, 80);

        Image bg = root.AddComponent<Image>();
        bg.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);
        Outline outl = root.AddComponent<Outline>();
        outl.effectColor = new Color(1f, 0.8f, 0.2f, 0.5f);

        Button btn = root.AddComponent<Button>();
        btn.targetGraphic = bg;

        TextMeshProUGUI txt = CreateText(root.transform, "Option Text", 22, Color.white, FontStyles.Normal);
        txt.alignment = TextAlignmentOptions.Left;
        txt.margin = new Vector4(20, 0, 20, 0);

        string path = "Assets/Prefabs/UI/SocialOptionButton.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        DestroyImmediate(root);
        return prefab;
    }

    private static void CreateCommentsPopup(Transform parent, SocialMediaUI ui)
    {
        GameObject popup = new GameObject("CommentsPopup");
        popup.transform.SetParent(parent, false);
        RectTransform pr = popup.AddComponent<RectTransform>();
        pr.anchorMin = Vector2.zero;
        pr.anchorMax = Vector2.one;
        
        Image bg = popup.AddComponent<Image>();
        bg.color = new Color(0,0,0,0.9f);

        // Container
        GameObject cont = new GameObject("Container");
        cont.transform.SetParent(popup.transform, false);
        RectTransform cr = cont.AddComponent<RectTransform>();
        cr.anchorMin = new Vector2(0.1f, 0.1f);
        cr.anchorMax = new Vector2(0.9f, 0.9f);
        
        Image cBg = cont.AddComponent<Image>();
        cBg.color = new Color(0.1f, 0.1f, 0.12f);
        Outline cOut = cont.AddComponent<Outline>();
        cOut.effectColor = new Color(1f, 0.8f, 0.2f);

        // Header
        GameObject header = new GameObject("Header");
        header.transform.SetParent(cont.transform, false);
        RectTransform hr = header.AddComponent<RectTransform>();
        hr.anchorMin = new Vector2(0, 0.9f);
        hr.anchorMax = new Vector2(1, 1);
        
        TextMeshProUGUI hTxt = CreateText(header.transform, "YORUMLAR", 30, Color.white, FontStyles.Bold);
        
        // Close Button
        GameObject close = new GameObject("CloseBtn");
        close.transform.SetParent(header.transform, false);
        RectTransform clr = close.AddComponent<RectTransform>();
        clr.anchorMin = new Vector2(0.9f, 0);
        clr.anchorMax = new Vector2(1, 1);
        TextMeshProUGUI clTxt = CreateText(close.transform, "X", 30, Color.red, FontStyles.Bold);
        ui.closeCommentsButton = close.AddComponent<Button>();

        // Post Content (Context)
        GameObject ctx = new GameObject("Context");
        ctx.transform.SetParent(cont.transform, false);
        RectTransform ctxr = ctx.AddComponent<RectTransform>();
        ctxr.anchorMin = new Vector2(0, 0.75f);
        ctxr.anchorMax = new Vector2(1, 0.9f);
        TextMeshProUGUI ctxTxt = CreateText(ctx.transform, "Post content...", 20, Color.gray, FontStyles.Italic);
        ui.commentsPostContent = ctxTxt;

        // Scroll View
        GameObject sv = new GameObject("ScrollView");
        sv.transform.SetParent(cont.transform, false);
        RectTransform svr = sv.AddComponent<RectTransform>();
        svr.anchorMin = new Vector2(0, 0);
        svr.anchorMax = new Vector2(1, 0.75f);
        
        ScrollRect sr = sv.AddComponent<ScrollRect>();
        sr.horizontal = false;
        sr.vertical = true;
        
        GameObject vp = new GameObject("Viewport");
        vp.transform.SetParent(sv.transform, false);
        RectTransform vpr = vp.AddComponent<RectTransform>();
        vpr.anchorMin = Vector2.zero;
        vpr.anchorMax = Vector2.one;
        vp.AddComponent<Image>().color = new Color(0,0,0,0.01f);
        vp.AddComponent<Mask>().showMaskGraphic = false;
        sr.viewport = vpr;

        GameObject content = new GameObject("Content");
        content.transform.SetParent(vp.transform, false);
        RectTransform ctr = content.AddComponent<RectTransform>();
        ctr.anchorMin = new Vector2(0, 1);
        ctr.anchorMax = new Vector2(1, 1);
        ctr.pivot = new Vector2(0.5f, 1);
        
        VerticalLayoutGroup vlg = content.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 10;
        vlg.childControlHeight = true;
        vlg.childForceExpandHeight = false;
        ContentSizeFitter csf = content.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        sr.content = ctr;
        ui.commentsContent = content.transform;
        ui.commentsPanel = popup;
    }

    private static void CreateCreatePostPopup(Transform parent, SocialMediaUI ui)
    {
        GameObject popup = new GameObject("CreatePostPopup");
        popup.transform.SetParent(parent, false);
        RectTransform pr = popup.AddComponent<RectTransform>();
        pr.anchorMin = Vector2.zero;
        pr.anchorMax = Vector2.one;
        
        Image bg = popup.AddComponent<Image>();
        bg.color = new Color(0,0,0,0.9f);

        // Container
        GameObject cont = new GameObject("Container");
        cont.transform.SetParent(popup.transform, false);
        RectTransform cr = cont.AddComponent<RectTransform>();
        cr.anchorMin = new Vector2(0.1f, 0.15f);
        cr.anchorMax = new Vector2(0.9f, 0.85f);
        
        Image cBg = cont.AddComponent<Image>();
        cBg.color = new Color(0.1f, 0.1f, 0.12f);
        Outline cOut = cont.AddComponent<Outline>();
        cOut.effectColor = new Color(1f, 0.8f, 0.2f);

        // Header
        GameObject header = new GameObject("Header");
        header.transform.SetParent(cont.transform, false);
        RectTransform hr = header.AddComponent<RectTransform>();
        hr.anchorMin = new Vector2(0, 0.9f);
        hr.anchorMax = new Vector2(1, 1);
        
        TextMeshProUGUI hTxt = CreateText(header.transform, "GÖNDERİ OLUŞTUR", 30, Color.white, FontStyles.Bold);
        
        // Close Button
        GameObject close = new GameObject("CloseBtn");
        close.transform.SetParent(header.transform, false);
        RectTransform clr = close.AddComponent<RectTransform>();
        clr.anchorMin = new Vector2(0.9f, 0);
        clr.anchorMax = new Vector2(1, 1);
        TextMeshProUGUI clTxt = CreateText(close.transform, "X", 30, Color.red, FontStyles.Bold);
        ui.closeCreatePostButton = close.AddComponent<Button>();

        // Last Match Info
        GameObject info = new GameObject("Info");
        info.transform.SetParent(cont.transform, false);
        RectTransform ir = info.AddComponent<RectTransform>();
        ir.anchorMin = new Vector2(0, 0.8f);
        ir.anchorMax = new Vector2(1, 0.9f);
        TextMeshProUGUI iTxt = CreateText(info.transform, "SON MAÇ: Real Madrid 2-1 Barcelona\nCan Yıldız: 1 Gol, 7.8 Puan", 20, new Color(1f, 0.9f, 0.7f), FontStyles.Normal);
        ui.lastMatchInfoText = iTxt;

        // Options Container
        GameObject opts = new GameObject("Options");
        opts.transform.SetParent(cont.transform, false);
        RectTransform or = opts.AddComponent<RectTransform>();
        or.anchorMin = new Vector2(0.05f, 0.05f);
        or.anchorMax = new Vector2(0.95f, 0.75f);
        
        VerticalLayoutGroup vlg = opts.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 20;
        vlg.childControlHeight = false;
        vlg.childForceExpandHeight = false;

        ui.optionsContainer = opts.transform;
        ui.createPostPanel = popup;
    }

    private static TextMeshProUGUI CreateText(Transform parent, string text, int size, Color color, FontStyles style)
    {
        GameObject obj = new GameObject("Text");
        obj.transform.SetParent(parent, false);
        RectTransform tr = obj.AddComponent<RectTransform>();
        tr.anchorMin = Vector2.zero;
        tr.anchorMax = Vector2.one;
        
        TextMeshProUGUI tmp = obj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = size;
        tmp.color = color;
        tmp.fontStyle = style;
        tmp.alignment = TextAlignmentOptions.Center;
        return tmp;
    }
}
