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
    private bool pointerInside;
    private bool hoverApplied;

    void Start()
    {
        img = GetComponent<Image>();
        originalColor = img.color;
        originalScale = transform.localScale;
        controller = Object.FindFirstObjectByType<DialogueController>();
    }

    void Update()
    {
        if (controller == null)
        {
            controller = Object.FindFirstObjectByType<DialogueController>();
        }

        if (!pointerInside)
        {
            return;
        }

        if (CanInteract())
        {
            ApplyHover();
        }
        else
        {
            ClearHover();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        pointerInside = true;
        if (CanInteract())
        {
            ApplyHover();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerInside = false;
        ClearHover();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!CanInteract())
        {
            return;
        }

        controller.SelectThisObject(objectID);
    }

    private bool CanInteract()
    {
        return controller != null && controller.CanInteractWithSceneObjects;
    }

    private void ApplyHover()
    {
        if (hoverApplied || img == null)
        {
            return;
        }

        img.color = highlightColor;
        transform.localScale = originalScale * hoverScale;
        hoverApplied = true;
        controller.NotifyHoverUI(hoverThought);
    }

    private void ClearHover()
    {
        if (img != null)
        {
            img.color = originalColor;
        }
        transform.localScale = originalScale;

        if (hoverApplied && controller != null)
        {
            controller.NotifyExitUI();
        }

        hoverApplied = false;
    }

    void OnDisable()
    {
        pointerInside = false;
        ClearHover();
    }
}
