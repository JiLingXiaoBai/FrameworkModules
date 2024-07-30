// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/SpriteFontShaderHLSL"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" {}
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
        LOD 200

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {


            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            


            v2f vert(appdata v)
            {
                v2f o;
                // float4 worldPos = mul(unity_ObjectToWorld, float4(0,0,0,1));
                // // 再获取cam的世界坐标下的Right,Up，通过ViewMatrix的逆矩阵的第一行与第二行
                // float3 camRight = UNITY_MATRIX_IT_MV[0].xyz;
                // float3 camUp = UNITY_MATRIX_IT_MV[1].xyz;
                // worldPos.xyz += // 使用原始顶点的大小与缩放系数：_BillboardSize来控制世界坐标下的顶点位置
                // camRight * v.vertex.x + 
                // camUp * v.vertex.y;
                // // 最后将按照camRight,camUp重新调整后的worldPos，再转换到clipSpace，片段需要
                // o.vertex = mul(UNITY_MATRIX_VP, worldPos);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                return col * i.color; // Multiply effect
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}