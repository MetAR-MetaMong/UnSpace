#ifndef GAUSSIANBLURINCLUDE
#define GAUSSIANBLURINCLUDE

// Function to sample a texture with Gaussian weights
void GaussianBlur_float(sampler2D tex, float2 uv, float2 texelSize, float blurSize, int blurRadius, out float4 color)
{
    float4 blurColor = float4(0.0, 0.0, 0.0, 0.0);
    float totalWeight = 0.0;

    // Gaussian weights (you can adjust these for different blur strengths)
    float weights[5] = float[](0.2270270270, 0.1945945946, 0.1216216216, 0.0540540541, 0.0162162162);

    // Horizontal blur
    for (int i = -blurRadius; i <= blurRadius; ++i)
    {
        blurColor += tex2D(tex, uv + float2(i * texelSize.x * blurSize, 0.0)) * weights[i + blurRadius];
        totalWeight += weights[i + blurRadius];
    }

    // Normalize by total weight
    blurColor /= totalWeight;

    // Vertical blur
    float4 finalColor = float4(0.0, 0.0, 0.0, 0.0);
    totalWeight = 0.0;

    for (int i = -blurRadius; i <= blurRadius; ++i)
    {
        finalColor += tex2D(tex, uv + float2(0.0, i * texelSize.y * blurSize)) * weights[i + blurRadius];
        totalWeight += weights[i + blurRadius];
    }

    // Normalize by total weight
    finalColor /= totalWeight;

    color = finalColor;
}

#endif