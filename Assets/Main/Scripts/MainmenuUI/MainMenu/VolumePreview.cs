using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; 

public class VolumePreview : MonoBehaviour, IPointerUpHandler
{
    public AudioSource previewSource; 
    public AudioClip previewClip;     

    private Slider _slider;

    void Awake()
    {
        _slider = GetComponent<Slider>();
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (previewSource != null && previewClip != null)
        {
            previewSource.volume = _slider.value;
            previewSource.PlayOneShot(previewClip);
        }
    }
}