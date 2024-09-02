using UnityEngine;

public class InfoMover : MonoBehaviour
{
    private RectTransform _imageRectTransform; // 위치를 설정할 이미지의 RectTransform
    public Vector3 offset;
    private Camera _uiCamera; // Canvas의 Render Camera
    public Transform source; // 위치를 기준으로 할 Transform

    private void Awake() {
        _imageRectTransform = GetComponent<RectTransform>();
        _uiCamera = Camera.main;
    }

    void Update()
    {
        // 소스의 월드 위치를 화면 좌표로 변환
        Vector3 screenPoint = RectTransformUtility.WorldToScreenPoint(_uiCamera, source.position + source.TransformDirection(offset));

        // 화면 좌표를 RectTransform의 로컬 위치로 변환
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _imageRectTransform.parent as RectTransform, screenPoint, _uiCamera, out localPoint
        );

        localPoint.x = Mathf.Clamp(localPoint.x, -1000, 1000);
        localPoint.y = Mathf.Clamp(localPoint.y, -1000, 1000);
        // RectTransform의 위치를 업데이트
        _imageRectTransform.localPosition = localPoint;
    }
}
