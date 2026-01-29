using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public string objectID;
    [TextArea] public string hoverThought;

    private SpriteRenderer sr;
    private Color originalColor;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null) originalColor = sr.color;
    }

    public void OnHoverEnter()
    {
        if (sr != null) sr.color = Color.yellow; 
    }

    public void OnHoverExit()
    {
        if (sr != null) sr.color = originalColor; 
    }
    private void OnMouseEnter()
    {
        var controller = Object.FindFirstObjectByType<DialogueController>();
        if (controller != null) controller.NotifyHover(this);
    }

    private void OnMouseExit()
    {
        var controller = Object.FindFirstObjectByType<DialogueController>();
        if (controller != null) controller.NotifyExit();
    }

    private void OnMouseDown()
    {
        var controller = Object.FindFirstObjectByType<DialogueController>();
        if (controller != null) controller.SelectThisObject(objectID);
    }
}