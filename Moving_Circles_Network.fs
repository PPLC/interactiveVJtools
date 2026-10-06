/*{
  "DESCRIPTION": "Animated network of moving circles for VDMX. The number of circles and the neighbour/link sensitivity can be controlled via sliders.",
  "CREDIT": "Created for Timo Dufner",
  "ISFVSN": "2.0",
  "CATEGORIES": ["Generator", "Animation", "Utility"],
  "INPUTS": [
    {
      "NAME": "pointCount",
      "TYPE": "float",
      "LABEL": "Circle Count",
      "DEFAULT": 10.0,
      "MIN": 1.0,
      "MAX": 24.0
    },
    {
      "NAME": "linkDistance",
      "TYPE": "float",
      "LABEL": "Neighbour Sensitivity",
      "DEFAULT": 0.22,
      "MIN": 0.02,
      "MAX": 0.7
    },
    {
      "NAME": "speed",
      "TYPE": "float",
      "LABEL": "Movement Speed",
      "DEFAULT": 0.25,
      "MIN": 0.0,
      "MAX": 2.0
    },
    {
      "NAME": "circleSize",
      "TYPE": "float",
      "LABEL": "Circle Size",
      "DEFAULT": 0.012,
      "MIN": 0.002,
      "MAX": 0.05
    },
    {
      "NAME": "lineWidth",
      "TYPE": "float",
      "LABEL": "Line Width",
      "DEFAULT": 0.004,
      "MIN": 0.0005,
      "MAX": 0.03
    },
    {
      "NAME": "softness",
      "TYPE": "float",
      "LABEL": "Softness",
      "DEFAULT": 0.002,
      "MIN": 0.0001,
      "MAX": 0.03
    },
    {
      "NAME": "opacity",
      "TYPE": "float",
      "LABEL": "Opacity",
      "DEFAULT": 1.0,
      "MIN": 0.0,
      "MAX": 1.0
    }
  ]
}*/

const int MAX_POINTS = 24;
const float PI = 3.14159265359;

float hash(float n)
{
    return fract(sin(n) * 43758.5453123);
}

vec2 pointPosition(int idx, float t)
{
    float i = float(idx);

    float baseX = hash(i * 12.371 + 1.37);
    float baseY = hash(i * 19.913 + 4.11);

    float ampX = mix(0.08, 0.42, hash(i * 3.117 + 2.41));
    float ampY = mix(0.08, 0.42, hash(i * 5.731 + 7.93));

    float freqX = mix(0.18, 0.85, hash(i * 8.157 + 0.71));
    float freqY = mix(0.18, 0.85, hash(i * 9.771 + 9.17));

    float phaseX = hash(i * 11.41 + 3.19) * PI * 2.0;
    float phaseY = hash(i * 15.93 + 5.87) * PI * 2.0;

    float x = baseX + ampX * sin(t * freqX + phaseX);
    float y = baseY + ampY * cos(t * freqY + phaseY);

    return clamp(vec2(x, y), vec2(0.03), vec2(0.97));
}

float circleMask(vec2 uv, vec2 center, float radius, float feather)
{
    float d = length(uv - center);
    return 1.0 - smoothstep(radius, radius + feather, d);
}

float lineMask(vec2 uv, vec2 a, vec2 b, float thickness, float feather)
{
    vec2 pa = uv - a;
    vec2 ba = b - a;
    float h = clamp(dot(pa, ba) / max(dot(ba, ba), 0.000001), 0.0, 1.0);
    float d = length(pa - ba * h);
    return 1.0 - smoothstep(thickness, thickness + feather, d);
}

void main()
{
    vec2 uv = isf_FragNormCoord;
    float t = TIME * speed;

    float clampedCount = floor(clamp(pointCount, 1.0, float(MAX_POINTS)) + 0.0001);
    float feather = max(softness, 0.00001);

    float circles = 0.0;
    float lines = 0.0;

    for (int i = 0; i < MAX_POINTS; ++i)
    {
        if (float(i) >= clampedCount)
            continue;

        vec2 p1 = pointPosition(i, t);
        circles = max(circles, circleMask(uv, p1, circleSize, feather));

        for (int j = i + 1; j < MAX_POINTS; ++j)
        {
            if (float(j) >= clampedCount)
                continue;

            vec2 p2 = pointPosition(j, t);
            float d = distance(p1, p2);

            if (d <= linkDistance)
            {
                float proximity = 1.0 - smoothstep(0.0, linkDistance, d);
                float lm = lineMask(uv, p1, p2, lineWidth, feather);
                lines = max(lines, lm * proximity);
            }
        }
    }

    float alpha = max(circles, lines) * opacity;
    gl_FragColor = vec4(1.0, 1.0, 1.0, alpha);
}
