/*{
  "DESCRIPTION": "Three independently controllable horizontal white bars for VDMX. Each bar has an X position and width control. Transparent background.",
  "CREDIT": "Created for Timo Dufner",
  "ISFVSN": "2.0",
  "CATEGORIES": ["Generator", "Utility"],
  "INPUTS": [
    {
      "NAME": "bar1Position",
      "TYPE": "float",
      "LABEL": "Bar 1 Position",
      "DEFAULT": 0.25,
      "MIN": 0.0,
      "MAX": 1.0
    },
    {
      "NAME": "bar1Width",
      "TYPE": "float",
      "LABEL": "Bar 1 Width",
      "DEFAULT": 0.25,
      "MIN": 0.0,
      "MAX": 1.0
    },
    {
      "NAME": "bar2Position",
      "TYPE": "float",
      "LABEL": "Bar 2 Position",
      "DEFAULT": 0.50,
      "MIN": 0.0,
      "MAX": 1.0
    },
    {
      "NAME": "bar2Width",
      "TYPE": "float",
      "LABEL": "Bar 2 Width",
      "DEFAULT": 0.25,
      "MIN": 0.0,
      "MAX": 1.0
    },
    {
      "NAME": "bar3Position",
      "TYPE": "float",
      "LABEL": "Bar 3 Position",
      "DEFAULT": 0.75,
      "MIN": 0.0,
      "MAX": 1.0
    },
    {
      "NAME": "bar3Width",
      "TYPE": "float",
      "LABEL": "Bar 3 Width",
      "DEFAULT": 0.25,
      "MIN": 0.0,
      "MAX": 1.0
    },
    {
      "NAME": "barHeight",
      "TYPE": "float",
      "LABEL": "Bar Height",
      "DEFAULT": 0.08,
      "MIN": 0.002,
      "MAX": 0.33
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

    float bar1 = rectangleMask(
        uv,
        vec2(bar1Position, 0.75),
        vec2(bar1Width, barHeight),
        softEdge
    );

    float bar2 = rectangleMask(
        uv,
        vec2(bar2Position, 0.50),
        vec2(bar2Width, barHeight),
        softEdge
    );

    float bar3 = rectangleMask(
        uv,
        vec2(bar3Position, 0.25),
        vec2(bar3Width, barHeight),
        softEdge
    );

    float alpha = max(bar1, max(bar2, bar3)) * opacity;
    gl_FragColor = vec4(1.0, 1.0, 1.0, alpha);
}
