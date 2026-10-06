/*{
  "DESCRIPTION": "Three independently controllable vertical white lines for VDMX. Each line has an X position and width control. Full screen height with transparent background.",
  "CREDIT": "Created for Timo Dufner",
  "ISFVSN": "2.0",
  "CATEGORIES": ["Generator", "Utility"],
  "INPUTS": [
    {
      "NAME": "line1Position",
      "TYPE": "float",
      "LABEL": "Line 1 Position",
      "DEFAULT": 0.25,
      "MIN": 0.0,
      "MAX": 1.0
    },
    {
      "NAME": "line1Width",
      "TYPE": "float",
      "LABEL": "Line 1 Width",
      "DEFAULT": 0.03,
      "MIN": 0.0,
      "MAX": 1.0
    },
    {
      "NAME": "line2Position",
      "TYPE": "float",
      "LABEL": "Line 2 Position",
      "DEFAULT": 0.50,
      "MIN": 0.0,
      "MAX": 1.0
    },
    {
      "NAME": "line2Width",
      "TYPE": "float",
      "LABEL": "Line 2 Width",
      "DEFAULT": 0.03,
      "MIN": 0.0,
      "MAX": 1.0
    },
    {
      "NAME": "line3Position",
      "TYPE": "float",
      "LABEL": "Line 3 Position",
      "DEFAULT": 0.75,
      "MIN": 0.0,
      "MAX": 1.0
    },
    {
      "NAME": "line3Width",
      "TYPE": "float",
      "LABEL": "Line 3 Width",
      "DEFAULT": 0.03,
      "MIN": 0.0,
      "MAX": 1.0
    },
    {
      "NAME": "softEdge",
      "TYPE": "float",
      "LABEL": "Soft Edge",
      "DEFAULT": 0.001,
      "MIN": 0.0,
      "MAX": 0.05
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

float rectangleMask(vec2 uv, vec2 center, vec2 size, float feather)
{
    vec2 delta = abs(uv - center) - (size * 0.5);
    float distanceToBox = max(delta.x, delta.y);
    float edge = max(feather, 0.00001);
    return 1.0 - smoothstep(0.0, edge, distanceToBox);
}

void main()
{
    vec2 uv = isf_FragNormCoord;

    float line1 = rectangleMask(
        uv,
        vec2(line1Position, 0.5),
        vec2(line1Width, 1.0),
        softEdge
    );

    float line2 = rectangleMask(
        uv,
        vec2(line2Position, 0.5),
        vec2(line2Width, 1.0),
        softEdge
    );

    float line3 = rectangleMask(
        uv,
        vec2(line3Position, 0.5),
        vec2(line3Width, 1.0),
        softEdge
    );

    float alpha = max(line1, max(line2, line3)) * opacity;
    gl_FragColor = vec4(1.0, 1.0, 1.0, alpha);
}
