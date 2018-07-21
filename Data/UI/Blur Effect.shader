shader_type canvas_item;
render_mode unshaded, blend_mix;

uniform float lod : hint_range(0, 5) = 1.5;

void fragment() {
	COLOR = textureLod(SCREEN_TEXTURE, SCREEN_UV, lod);
}
