shader_type spatial;
render_mode blend_mix, depth_draw_opaque, cull_disabled, diffuse_burley, specular_schlick_ggx;

uniform vec4 albedo : hint_color = vec4(1, 1, 1, 1);
uniform sampler2D albedo_texture : hint_albedo;

uniform float roughness : hint_range(0, 1) = 1f;
uniform float specular : hint_range(0, 1) = 0f;
uniform float metallic : hint_range(0, 1) = 0f;

uniform float normal : hint_range(-5, 5) = 0f;
uniform sampler2D normal_texture : hint_normal;

uniform float alpha_threshold : hint_range(0, 1) = 0.5f;

float is_dithered(vec4 frag, float alpha) {
    vec2 pos = frag.xy * frag.xy;

    int index = (int(pos.x) % 2) * 2 + int(pos.y) % 2;

    return alpha - float(index + 1) / 5.0;
}

void fragment() {
	vec2 base_uv = UV;
	vec4 albedo_tex = texture(albedo_texture, base_uv);

    if (is_dithered(FRAGCOORD, albedo_tex.a) < alpha_threshold) {
        discard;
    } else {
	    ALBEDO = albedo.rgb * albedo_tex.rgb;
        ALPHA = 0.8;
    }

    ROUGHNESS = roughness;
    METALLIC = metallic;
    SPECULAR = specular;

	NORMALMAP = texture(normal_texture, base_uv).rgb;
	NORMALMAP_DEPTH = normal;
}
