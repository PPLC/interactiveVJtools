/*
{
  "ISFVSN": "2",
  "DESCRIPTION": "Procedural moving triangle mesh inspired by the QC noise-structure and Kineme triangle renderer.",
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
 
 for(int iy=0;iy<8;iy++){
  for(int ix=0;ix<10;ix++){
   vec2 cell=vec2(float(ix),float(iy));
   vec2 base=vec2((float(ix)-5.)*.103,(float(iy)-4.)*.125);
   vec2 v0=base+vec2(0.,.055);
   vec2 v1=base+vec2(-.05,-.055);
   vec2 v2=base+vec2(.05,-.055);
   v0+=.029*(vec2(ns(cell+t*.4),ns(cell*1.4+t*.4))-.5)*(1.+a*AudioReaction);
   v1+=.029*(vec2(ns(cell*1.7+t*.3),ns(cell*1.8+t*.3))-.5);
   v2+=.029*(vec2(ns(cell*1.1+t*.2),ns(cell*2.1+t*.2))-.5);
   float val=max(stroke(seg(p,v0,v1),w),max(stroke(seg(p,v1,v2),w),stroke(seg(p,v2,v0),w)));
   col+=val*.34;alpha=max(alpha,val);
  }
 }

 gl_FragColor=vec4(clamp(col*Tint.rgb,0.,1.),clamp(alpha*Tint.a,0.,1.));
}
