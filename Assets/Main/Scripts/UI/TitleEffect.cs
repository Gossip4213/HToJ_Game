using UnityEngine;
using TMPro;

public class TitleMetalSafetyAFK : MonoBehaviour
{
    private TextMeshProUGUI _textMesh;
    private Material _titleMaterial;

    [Header("挂机设置")]
    public float afkDelay = 3.0f;
    public float transitionSpeed = 2.0f;

    private readonly Color[] _roleColors = new Color[]
    {
        new Color32(217, 48, 37, 255),  // Adams 红
        new Color32(46, 204, 113, 255), // Kate 绿
        new Color32(212, 172, 13, 255), // Miniel 金
        new Color32(52, 152, 219, 255), // Rumins 蓝
        new Color32(160, 32, 240, 255)  // Sera 紫
    };

    private int _currentIndex = 0;
    private float _timer;
    private float _lastInputTime;
    private Vector3 _lastMousePos;
    private float _glowAlpha = 0f;

    void Start()
    {
        _textMesh = GetComponent<TextMeshProUGUI>();

        // 获取材质球的一个副本，这样修改不会影响其他物体
        // 关键：这里用 fontSharedMaterial 的话会改掉所有字，用 materialForRendering
        _titleMaterial = _textMesh.fontMaterial;

        _lastInputTime = Time.time;
        _lastMousePos = Input.mousePosition;
    }

    void Update()
    {
        // 1. 输入检测
        if (Input.anyKey || Vector3.Distance(Input.mousePosition, _lastMousePos) > 1.0f)
        {
            _lastInputTime = Time.time;
            _lastMousePos = Input.mousePosition;
        }

        // 2. 计算权重 (0 = 不闪, 1 = 闪烁)
        float targetAlpha = (Time.time - _lastInputTime > afkDelay) ? 1f : 0f;
        _glowAlpha = Mathf.Lerp(_glowAlpha, targetAlpha, Time.deltaTime * transitionSpeed);

        // 3. 计算呼吸颜色
        _timer += Time.deltaTime;
        if (_timer >= 2.5f) { _timer = 0; _currentIndex = (_currentIndex + 1) % _roleColors.Length; }

        Color nextColor = _roleColors[(_currentIndex + 1) % _roleColors.Length];
        Color targetColor = Color.Lerp(_roleColors[_currentIndex], nextColor, _timer / 2.5f);

        // 4. 重点：只修改材质球的 Glow Color (发光颜色)
        // 我们保持文字本身的金属 Face 不动，只控制光晕的颜色和透明度
        targetColor.a = _glowAlpha; // 用权重控制光晕的亮起和熄灭

        // _GlowColor 是 TMP Shader 的标准属性名
        _titleMaterial.SetColor(ShaderUtilities.ID_GlowColor, targetColor);
    }
}