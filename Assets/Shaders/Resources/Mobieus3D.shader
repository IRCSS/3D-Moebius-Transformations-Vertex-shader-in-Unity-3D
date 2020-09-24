Shader "Unlit/Mobieus3D"
{
    Properties
    {
        _MainTex  ("Texture", 2D) = "white" {}
        
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
      //  Cull Off
        Pass
        {
            CGPROGRAM
            #pragma vertex   vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv     : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4    _MainTex_ST;
            float     _Radius;
            float     _isoclinic;
            float     _qRotation;
            float     _speed;
            float     iTime;
            float     _maxRotat;
            float     _theta;
            int       _CameraCenteric;

#define PI 3.141592653589793238

            v2f vert (appdata v)
            {
                v2f o;

                float4 pos = mul(unity_ObjectToWorld, v.vertex);
                if(_CameraCenteric != 0) pos = mul(UNITY_MATRIX_V, float4(pos.xyz, 1.));
                pos.xyz /= _Radius;

                float  wf  = 1. + pos.x*pos.x + pos.y*pos.y + pos.z*pos.z;
                
                float4x4 projectionTo3Sphere = 
                {
                    2./wf,      0.,      0.,           0.,
                       0.,   2./wf,      0.,           0., 
                       0.,      0.,   2./wf,           0.,   
                       0.,      0.,      0.,   (wf-2.)/wf
                };

                pos      = mul(projectionTo3Sphere, pos);
                
               // float theta = (sin(iTime*_speed)) * PI * _maxRotat;
                float theta = _theta;
                float cos_p = cos(_isoclinic * theta);
                float sin_p = sin(_isoclinic * theta);

                float cos_q = cos(_qRotation * theta);
                float sin_q = sin(_qRotation * theta);

                float4x4 rotate4D =
                {
                     cos_p, sin_p,   0.0,    0.0,
                    -sin_p, cos_p,   0.0,    0.0,
                       0.0,   0.0, cos_q, -sin_q,
                       0.0,   0.0, sin_q,  cos_q
                };

                pos      = mul(rotate4D, pos);



                pos.xyz /= (1. - pos.w);

                pos.xyz *= _Radius;


                if (_CameraCenteric == 0)  pos = mul(UNITY_MATRIX_V, float4(pos.xyz,1.));
                pos      = mul(UNITY_MATRIX_P, float4(pos.xyz,1.));
                o.vertex = pos;
               
                o.uv     = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}
