/*{
  "DESCRIPTION": "Animated number nodes for VDMX. Instead of circles, each moving node shows numeric position data in parentheses, and the connecting lines display the distance between linked nodes.",
  "CREDIT": "Created for Timo Dufner",
  "ISFVSN": "2.0",
  "CATEGORIES": ["Generator", "Animation", "Utility"],
  "INPUTS": [
    {
      "NAME": "nodeCount",
      "TYPE": "float",
      "LABEL": "Node Count",
      "DEFAULT": 5.0,
      "MIN": 1.0,
      "MAX": 8.0
    },
    {
      "NAME": "linkDistance",
      "TYPE": "float",
      "LABEL": "Neighbour Sensitivity",
      "DEFAULT": 0.26,
      "MIN": 0.03,
      "MAX": 0.7
    },
    {
      "NAME": "valueMode",
      "TYPE": "long",
      "LABEL": "Node Value",
      "DEFAULT": 0,
      "VALUES": [0, 1],
      "LABELS": ["Pixel Position", "Center Offset"]
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
      "NAME": "textSize",
      "TYPE": "float",
      "LABEL": "Text Size",
      "DEFAULT": 0.030,
      "MIN": 0.010,
      "MAX": 0.060
    },
    {
      "NAME": "lineWidth",
      "TYPE": "float",
      "LABEL": "Line Width",
      "DEFAULT": 0.003,
      "MIN": 0.0005,
      "MAX": 0.02
    },
    {
      "NAME": "softness",
      "TYPE": "float",
      "LABEL": "Softness",
      "DEFAULT": 0.0015,
      "MIN": 0.0001,
      "MAX": 0.02
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

const int MAX_POINTS = 8;
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

    float ampX = mix(0.07, 0.40, hash(i * 3.117 + 2.41));
    float ampY = mix(0.07, 0.40, hash(i * 5.731 + 7.93));

    float freqX = mix(0.18, 0.85, hash(i * 8.157 + 0.71));
    float freqY = mix(0.18, 0.85, hash(i * 9.771 + 9.17));

    float phaseX = hash(i * 11.41 + 3.19) * PI * 2.0;
    float phaseY = hash(i * 15.93 + 5.87) * PI * 2.0;

    float x = baseX + ampX * sin(t * freqX + phaseX);
    float y = baseY + ampY * cos(t * freqY + phaseY);

    return clamp(vec2(x, y), vec2(0.04), vec2(0.96));
}

float lineMask(vec2 uv, vec2 a, vec2 b, float thickness, float feather)
{
    vec2 pa = uv - a;
    vec2 ba = b - a;
    float h = clamp(dot(pa, ba) / max(dot(ba, ba), 0.000001), 0.0, 1.0);
    float d = length(pa - ba * h);
    return 1.0 - smoothstep(thickness, thickness + feather, d);
}

float glyphRowBits(int cid, int row)
{
    // 3x5 bitmap font, rows top-to-bottom, values encoded as 3-bit integers.
    if (cid == 0) { if (row == 0 || row == 4) return 7.0; if (row == 1 || row == 2 || row == 3) return 5.0; }
    if (cid == 1) { if (row == 0) return 2.0; if (row == 1) return 6.0; if (row == 2 || row == 3) return 2.0; if (row == 4) return 7.0; }
    if (cid == 2) { if (row == 0 || row == 2 || row == 4) return 7.0; if (row == 1) return 1.0; if (row == 3) return 4.0; }
    if (cid == 3) { if (row == 0 || row == 2 || row == 4) return 7.0; if (row == 1 || row == 3) return 1.0; }
    if (cid == 4) { if (row == 0 || row == 1) return 5.0; if (row == 2) return 7.0; if (row == 3 || row == 4) return 1.0; }
    if (cid == 5) { if (row == 0 || row == 2 || row == 4) return 7.0; if (row == 1) return 4.0; if (row == 3) return 1.0; }
    if (cid == 6) { if (row == 0 || row == 2 || row == 4) return 7.0; if (row == 1) return 4.0; if (row == 3) return 5.0; }
    if (cid == 7) { if (row == 0) return 7.0; if (row == 1) return 1.0; if (row == 2) return 2.0; if (row == 3) return 2.0; if (row == 4) return 2.0; }
    if (cid == 8) { if (row == 0 || row == 2 || row == 4) return 7.0; if (row == 1 || row == 3) return 5.0; }
    if (cid == 9) { if (row == 0 || row == 2 || row == 4) return 7.0; if (row == 1) return 5.0; if (row == 3) return 1.0; }
    if (cid == 10) { if (row == 4) return 2.0; }         // .
    if (cid == 11) { if (row == 3) return 2.0; if (row == 4) return 4.0; } // ,
    if (cid == 12) { if (row == 0 || row == 4) return 2.0; if (row == 1 || row == 2 || row == 3) return 4.0; } // (
    if (cid == 13) { if (row == 0 || row == 4) return 2.0; if (row == 1 || row == 2 || row == 3) return 1.0; } // )
    if (cid == 14) { if (row == 2) return 7.0; }         // -
    return 0.0;                                           // blank/unsupported
}

float glyphMask(vec2 uv, vec2 origin, vec2 size, int cid)
{
    if (cid == 15)
        return 0.0;

    vec2 p = (uv - origin) / size;
    if (p.x < 0.0 || p.y < 0.0 || p.x >= 1.0 || p.y >= 1.0)
        return 0.0;

    // inner padding for a bit of spacing inside each glyph box
    p = p * 0.82 + vec2(0.09, 0.09);
    p = clamp(p, vec2(0.0), vec2(0.999));

    int cellX = int(floor(p.x * 3.0));
    int cellY = int(floor(p.y * 5.0));

    float rowBits = glyphRowBits(cid, cellY);
    float shifted = floor(rowBits / pow(2.0, float(2 - cellX)));
    return mod(shifted, 2.0);
}

float drawChar(vec2 uv, vec2 origin, float charH, int cid)
{
    float aspect = RENDERSIZE.y / max(RENDERSIZE.x, 1.0);
    vec2 charSize = vec2(charH * aspect * 0.72, charH);
    return glyphMask(uv, origin, charSize, cid);
}

float drawFixedFloat1(vec2 uv, vec2 origin, float charH, float value)
{
    float aspect = RENDERSIZE.y / max(RENDERSIZE.x, 1.0);
    float charW = charH * aspect * 0.72;
    float advance = charW * 0.92;

    float v = clamp(abs(value), 0.0, 9999.9);
    bool neg = value < 0.0;
    bool ge1000 = v >= 1000.0;
    bool ge100 = v >= 100.0;
    bool ge10 = v >= 10.0;

    int d1000 = int(mod(floor(v / 1000.0), 10.0));
    int d100 = int(mod(floor(v / 100.0), 10.0));
    int d10 = int(mod(floor(v / 10.0), 10.0));
    int d1 = int(mod(floor(v), 10.0));
    int dDec = int(mod(floor(v * 10.0), 10.0));

    float m = 0.0;
    m = max(m, drawChar(uv, origin + vec2(advance * 0.0, 0.0), charH, neg ? 14 : 15));
    m = max(m, drawChar(uv, origin + vec2(advance * 1.0, 0.0), charH, ge1000 ? d1000 : 15));
    m = max(m, drawChar(uv, origin + vec2(advance * 2.0, 0.0), charH, (ge1000 || ge100) ? d100 : 15));
    m = max(m, drawChar(uv, origin + vec2(advance * 3.0, 0.0), charH, (ge1000 || ge100 || ge10) ? d10 : 15));
    m = max(m, drawChar(uv, origin + vec2(advance * 4.0, 0.0), charH, d1));
    m = max(m, drawChar(uv, origin + vec2(advance * 5.0, 0.0), charH, 10));
    m = max(m, drawChar(uv, origin + vec2(advance * 6.0, 0.0), charH, dDec));

    return m;
}

float drawTuple2(vec2 uv, vec2 origin, float charH, vec2 values)
{
    float aspect = RENDERSIZE.y / max(RENDERSIZE.x, 1.0);
    float charW = charH * aspect * 0.72;
    float advance = charW * 0.92;

    float m = 0.0;
    m = max(m, drawChar(uv, origin + vec2(advance * 0.0, 0.0), charH, 12));
    m = max(m, drawFixedFloat1(uv, origin + vec2(advance * 1.0, 0.0), charH, values.x));
    m = max(m, drawChar(uv, origin + vec2(advance * 8.0, 0.0), charH, 11));
    m = max(m, drawFixedFloat1(uv, origin + vec2(advance * 9.0, 0.0), charH, values.y));
    m = max(m, drawChar(uv, origin + vec2(advance * 16.0, 0.0), charH, 13));
    return m;
}

float drawDistanceValue(vec2 uv, vec2 center, float charH, float value)
{
    float aspect = RENDERSIZE.y / max(RENDERSIZE.x, 1.0);
    float charW = charH * aspect * 0.72;
    float advance = charW * 0.92;
    float totalW = advance * 7.0;
    vec2 origin = center - vec2(totalW * 0.5, charH * 0.5);
    return drawFixedFloat1(uv, origin, charH, value);
}

void main()
{
    vec2 uv = isf_FragNormCoord;
    float t = TIME * speed;
    float feather = max(softness, 0.00001);
    float count = floor(clamp(nodeCount, 1.0, float(MAX_POINTS)) + 0.0001);

    float lines = 0.0;
    float nodeLabels = 0.0;
    float distanceLabels = 0.0;

    for (int i = 0; i < MAX_POINTS; ++i)
    {
        if (float(i) >= count)
            continue;

        vec2 p1 = pointPosition(i, t);
        vec2 valueA = p1 * RENDERSIZE;
        if (valueMode == 1)
            valueA = (p1 - 0.5) * RENDERSIZE;

        vec2 labelOrigin = p1 + vec2(0.010, 0.010);
        nodeLabels = max(nodeLabels, drawTuple2(uv, labelOrigin, textSize, valueA));

        for (int j = i + 1; j < MAX_POINTS; ++j)
        {
            if (float(j) >= count)
                continue;

            vec2 p2 = pointPosition(j, t);
            float dNorm = distance(p1, p2);

            if (dNorm <= linkDistance)
            {
                float proximity = 1.0 - smoothstep(0.0, linkDistance, dNorm);
                float lm = lineMask(uv, p1, p2, lineWidth, feather);
                lines = max(lines, lm * proximity);

                vec2 mid = mix(p1, p2, 0.5) + vec2(0.0, -textSize * 0.7);
                float dPixels = distance(p1 * RENDERSIZE, p2 * RENDERSIZE);
                distanceLabels = max(distanceLabels, drawDistanceValue(uv, mid, textSize * 0.85, dPixels));
            }
        }
    }

    float alpha = max(max(lines, nodeLabels), distanceLabels) * opacity;
    gl_FragColor = vec4(1.0, 1.0, 1.0, alpha);
}
