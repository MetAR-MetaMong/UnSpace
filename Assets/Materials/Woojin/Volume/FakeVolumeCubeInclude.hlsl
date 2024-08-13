#ifndef FAKEVOLUMECUBEINCLUDE
#define FAKEVOLUMECUBEINCLUDE

void CubeIntersect_float(float3 ro, float3 rd, float cameraNearPlane, out bool isInVolume, out float3 posFarOS) {
    // Initialize t to a large number for comparison
    float t[3] = {FLT_MAX, FLT_MAX, FLT_MAX};
    float ta[3] = {-FLT_MAX, -FLT_MAX, -FLT_MAX};
    // Check intersection with planes
    for (int i = 0; i < 3; i++) {
        if (rd[i] != 0) {
            float t1 = (0.5 - ro[i]) / rd[i];
            float t2 = (-0.5 - ro[i]) / rd[i];
            if (t1 > t2) {
                t[i] = t1;
                ta[i] = t2;
            } else {
                t[i] = t2;
                ta[i] = t1;
            }
        }
    }

    float tFront = min(t[0], min(t[1], t[2]));
    float tBehind = max(ta[0], max(ta[1], ta[2]));
    posFarOS = ro + rd * tFront;
    isInVolume = sign(tFront) * sign(tBehind-cameraNearPlane) < 0;

}

#endif
