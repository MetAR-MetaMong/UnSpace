using System.Collections.Generic;
using UnityEngine;

public class InfoMover : MonoBehaviour
{
    public Vector3 offset;
    private Camera _uiCamera; // Canvas의 Render Camera
    public List<Transform> sources = new(); // 위치를 기준으로 할 Transform
    public List<float> depths = new(); // 각 요소의 깊이
    public List<int> sortedIndices = new(); // 깊이에 따른 순서
    public GameObject infoPrefab;
    private List<RectTransform> _instantiatedRectTransforms = new(); // 위치를 설정할 이미지의 RectTransform

    private RectTransform rectTransform;

    private void Awake() {
        _uiCamera = Camera.main;
        rectTransform = GetComponent<RectTransform>();
        foreach (Transform _ in sources) {
            _instantiatedRectTransforms.Add(Instantiate(infoPrefab, transform).GetComponent<RectTransform>());
            depths.Add(0);
        }
    }

    void Update()
    {
        // 각 요소의 깊이 계산
        for (int i = 0; i < sources.Count; i++) {
            depths[i] = Vector3.Dot((sources[i].position - _uiCamera.transform.position), _uiCamera.transform.forward);
        }

        // 깊이에 따른 순서 계산
        sortedIndices.Clear();
        for (int i = 0; i < depths.Count; i++)
            sortedIndices.Add(i);

        sortedIndices.Sort((a, b) => depths[a].CompareTo(depths[b]));

        // 정렬된 순서에 따라 UI 요소 배치
        for (int i = 0; i < sortedIndices.Count; i++) {
            int index = sortedIndices[i];

            // 소스의 월드 위치를 화면 좌표로 변환
            Vector3 screenPoint = RectTransformUtility.WorldToScreenPoint(_uiCamera, sources[index].position + sources[index].TransformDirection(offset));

            // 화면 좌표를 RectTransform의 로컬 위치로 변환
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, screenPoint, _uiCamera, out localPoint
            );

            // 요소 배치
            _instantiatedRectTransforms[index].localPosition = localPoint;

            // 깊이 순서에 따라 충돌 검사 및 위치 조정
            for (int j = 0; j < i; j++) {
                int prevIndex = sortedIndices[j];
                RectTransform rect1 = _instantiatedRectTransforms[prevIndex];
                RectTransform rect2 = _instantiatedRectTransforms[index];

                if (LocalRectOverlap(rect1, rect2)) {
                    Vector3 offsetDirection = (rect2.localPosition - rect1.localPosition).normalized;
                    rect2.localPosition = rect1.localPosition + (rect2.localPosition.x - rect1.localPosition.x) * Vector3.right + 100 * Vector3.up;
                }
            }
        }
    }

    // 두 RectTransform이 로컬 좌표계에서 겹치는지 확인
    bool LocalRectOverlap(RectTransform rect1, RectTransform rect2) {
        Rect rect1Bounds = GetLocalRect(rect1);
        Rect rect2Bounds = GetLocalRect(rect2);
        return rect1Bounds.Overlaps(rect2Bounds);
    }

    // RectTransform의 로컬 좌표계에서의 바운드를 계산
    Rect GetLocalRect(RectTransform rt) {
        Vector2 size = rt.rect.size;
        Vector2 position = rt.localPosition - (Vector3)(size * 0.5f);
        return new Rect(position, size);
    }
}
