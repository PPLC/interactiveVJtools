/*
{
  "ISFVSN": "2",
  "DESCRIPTION": "TorusTriplesVincio — tori (Quartz Composer reconstruction, not pixel-identical)",
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
      "NAME": "Density",
      "TYPE": "float",
      "DEFAULT": 0.65,
      "MIN": 0.0,
      "MAX": 1.0
    },
    {
      "NAME": "Frequency",
      "TYPE": "float",
      "DEFAULT": 3.0,
      "MIN": 0.2,
      "MAX": 20.0
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
      "NAME": "AudioLevel",
      "TYPE": "float",
      "DEFAULT": 0.35,
      "MIN": 0.0,
      "MAX": 1.0
    },
    {
      "NAME": "AudioReaction",
      "TYPE": "float",
      "DEFAULT": 0.8,
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
  ],
  "CREDIT": "Reconstructed from user-supplied QC composition; patch plugins not bundled"
}
*/
#define SEED 18.904110


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
float lineMask(float d,float w){float e=1.2/RENDERSIZE.y;return 1.-smoothstep(w,w+e,max(0.,d));}
float discMask(float d,float r){return 1.-smoothstep(r,r+1.2/RENDERSIZE.y,d);}
float valnoise(vec2 x){vec2 i=floor(x),f=fract(x);f=f*f*(3.-2.*f);return mix(mix(hash21(i),hash21(i+vec2(1.,0.)),f.x),mix(hash21(i+vec2(0.,1.)),hash21(i+1.),f.x),f.y);}
float fnoise(vec2 p){return .57*valnoise(p)+.29*valnoise(p*2.11)+.14*valnoise(p*4.15);}
float getAudio(){float f=IMG_NORM_PIXEL(audioFFT,vec2(.04,.5)).r;return clamp(UseFFT ? f*2.0 : AudioLevel,0.,1.);}
float activity(){return 0.25+getAudio()*AudioReaction;}
vec3 palette(float v){return .42+.58*cos(TAU*(vec3(.0,.18,.4)+v*.1+SEED*.024));}

void main(){
  vec2 p=(gl_FragCoord.xy-0.5*RENDERSIZE.xy)/RENDERSIZE.y;
  float t=TIME*Speed;
  vec3 col=vec3(0.);float alpha=0.;

 for(int j=0;j<5;j++){if(float(j)>=mix(2.,5.,Quality))break;
  float fj=float(j),tub=.06+.012*fj;
  for(int k=0;k<8;k++){if(float(k)>=mix(3.,8.,Quality))break;
   float v0=TAU*float(k)/8.,v1=TAU*float(k+1)/8.;
   vec2 last=vec2(0.);
   int samples=int(mix(22.,75.,Quality));for(int i=0;i<75;i++){if(i>=samples)break;
    float u=TAU*float(i)/float(samples-1);
    vec3 pos=vec3((.45+tub*cos(v0))*cos(u),(.45+tub*cos(v0))*sin(u),tub*sin(v0));
    pos+=vec3(0.,0.,(fj-2.)*.19);
    pos=rotX(pos,t*.17+fj*.13);
    vec2 cur=project3(pos);
    if(i>0){float v=lineMask(segmentDist(p,last,cur),Thickness*.82);col+=v*palette((fj+float(k)*.15)*.15)*.16;alpha=max(alpha,v);} last=cur;
   }
  }
 }

  col=mix(vec3(dot(col,vec3(.299,.587,.114))),col,ColorMix)*Brightness;
  gl_FragColor=vec4(clamp(col*Tint.rgb,0.,1.),clamp(alpha*Tint.a,0.,1.));
}
