using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadMenuController : MonoBehaviour
{
    public GameObject panelSaveLoad; 
    public SaveSlotUI[] slots;       

    private bool _isSaveMode = false;
    void Awake()
    {
        if (panelSaveLoad != null)
        {
            panelSaveLoad.SetActive(false);
        }
    }

    public void ShowMenu(bool isSaveMode)
    {
        _isSaveMode = isSaveMode;
        panelSaveLoad.SetActive(true);
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null) slots[i].InitSlot(i, this);
        }
    }

    public void CloseMenu()
    {
        panelSaveLoad.SetActive(false);
    }

    public void OnSlotClicked(int slotIndex)
    {
        if (_isSaveMode)
        {
            // GameSystem.Instance.SaveGame(slotIndex);
            // slots[slotIndex].RefreshUI(); 
        }
        else
        {
            if (GameSystem.Instance.HasSaveFile(slotIndex))
            {
                Debug.Log($"Loading Slot {slotIndex}...");
                GameSystem.Instance.LoadAndStartGame(slotIndex);
            }
            else
            {
                Debug.Log("这个槽位是空的！");
            }
        }
    }

}