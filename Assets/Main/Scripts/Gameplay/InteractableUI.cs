using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class InteractableUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler
{
    public string objectID;
    [TextArea] public string hoverThought;

    [Header("hover settings")]
    public Color highlightColor = new Color(0.9f, 0.9f, 0.9f, 1f);
    public float hoverScale = 1.05f;

    private Image img;
    private Color originalColor;
    private Vector3 originalScale;
    private DialogueController controller;

    void Start()
    {
        img = GetComponent<Image>();
        originalColor = img.color;
        originalScale = transform.localScale;
        controller = Object.FindFirstObjectByType<DialogueController>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        img.color = highlightColor;
        transform.localScale = originalScale * hoverScale;
        if (controller != null) controller.NotifyHoverUI(hoverThought);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        img.color = originalColor;
        transform.localScale = originalScale;
        if (controller != null) controller.NotifyExitUI();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log($"【底层检测】鼠标按下了 {gameObject.name}");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"【UI点击】点到了 {gameObject.name}，准备发送 ID: {objectID}");
        if (controller != null) controller.SelectThisObject(objectID);
    }
}