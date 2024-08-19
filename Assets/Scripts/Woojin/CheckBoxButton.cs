using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CheckBoxButton : MonoBehaviour
{
    private Button _button;
    private Image _buttonImage;
    private RectTransform _rectTransform;
    private CheckBox _attachedBox;
    private MeshRenderer _renderer;

    void Awake() {
        _button = GetComponent<Button>();
        _attachedBox = GetComponentInParent<CheckBox>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rectTransform = _button.gameObject.GetComponent<RectTransform>();
        _buttonImage = _button.gameObject.GetComponent<Image>();
        _renderer = _attachedBox.GetComponent<MeshRenderer>();

    }

    void LateUpdate(){
        Vector3 pos = Camera.main.WorldToScreenPoint(_attachedBox.transform.position);
        _buttonImage.enabled = _renderer.isVisible && pos.z > 0;
        _rectTransform.position = pos;
    }
}
