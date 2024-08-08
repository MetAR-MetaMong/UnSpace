#ifndef FAKEVOLUMECUBEINCLUDE
#define FAKEVOLUMECUBEINCLUDE

void CubeIntersect_float(float3 ro, float3 rd, out bool hasIntersection, out float3 posBehindOS) {
    // Initialize t to a large number for comparison
    float t[3] = {FLT_MAX, FLT_MAX, FLT_MAX};
    hasIntersection = false;

    // Check intersection with planes
    for (int i = 0; i < 3; i++) {
        if (rd[i] != 0) {
            float t1 = (0.5 - ro[i]) / rd[i];
            float t2 = (-0.5 - ro[i]) / rd[i];
            if (t1 > 0 || t2 > 0) {
                hasIntersection = true;
                t[i] = max(t1, t2);
            }
        }
    }

    if (!hasIntersection) {
        posBehindOS = ro; // Default value if no intersection
        return;
    }

    float tBehind = min(t[0], min(t[1], t[2]));
    posBehindOS = ro + rd * tBehind;
}

#endif
