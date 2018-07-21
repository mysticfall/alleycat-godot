shader_type spatial;
render_mode unshaded, blend_add;

// Adapted from 'Fire' by Xavier Benech, which is available under CC 3.0 BY NC SA:
// https://www.shadertoy.com/view/XsXSWS

uniform float strength : hint_range(0, 1) = 0.5;
uniform float flicker : hint_range(0, 1) = 0.5;
uniform float opacity : hint_range(0, 1) = 1.0;

// The 'canonical' one-liner for getting a random value.
// See https://stackoverflow.com/questions/12964279/whats-the-origin-of-this-glsl-rand-one-liner
float random(vec2 co){
	return fract(sin(dot(co ,vec2(12.9898, 78.233))) * 43758.5453);
}

float noise(in vec2 _st) {
	vec2 i = floor(_st);
	vec2 f = fract(_st);

	// Four corners in 2D of a tile
	float a = random(i);
	float b = random(i + vec2(1.0, 0.0));
	float c = random(i + vec2(0.0, 1.0));
	float d = random(i + vec2(1.0, 1.0));

	vec2 u = f * f * (3.0 - 2.0 * f);

	return mix(a, b, u.x) + (c - a)* u.y * (1.0 - u.x) + (d - b) * u.x * u.y;
}

float fbm(in vec2 st) {
	int OCTAVES = 5;

	float value = 0.0;
	float amplitude = 0.5;
	float frequency = 0.0;

	for(int i = 0; i < OCTAVES; i++) {
		value += amplitude * noise(st);
		st *= 2.0;
		amplitude *= 0.5;
	}

	return value;
}

void fragment() {
	vec2 uv = vec2(UV.x, (0.85 - UV.y) * 0.5);
	vec2 q = vec2(mod(uv.x, 1.0) - 0.5, uv.y * 2.0 - 0.18);

	float seed = (WORLD_MATRIX * vec4(0.0, 0.0, 0.0, 1.0)).x;
	float time = TIME + 3.0 * seed;
	float amount = strength * 10.0;
	float width = 0.25;

	float n = fbm(amount * q - vec2(0, flicker * 3.0 * time));
	float c = 1.0 - 16.0 * pow(max(0.0, length(q * vec2(1.8 + q.y * 1.5, 1.0)) - n * max(0.0, q.y + width)), 1.2);

	float c1 = n * c * (1.5 - pow(2.50 * uv.y, 4));

	c1 = clamp(c1, 0.0, 1.0);

	ALBEDO = vec3(1.5 * c1, 1.5 * pow(c1, 2), pow(c1, 6));
	ALPHA = c * (1.0 - pow(uv.y, 3)) * opacity * (1.0 - smoothstep(0.9, 1.0, 1.0 - UV.y));
}