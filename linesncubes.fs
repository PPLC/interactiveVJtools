/*
{
  "ISFVSN": "2.0",
  "DESCRIPTION": "linesncubes — individually reviewed and refined ISF shader. Original QTZ graph not included in this archive.",
  "CATEGORIES": [
    "Generator"
  ],
  "INPUTS": [
    {
      "NAME": "Speed",
      "TYPE": "float",
      "DEFAULT": 0.7,
      "MIN": -3.0,
      "MAX": 3.0
    },
    {
      "NAME": "Thickness",
      "TYPE": "float",
      "DEFAULT": 0.0025,
      "MIN": 0.0005,
      "MAX": 0.02
    },
    {
      "NAME": "Spread",
      "TYPE": "float",
      "DEFAULT": 1.0,
      "MIN": 0.2,
      "MAX": 3.0
    },
    {
      "NAME": "Zoom",
      "TYPE": "float",
      "DEFAULT": 1.0,
      "MIN": 0.25,
      "MAX": 5.0
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
      "NAME": "Quality",
      "TYPE": "float",
      "DEFAULT": 0.55,
      "MIN": 0.15,
      "MAX": 1.0
    },
    {
      "NAME": "Brightness",
      "TYPE": "float",
      "DEFAULT": 1.8,
      "MIN": 0.0,
      "MAX": 6.0
    },
    {
      "NAME": "ColorMix",
      "TYPE": "float",
      "DEFAULT": 0.28,
      "MIN": 0.0,
      "MAX": 1.0
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
    }
  ],
  "CREDIT": "Based on user-supplied ISF file; individually reviewed and refined for VDMX. Not a verification against original Quartz Composer graph."
}
*/

#define SEED 3.438356


#define PI 3.141592653589793
#define TAU 6.283185307179586
float hash11(float a){return fract(sin(a*127.17+SEED)*43758.5453);}
float hash21(vec2 a){return fract(sin(dot(a,vec2(127.1,311.7))+SEED)*43758.5453);}
vec2 hash22(vec2 a){return vec2(hash21(a),hash21(a+vec2(17.71,92.13)));}
vec2 rot(vec2 p,float a){float c=cos(a),s=sin(a);return mat2(c,-s,s,c)*p;}
vec3 rotX(vec3 p,float a){float c=cos(a),s=sin(a);return vec3(p.x,c*p.y-s*p.z,s*p.y+c*p.z);}
vec3 rotY(vec3 p,float a){float c=cos(a),s=sin(a);return vec3(c*p.x+s*p.z,p.y,-s*p.x+c*p.z);}
vec3 rotZ(vec3 p,float a){float c=cos(a),s=sin(a);return vec3(c*p.x-s*p.y,s*p.x+c*p.y,p.z);}
vec3 rotateXYZ(vec3 v){return rotZ(rotY(rotX(v,radians(RotX)),radians(RotY)),radians(RotZ));}
vec2 project3(vec3 v){v=rotateXYZ(v);return v.xy * (2.65 * Zoom/max(0.35,4.25-v.z));}
float segmentDist(vec2 p,vec2 a,vec2 b){vec2 q=b-a;return length(p-a-q*clamp(dot(p-a,q)/max(dot(q,q),0.000001),0.,1.));}
float boxDist(vec2 p,vec2 b){vec2 d=abs(p)-b;return length(max(d,0.))+min(max(d.x,d.y),0.);}
float lineMask(float d,float w){float e=1.2/RENDERSIZE.y;return 1.-smoothstep(w,w+e,abs(d));}
float discMask(float d,float r){return 1.-smoothstep(r,r+1.2/RENDERSIZE.y,d);}
float valnoise(vec2 x){vec2 i=floor(x),f=fract(x);f=f*f*(3.-2.*f);return mix(mix(hash21(i),hash21(i+vec2(1.,0.)),f.x),mix(hash21(i+vec2(0.,1.)),hash21(i+1.),f.x),f.y);}
float fnoise(vec2 p){return .57*valnoise(p)+.29*valnoise(p*2.11)+.14*valnoise(p*4.15);}
float getAudio(){return 0.;}
float activity(){return 0.25;}
vec3 palette(float v){return .42+.58*cos(TAU*(vec3(.0,.18,.4)+v*.1+SEED*.024));}



