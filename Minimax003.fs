/*
{
  "ISFVSN": "2",
  "DESCRIPTION": "Ten spline-interpolated moving control points reconstructed as animated line segments and point markers.",
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
 
 vec2 last=vec2(0.);
 for(int i=0;i<100;i++){
  float u=float(i)/99.*9.; float n=floor(u); float f=fract(u);
  vec2 a0=vec2(-.43+(n-1.)*.097,.18*sin(t*1.2+(n-1.)*.8));
  vec2 a1=vec2(-.43+n*.097,.18*sin(t*1.2+n*.8));
  vec2 a2=vec2(-.43+(n+1.)*.097,.18*sin(t*1.2+(n+1.)*.8));
  vec2 a3=vec2(-.43+(n+2.)*.097,.18*sin(t*1.2+(n+2.)*.8));
  vec2 point=.5*((2.*a1)+(-a0+a2)*f+(2.*a0-5.*a1+4.*a2-a3)*f*f+(-a0+3.*a1-3.*a2+a3)*f*f*f);
  point.y*=1.+AudioReaction*a;
  if(i>0){float v=stroke(seg(p,last,point),w);col+=v*.24;alpha=max(alpha,v);}
  if(mod(float(i),11.)<1.){float d=length(p-point);float v=fillShape(d-w*2.2);col+=v*.15;alpha=max(alpha,v);}
  last=point;
 }

 gl_FragColor=vec4(clamp(col*Tint.rgb,0.,1.),clamp(alpha*Tint.a,0.,1.));
}
