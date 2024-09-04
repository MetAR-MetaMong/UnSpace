using System.Collections.Generic;
using UnityEngine;

public class InfoMover : MonoBehaviour
{
    [System.Serializable]
    class instantiatedInfo{
        public CheckBox source;
        public float depth;
        public int collisionCount;
        public RectTransform trs;
        public LineRenderer lineRenderer;

        public instantiatedInfo(CheckBox source, float depth, RectTransform trs, LineRenderer lineRenderer){
            this.source = source;
            this.depth = depth;
            this.collisionCount = 0;
            this.trs = trs;
            this.lineRenderer = lineRenderer;
        }
    }

    public Vector3 offset;
    public int padding = 10;

    public List<CheckBox> sources = new();
    public List<int> sortedIndices = new(); // 깊이에 따른 순서
    public GameObject infoPrefab;
    [SerializeField] private List<instantiatedInfo> _instantiatedInfos = new(); // 위치를 설정할 이미지의 RectTransform

    private Camera _uiCamera; // Canvas의 Render Camera
    private RectTransform rectTransform;

    private void Awake() {
        _uiCamera = Camera.main;
        rectTransform = GetComponent<RectTransform>();
        foreach (CheckBox _ in sources) {
            // _instantiatedRectTransforms.Add(Instantiate(infoPrefab, transform).GetComponent<RectTransform>());
            var inst = Instantiate(infoPrefab, transform);
            _instantiatedInfos.Add(new instantiatedInfo(_, 0, inst.GetComponent<RectTransform>(), inst.GetComponentInChildren<LineRenderer>()));
        }
    }

    void Update()
    {
        // 각 요소의 깊이 계산
        for (int i = 0; i < _instantiatedInfos.Count; i++) {
            _instantiatedInfos[i].depth = Vector3.Dot(_instantiatedInfos[i].source.transform.position - _uiCamera.transform.position, _uiCamera.transform.forward);
            if (_instantiatedInfos[i].depth < 0) {
                _instantiatedInfos[i].trs.gameObject.SetActive(false);
            } else {
                _instantiatedInfos[i].trs.gameObject.SetActive(true);
            }
        }

        // 깊이에 따른 순서 계산
        sortedIndices.Clear();
        for (int i = 0; i < _instantiatedInfos.Count; i++)
            sortedIndices.Add(i);

        sortedIndices.Sort((a, b) => _instantiatedInfos[a].depth.CompareTo(_instantiatedInfos[b].depth));

        // 정렬된 순서에 따라 UI 요소 배치
        for (int i = 0; i < sortedIndices.Count; i++) {
            int index = sortedIndices[i];
            if (_instantiatedInfos[index].depth < 0) continue;

            // 소스의 월드 위치를 RectTransform의 로컬 위치로 변환
            Vector2 localPoint = WorldPointToCanvasPosition(_instantiatedInfos[index].source.transform.position + _instantiatedInfos[index].source.transform.TransformDirection(offset));
            

            // 요소 배치
            _instantiatedInfos[index].trs.localPosition = localPoint;
        }

        for (int i = 0; i < _instantiatedInfos.Count; i++) {
            _instantiatedInfos[i].collisionCount = 0;
        }

        for (int i = 0; i < sortedIndices.Count; i++) {
            int index = sortedIndices[i];
            if (_instantiatedInfos[index].depth < 0) continue;
            bool flag = false;
            
            // 깊이 순서에 따라 충돌 검사 및 위치 조정
            for (int n = 0; n < 5; n++) {
                flag = true;
                for (int j = i - 1; j > 0; j--) {
                    int prevIndex = sortedIndices[j];
                    if (_instantiatedInfos[prevIndex].depth < 0) continue;
                    RectTransform rect1 = _instantiatedInfos[prevIndex].trs;
                    RectTransform rect2 = _instantiatedInfos[index].trs;
                    
                    if (LocalCapsuleOverlap(rect1, rect2, out Vector3 o)) {
                        flag = false;
                        _instantiatedInfos[index].collisionCount++;
                        Vector3 target = rect2.localPosition;
                        target += o;
                        if (o.y <= 0) target = rect1.localPosition + Vector3.Scale((target - rect1.localPosition), new Vector2(1, -1));
                        //if (o == Vector3.zero) target += Vector3.up * (100 + padding * 2);
                        // Debug.Log(i + ": " + j + " => " + o);
                        // target.y = rect1.localPosition.y + 120f;
                        
                        _instantiatedInfos[index].trs.localPosition = target;
                    }
                    
                }
                if (flag) break;
            }


            Vector2 localPoint2 = WorldPointToCanvasPosition(_instantiatedInfos[index].source.transform.position);
            _instantiatedInfos[index].lineRenderer.SetPosition(0, Vector3.zero);
            _instantiatedInfos[index].lineRenderer.SetPosition(1,  localPoint2 - new Vector2(_instantiatedInfos[index].trs.localPosition.x, _instantiatedInfos[index].trs.localPosition.y));
        }
    }

