Shader "Custom/TransparencyCell" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_TransparnecySharpness("Transparency Sharpness", Range(0.1,1.0)) = 0.5
		_AbsorptionFactor("Absorption Factor", Range(0.0, 10.0)) = 0.5
		_AbsorptionSharpness("Absorption Sharpness", Range(0.0, 3.0)) = 0.5
		_AbsorptionOffset("Absorption Offset", Range(-3.14, 3.14)) = 0
		_EdgeColor("Color", Color) = (1,1,1,1)
	}
	SubShader {
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }

		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows alpha

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float3 viewDir;
		};

		half _Glossiness;
		half _Metallic;
		half _TransparnecySharpness;
		half _AbsorptionSharpness;
		half _AbsorptionFactor;
		half _AbsorptionOffset;

		fixed4 _EdgeColor;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;

			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			
			half angle = saturate(abs(dot(normalize(IN.viewDir), o.Normal)));
			o.Alpha = c.a * (1 - pow(angle, _TransparnecySharpness));
			o.Albedo = saturate(lerp(
				c.rgb, 
				_EdgeColor.rgb, 
				saturate(
					_AbsorptionFactor * pow(sin(angle + _AbsorptionOffset), _AbsorptionSharpness))));

		}
		ENDCG
	}
	FallBack "Diffuse"
}
