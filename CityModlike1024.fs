/*
{
  "ISFVSN": "2",
  "DESCRIPTION": "Procedural 3D-style city field with added Zoom and XYZ rotation controls.",
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
      "NAME": "CityDepth",
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

 for(int row=0;row<6;row++){
   float rr=float(row);
   float z=-0.3 + rr*0.28*CityDepth;
   for(int i=0;i<14;i++){
     float fi=float(i);
     float x=-1.0+fi*0.16;
     float rnd=h2(vec2(fi,rr));
     float h=(0.15+0.55*rnd)*(1.0+0.5*a*AudioReaction);
     float hw=0.05+0.03*fract(fi*0.37+rr*0.21);
     vec3 c=vec3(x,-0.6+h*0.5,z + 0.08*sin(t*0.6+fi*0.2));
     vec3 p1=c+vec3(-hw,-h*0.5,-hw);
     vec3 p2=c+vec3(hw,-h*0.5,-hw);
     vec3 p3=c+vec3(hw,h*0.5,-hw);
     vec3 p4=c+vec3(-hw,h*0.5,-hw);
     vec3 p5=c+vec3(-hw,-h*0.5,hw);
     vec3 p6=c+vec3(hw,-h*0.5,hw);
     vec3 p7=c+vec3(hw,h*0.5,hw);
     vec3 p8=c+vec3(-hw,h*0.5,hw);
     vec2 a1=project3(p1),a2=project3(p2),a3=project3(p3),a4=project3(p4),a5=project3(p5),a6=project3(p6),a7=project3(p7),a8=project3(p8);
     float v=0.;
     v=max(v,stroke(seg(p,a1,a2),w)); v=max(v,stroke(seg(p,a2,a3),w)); v=max(v,stroke(seg(p,a3,a4),w)); v=max(v,stroke(seg(p,a4,a1),w));
     v=max(v,stroke(seg(p,a5,a6),w*0.85)); v=max(v,stroke(seg(p,a6,a7),w*0.85)); v=max(v,stroke(seg(p,a7,a8),w*0.85)); v=max(v,stroke(seg(p,a8,a5),w*0.85));
     v=max(v,stroke(seg(p,a1,a5),w*0.75)); v=max(v,stroke(seg(p,a2,a6),w*0.75)); v=max(v,stroke(seg(p,a3,a7),w*0.75)); v=max(v,stroke(seg(p,a4,a8),w*0.75));
     col+=v*(0.22+0.1*rr+0.25*rnd); alpha=max(alpha,v);
   }
 }

 gl_FragColor=vec4(clamp(col*Tint.rgb,0.,1.),clamp(alpha*Tint.a,0.,1.));
}
