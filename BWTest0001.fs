/*
{
  "ISFVSN": "2",
  "DESCRIPTION": "Reuses the two PNG source images embedded in QC; reproduces the rotating/accumulating image concept as a timed 2D composite, without exact Core Image feedback.",
  "CATEGORIES": [
    "Generator"
  ],
  "INPUTS": [
    {
      "NAME": "Speed",
      "TYPE": "float",
      "DEFAULT": 0.65,
      "MIN": 0.0,
      "MAX": 4.0
    },
    {
      "NAME": "Scale",
      "TYPE": "float",
      "DEFAULT": 1.0,
      "MIN": 0.2,
      "MAX": 3.0
    },
    {
      "NAME": "Thickness",
      "TYPE": "float",
      "DEFAULT": 0.004,
      "MIN": 0.0005,
      "MAX": 0.03
    },
    {
      "NAME": "ManualAudio",
      "TYPE": "float",
      "DEFAULT": 0.3,
      "MIN": 0.0,
      "MAX": 1.0
    },
    {
      "NAME": "AudioReaction",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": 0.0,
      "MAX": 3.0
    },
    {
      "NAME": "Tint",
      "TYPE": "color",
      "DEFAULT": [
        1.0,
        1.0,
        1.0,
        1.0
      ]
    },
    {
      "NAME": "UseFFT",
      "TYPE": "bool",
      "DEFAULT": false
    },
    {
      "NAME": "audioFFT",
      "TYPE": "audioFFT"
    },
    {
      "NAME": "BWAmount",
      "TYPE": "float",
      "DEFAULT": 1.0,
      "MIN": 0.0,
      "MAX": 1.0
    },
    {
      "NAME": "MaskAmount",
      "TYPE": "float",
      "DEFAULT": 0.65,
      "MIN": 0.0,
      "MAX": 1.0
    }
  ],
  "IMPORTED": [
    {
      "NAME": "colorWheel",
      "PATH": "qtz_bw_asset_0.png"
    },
    {
      "NAME": "bwMask",
      "PATH": "qtz_bw_asset_1.png"
    }
  ]
}
*/

#define PI 3.14159265359
float h1(float a){return fract(sin(a*127.17)*43758.5453);}
float h2(vec2 a){return fract(sin(dot(a,vec2(127.1,311.7)))*43758.5453);}
float ns(vec2 p){vec2 i=floor(p), f=fract(p);f=f*f*(3.0-2.0*f);return mix(mix(h2(i),h2(i+vec2(1.,0.)),f.x),mix(h2(i+vec2(0.,1.)),h2(i+1.),f.x),f.y);}
vec2 rot(vec2 p,float a){return mat2(cos(a),-sin(a),sin(a),cos(a))*p;}
float seg(vec2 p,vec2 a,vec2 b){vec2 v=b-a;return length(p-a-v*clamp(dot(p-a,v)/max(dot(v,v),0.000001),0.,1.));}
float sdBox(vec2 p,vec2 b){vec2 d=abs(p)-b; return length(max(d,0.))+min(max(d.x,d.y),0.);}
float stroke(float d,float w){return 1.0-smoothstep(w,w+1.5/RENDERSIZE.y,abs(d));}
float fillShape(float d){return 1.0-smoothstep(0.,1.3/RENDERSIZE.y,d);}
float audioLevel(){float s=IMG_NORM_PIXEL(audioFFT,vec2(0.055,0.5)).r;return clamp(mix(ManualAudio,s,float(UseFFT)),0.,2.);}
vec2 project(vec3 v){float z=max(0.4,3.4-v.z);return v.xy*2.0/z;}

void main(){

 vec2 p=(gl_FragCoord.xy-.5*RENDERSIZE.xy)/RENDERSIZE.y;
 p /= Scale;
 float t=TIME*Speed;
 float a=audioLevel();
 float w=Thickness;
 vec3 col=vec3(0.);
 float alpha=0.;
 
 vec2 uv=gl_FragCoord.xy/RENDERSIZE;
 vec2 v=(uv-0.5)*vec2(RENDERSIZE.x/RENDERSIZE.y,1.);
 vec2 uv1=rot(v,-t*.52)/(1.0+0.2*sin(t))*.85+0.5;
 vec2 uv2=rot(v,t*.16)/.9+.5;
 vec4 src=IMG_NORM_PIXEL(colorWheel,uv1);
 vec4 mask=IMG_NORM_PIXEL(bwMask,uv2);
 vec3 mixedC=mix(src.rgb,vec3(dot(src.rgb,vec3(.299,.587,.114))),BWAmount);
 col=mix(mixedC,mask.rgb,MaskAmount)*(1.+a*AudioReaction*.2);
 alpha=1.;

 gl_FragColor=vec4(clamp(col*Tint.rgb,0.,1.),clamp(alpha*Tint.a,0.,1.));
}