float line3D(vec2 p, vec3 a, vec3 b, float w){return lineMask(segmentDist(p,project3(a),project3(b)),w);}
float roundedUnion(float d1,float d2,float k){float h=max(k-abs(d1-d2),0.)/max(k,.0001);return min(d1,d2)-h*h*k*.25;}
float polygonSDF(vec2 p,float n,float radius){
 float a=atan(p.y,p.x)+PI; float sector=TAU/n;
 return cos(floor(.5+a/sector)*sector-a)*length(p)-radius;
}
float cubeWire(vec2 p, vec3 c, vec3 size, vec3 angles, float w){
 vec3 v0=vec3(-size.x,-size.y,-size.z),v1=vec3(size.x,-size.y,-size.z),v2=vec3(size.x,size.y,-size.z),v3=vec3(-size.x,size.y,-size.z);
 vec3 v4=vec3(-size.x,-size.y,size.z),v5=vec3(size.x,-size.y,size.z),v6=vec3(size.x,size.y,size.z),v7=vec3(-size.x,size.y,size.z);
 v0=rotZ(rotY(rotX(v0,angles.x),angles.y),angles.z)+c;
 v1=rotZ(rotY(rotX(v1,angles.x),angles.y),angles.z)+c;
 v2=rotZ(rotY(rotX(v2,angles.x),angles.y),angles.z)+c;
 v3=rotZ(rotY(rotX(v3,angles.x),angles.y),angles.z)+c;
 v4=rotZ(rotY(rotX(v4,angles.x),angles.y),angles.z)+c;
 v5=rotZ(rotY(rotX(v5,angles.x),angles.y),angles.z)+c;
 v6=rotZ(rotY(rotX(v6,angles.x),angles.y),angles.z)+c;
 v7=rotZ(rotY(rotX(v7,angles.x),angles.y),angles.z)+c;
 vec2 a=project3(v0),b=project3(v1),c0=project3(v2),d=project3(v3);
 vec2 e=project3(v4),f=project3(v5),g=project3(v6),h=project3(v7);
 float m=0.;
 m=max(m,lineMask(segmentDist(p,a,b),w));m=max(m,lineMask(segmentDist(p,b,c0),w));m=max(m,lineMask(segmentDist(p,c0,d),w));m=max(m,lineMask(segmentDist(p,d,a),w));
 m=max(m,lineMask(segmentDist(p,e,f),w));m=max(m,lineMask(segmentDist(p,f,g),w));m=max(m,lineMask(segmentDist(p,g,h),w));m=max(m,lineMask(segmentDist(p,h,e),w));
 m=max(m,lineMask(segmentDist(p,a,e),w));m=max(m,lineMask(segmentDist(p,b,f),w));m=max(m,lineMask(segmentDist(p,c0,g),w));m=max(m,lineMask(segmentDist(p,d,h),w));
 return m;
}


void main(){
vec2 p=(gl_FragCoord.xy-0.5*RENDERSIZE.xy)/RENDERSIZE.y;
float t=TIME*Speed;
vec3 col=vec3(0.);float alpha=0.;

 for(int j=0;j<9;j++){if(float(j)>=mix(3.,9.,Quality))break;
  float fj=float(j),a=fj*TAU/9.+t*.11;
  vec3 c=vec3(.62*cos(a),.43*sin(a),(.5*hash11(fj)-.25)*Spread);
  float v=cubeWire(p,c,vec3(.07),vec3(t*.35+fj*.11,t*.42,0.),Thickness);
  col+=v*palette(fj*.1)*.46;alpha=max(alpha,v);
  vec3 c2=vec3(.62*cos(a+TAU/9.),.43*sin(a+TAU/9.),(.5*hash11(mod(fj+1.,9.))-.25)*Spread);
  float lin=line3D(p,c,c2,Thickness*.85);
  col+=lin*.35;alpha=max(alpha,lin);
 }


col=mix(vec3(dot(col,vec3(.299,.587,.114))),col,ColorMix)*Brightness;
gl_FragColor=vec4(clamp(col*Tint.rgb,0.,1.),clamp(alpha*Tint.a,0.,1.));
}
