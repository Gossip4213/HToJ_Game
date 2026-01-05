using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveSlotUI : MonoBehaviour
{
    [Header("UI Components")]
    public Button btnSlot;
    public TextMeshProUGUI txtChapter; 
    public TextMeshProUGUI txtTime;    
    public TextMeshProUGUI txtEmpty;   
    public GameObject contentGroup;    

    private int _slotIndex;
    private SaveLoadMenuController _menuController;

    public void InitSlot(int index, SaveLoadMenuController controller)
    {
        _slotIndex = index;
        _menuController = controller;

        btnSlot.onClick.RemoveAllListeners();
        btnSlot.onClick.AddListener(OnClickSlot);

        RefreshUI();
    }

    public void RefreshUI()
    {
        if (GameSystem.Instance == null) return;

        var profile = GameSystem.Instance.GetSaveProfile(_slotIndex);

        if (profile != null)
        {
            if (contentGroup != null) contentGroup.SetActive(true);
            if (txtEmpty != null) txtEmpty.gameObject.SetActive(false);

            if (txtChapter != null) txtChapter.text = $"Chapter {profile.currentChapterIndex}";
            if (txtTime != null) txtTime.text = profile.saveTime;
        }
        else
        {
            if (contentGroup != null) contentGroup.SetActive(false);
            if (txtEmpty != null) txtEmpty.gameObject.SetActive(true);
        }
    }

    void OnClickSlot()
    {
        _menuController.OnSlotClicked(_slotIndex);
    }
}