using UnityEngine;
using TMPro;

public class TitleMetalSafetyAFK : MonoBehaviour
{
    private TextMeshProUGUI _textMesh;
    private Material _titleMaterial;

    [Header("afk")]
    public float afkDelay = 1f;
    public float transitionSpeed = 2.0f;

    private readonly Color[] _roleColors = new Color[]
    {
        new Color32(217, 48, 37, 255),  // 
        new Color32(46, 204, 113, 255), // 
        new Color32(212, 172, 13, 255), // 
        new Color32(52, 152, 219, 255), // 
        new Color32(160, 32, 240, 255)  // 
    };

    private int _currentIndex = 0;
    private float _timer;
    private float _lastInputTime;
    private Vector3 _lastMousePos;
    private float _glowAlpha = 0f;

    void Start()
    {
        _textMesh = GetComponent<TextMeshProUGUI>();
        _titleMaterial = _textMesh.fontMaterial;

        _lastInputTime = Time.time;
        _lastMousePos = Input.mousePosition;
    }

    void Update()
    {
        if (Input.anyKey || Vector3.Distance(Input.mousePosition, _lastMousePos) > 1.0f)
        {
            _lastInputTime = Time.time;
            _lastMousePos = Input.mousePosition;
        }
        float targetAlpha = (Time.time - _lastInputTime > afkDelay) ? 1f : 0f;
        _glowAlpha = Mathf.Lerp(_glowAlpha, targetAlpha, Time.deltaTime * transitionSpeed);
        _timer += Time.deltaTime;
        if (_timer >= 2.5f) { _timer = 0; _currentIndex = (_currentIndex + 1) % _roleColors.Length; }

        Color nextColor = _roleColors[(_currentIndex + 1) % _roleColors.Length];
        Color targetColor = Color.Lerp(_roleColors[_currentIndex], nextColor, _timer / 2.5f);
        targetColor.a = _glowAlpha; 
        _titleMaterial.SetColor(ShaderUtilities.ID_GlowColor, targetColor);
    }
}