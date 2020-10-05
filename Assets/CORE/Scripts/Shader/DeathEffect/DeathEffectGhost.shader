Shader "Custom/DeathEffectGhost"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _UVDist ("UVDistord Texture", 2D) = "white" {}
        _Speed ("Speed", Range(0,1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha
        Zwrite Off
        Cull Off
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                float2 uvdist : TEXCOORD1;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 uvdist : TEXCOORD1;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            sampler2D _MainTex, _UVDist;
            float4 _MainTex_ST, _UVDist_ST;
            float _Speed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uvdist = TRANSFORM_TEX(v.uvdist, _UVDist);
                o.color = v.color;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed2 disset = float2(i.uvdist.x, i.uvdist.y+(_Time.y * _Speed));
                fixed4 distortion = tex2D(_UVDist, disset);

                fixed2 colset = float2(distortion.x, i.uv.y);
                fixed4 col = tex2D(_MainTex, colset);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col * i.color;
            }
            ENDCG
        }
    }
}