    public Vector2 WorldPointToCanvasPosition(Vector3 worldPosition)
    {
        // 월드 좌표를 화면 좌표로 변환
        Vector3 screenPosition = _uiCamera.WorldToScreenPoint(worldPosition);

        // 캔버스가 스크린 스페이스일 때, 화면 좌표를 캔버스 공간으로 변환
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            screenPosition, 
            _uiCamera,
            out Vector2 localPosition
        );

        return localPosition;
    }

    bool LocalCapsuleOverlap(in RectTransform rect1, in RectTransform rect2, out Vector3 offset) {
        Rect rect1Bounds = GetLocalRect(rect1);
        Rect rect2Bounds = GetLocalRect(rect2);

        Vector3 a1 = rect1Bounds.center + (rect1Bounds.width - rect1Bounds.height) * 0.5f * Vector2.right;
        Vector3 b1 = rect1Bounds.center - (rect1Bounds.width - rect1Bounds.height) * 0.5f * Vector2.right;
        float r1 = rect1Bounds.height * 0.5f + padding;

        Vector3 a2 = rect2Bounds.center + (rect2Bounds.width - rect2Bounds.height) * 0.5f * Vector2.right;
        Vector3 b2 = rect2Bounds.center - (rect2Bounds.width - rect2Bounds.height) * 0.5f * Vector2.right;
        float r2 = rect2Bounds.height * 0.5f + padding;

        bool isCollided = TestCapsuleCapsule(a1, b1, r1, a2, b2, r2, out offset);
        if (isCollided) return offset.magnitude > 0.1f;
        else return false;
    }
    bool TestCapsuleCapsule(Vector3 a1, Vector3 b1, float r1, Vector3 a2, Vector3 b2, float r2, out Vector3 distance) {
        float distSqr = ClosestPtSegmentSegment(a1, b1, a2, b2, out float s, out float t, out Vector3 c1, out Vector3 c2);
        float radius = r1 + r2;
        if (c2 == c1) {
            distance = (r1 + r2) * Vector3.up;
        }
        else distance = -(c2 - c1) + (c2 - c1).normalized * (r1 + r2);
        return distSqr <= radius * radius;
    }

    float ClosestPtSegmentSegment(Vector3 p1, Vector3 q1, Vector3 p2, Vector3 q2,
        out float s, out float t, out Vector3 c1, out Vector3 c2)
    {
        Vector3 d1 = q1 - p1; // Direction vector of segment S1
        Vector3 d2 = q2 - p2; // Direction vector of segment S2
        Vector3 r = p1 - p2;
        float a = Vector3.Dot(d1, d1); // Squared length of segment S1
        float e = Vector3.Dot(d2, d2); // Squared length of segment S2
        float f = Vector3.Dot(d2, r);

        // Check if either or both segments degenerate into points
        if (a <= Mathf.Epsilon && e <= Mathf.Epsilon)
        {
            // Both segments degenerate into points
            s = t = 0.0f;
            c1 = p1;
            c2 = p2;
            return Vector3.SqrMagnitude(c1 - c2);
        }

        if (a <= Mathf.Epsilon)
        {
            // First segment degenerates into a point
            s = 0.0f;
            t = f / e; // s = 0 => t = (b*s + f) / e = f / e
            t = Mathf.Clamp(t, 0.0f, 1.0f);
        }
        else
        {
            float c = Vector3.Dot(d1, r);
            if (e <= Mathf.Epsilon)
            {
                // Second segment degenerates into a point
                t = 0.0f;
                s = Mathf.Clamp(-c / a, 0.0f, 1.0f); // t = 0 => s = (b*t - c) / a = -c / a
            }
            else
            {
                // The general nondegenerate case starts here
                float b = Vector3.Dot(d1, d2);
                float denom = a * e - b * b; // Always nonnegative
                // If segments not parallel, compute closest point on L1 to L2 and
                // clamp to segment S1. Else pick arbitrary s (here 0)
                if (denom != 0.0f)
                {
                    s = Mathf.Clamp((b * f - c * e) / denom, 0.0f, 1.0f);
                }
                else
                {
                    s = 0.0f;
                }

                // Compute point on L2 closest to S1(s) using
                // t = Dot((P1 + D1*s) - P2,D2) / Dot(D2,D2) = (b*s + f) / e
                t = (b * s + f) / e;

                // If t in [0,1] done. Else clamp t, recompute s for the new value
                // of t using s = Dot((P2 + D2*t) - P1,D1) / Dot(D1,D1)= (t*b - c) / a
                // and clamp s to [0, 1]
                if (t < 0.0f)
                {
                    t = 0.0f;
                    s = Mathf.Clamp(-c / a, 0.0f, 1.0f);
                }
                else if (t > 1.0f)
                {
                    t = 1.0f;
                    s = Mathf.Clamp((b - c) / a, 0.0f, 1.0f);
                }
            }
        }

        c1 = p1 + d1 * s;
        c2 = p2 + d2 * t;
        return Vector3.SqrMagnitude(c1 - c2);
    }

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
