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
        if (panelSaveLoad != null) panelSaveLoad.SetActive(true);

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null) slots[i].InitSlot(i, this);
        }
    }

    public void CloseMenu()
    {
        if (panelSaveLoad != null) panelSaveLoad.SetActive(false);
    }

    public void OnSlotClicked(int slotIndex)
    {
        if (GameSystem.Instance == null)
        {
            Debug.LogError("【致命错误】GameSystem 不存在！请务必从 MainMenu 场景开始运行游戏，否则无法存读档！");
            return;
        }

        if (_isSaveMode)
        {
            Debug.Log($"【系统】正在写入世界线至槽位 {slotIndex}...");
            GameSystem.Instance.SaveGame(slotIndex);

            if (TelemetryManager.Instance != null) TelemetryManager.Instance.LogEvent("save_game", $"slot_{slotIndex}");

            if (slots[slotIndex] != null) slots[slotIndex].RefreshUI();
        }
        else
        {
            if (GameSystem.Instance.HasSaveFile(slotIndex))
            {
                Debug.Log($"【系统】正在读取槽位 {slotIndex}，重置世界线...");

                if (TelemetryManager.Instance != null) TelemetryManager.Instance.LogEvent("load_game", $"slot_{slotIndex}");
                Time.timeScale = 1f;

                GameSystem.Instance.LoadAndStartGame(slotIndex);
            }
            else
            {
                Debug.Log("【系统】当前槽位是虚无的，无法读取！");
            }
        }
    }
}