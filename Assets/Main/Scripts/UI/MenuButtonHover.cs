using UnityEngine;
using TMPro;
using UnityEngine.EventSystems; 

public class MenuButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TextMeshProUGUI _text;
    private Color _originalColor;
    private Vector3 _originalPos;

    [Header("interaction")]
    public Color hoverColor = new Color(0.6f, 0.2f, 0.8f); 
    public float moveDistance = 15f; 
    public float animSpeed = 10f; 

    private Vector3 _targetPos;
    private Color _targetColor;

    void Start()
    {
        _text = GetComponent<TextMeshProUGUI>();
        _originalColor = _text.color;
        _originalPos = transform.localPosition; 

        _targetPos = _originalPos;
        _targetColor = _originalColor;
    }

    void Update()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, _targetPos, Time.deltaTime * animSpeed);
        _text.color = Color.Lerp(_text.color, _targetColor, Time.deltaTime * animSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _targetColor = hoverColor;
        _targetPos = _originalPos + new Vector3(moveDistance, 0, 0); 
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _targetColor = _originalColor;
        _targetPos = _originalPos; 
    }
}