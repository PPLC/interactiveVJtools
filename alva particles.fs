/*
{
  "ISFVSN": "2",
  "DESCRIPTION": "Dense animated micro-particle field with added Zoom and XYZ rotation controls.",
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
      "NAME": "Zoom",
      "TYPE": "float",
      "DEFAULT": 1.0,
      "MIN": 0.25,
      "MAX": 6.0
    },
    {
      "NAME": "RotX",
      "TYPE": "float",
      "DEFAULT": 0.0,
      "MIN": -180.0,
      "MAX": 180.0
    },
    {
      "NAME": "RotY",
      "TYPE": "float",
      "DEFAULT": 0.0,
      "MIN": -180.0,
      "MAX": 180.0
    },
    {
      "NAME": "RotZ",
      "TYPE": "float",
      "DEFAULT": 0.0,
      "MIN": -180.0,
      "MAX": 180.0
    },
    {
      "NAME": "Density",
      "TYPE": "float",
      "DEFAULT": 90.0,
      "MIN": 10.0,
      "MAX": 130.0
    },
    {
      "NAME": "DepthSpread",
      "TYPE": "float",
      "DEFAULT": 1.0,
      "MIN": 0.3,
      "MAX": 2.5
    }
  ]
}
*/


#define PI 3.14159265359
float h2(vec2 a){return fract(sin(dot(a,vec2(127.1,311.7)))*43758.5453);}
float ns(vec2 p){vec2 i=floor(p), f=fract(p);f=f*f*(3.0-2.0*f);return mix(mix(h2(i),h2(i+vec2(1.,0.)),f.x),mix(h2(i+vec2(0.,1.)),h2(i+1.),f.x),f.y);}
vec2 rot2(vec2 p,float a){return mat2(cos(a),-sin(a),sin(a),cos(a))*p;}
float seg(vec2 p,vec2 a,vec2 b){vec2 v=b-a;return length(p-a-v*clamp(dot(p-a,v)/max(dot(v,v),0.000001),0.,1.));}
float sdBox(vec2 p,vec2 b){vec2 d=abs(p)-b; return length(max(d,0.))+min(max(d.x,d.y),0.);}
float stroke(float d,float w){return 1.0-smoothstep(w,w+1.5/RENDERSIZE.y,abs(d));}
float fillShape(float d){return 1.0-smoothstep(0.,1.3/RENDERSIZE.y,d);}
float audioLevel(){float s=IMG_NORM_PIXEL(audioFFT,vec2(0.055,0.5)).r;return clamp(mix(ManualAudio,s,float(UseFFT)),0.,2.);}
vec3 rotX3(vec3 p,float a){float s=sin(a),c=cos(a); return vec3(p.x,c*p.y-s*p.z,s*p.y+c*p.z);} 
vec3 rotY3(vec3 p,float a){float s=sin(a),c=cos(a); return vec3(c*p.x+s*p.z,p.y,-s*p.x+c*p.z);} 
vec3 rotZ3(vec3 p,float a){float s=sin(a),c=cos(a); return vec3(c*p.x-s*p.y,s*p.x+c*p.y,p.z);} 
vec3 applyCamera(vec3 p){p=rotX3(p,radians(RotX)); p=rotY3(p,radians(RotY)); p=rotZ3(p,radians(RotZ)); return p;}
vec2 project3(vec3 p){p=applyCamera(p); float camDist=4.8/max(Zoom,0.25); float z=max(0.18,camDist-p.z); return p.xy*camDist/z;}

void main(){
 vec2 p=(gl_FragCoord.xy-.5*RENDERSIZE.xy)/RENDERSIZE.y;
 p /= Scale;
 float t=TIME*Speed;
 float a=audioLevel();
 float w=Thickness;
 vec3 col=vec3(0.);
 float alpha=0.;

 float density=clamp(Density,10.,130.);
 for(int i=0;i<220;i++){
   float fi=float(i);
   vec3 pos;
   pos.x=(h2(vec2(fi,1.7))*2.-1.)*1.2;
   pos.y=(h2(vec2(fi,9.1))*2.-1.)*0.85;
   pos.z=(h2(vec2(fi,13.4))*2.-1.)*DepthSpread;
   pos += 0.08*vec3(sin(t*1.8+fi*0.17),cos(t*1.4+fi*0.21),sin(t*1.2+fi*0.11))*(1.+a*AudioReaction);
   vec2 cur=project3(pos);
   float sz = w*(0.7+1.8*h2(vec2(fi,3.3)))*(1.0+0.15*density/90.0);
   float v=fillShape(length(p-cur)-sz);
   col += v*(0.25+0.75*h2(vec2(fi,7.1))); alpha=max(alpha,v);
 }

 gl_FragColor=vec4(clamp(col*Tint.rgb,0.,1.),clamp(alpha*Tint.a,0.,1.));
}
