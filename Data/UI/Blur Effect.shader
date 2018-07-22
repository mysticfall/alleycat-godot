shader_type canvas_item;
render_mode unshaded, blend_mix;

uniform float lod : hint_range(0, 5) = 2.0;

uniform float brightness : hint_range(0, 1) = 0.8;

void fragment() {
	vec4 color = textureLod(SCREEN_TEXTURE, SCREEN_UV, lod);

	COLOR = vec4(color.xyz * brightness, color.a);
}
