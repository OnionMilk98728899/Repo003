shader_type canvas_item;

uniform sampler2D palette_texture : filter_nearest, repeat_disable;
uniform int palette_size = 8;
//uniform bool enabled = true;

void fragment()
{
    vec4 original_color = texture(TEXTURE, UV);

    if (abs(original_color.a - original_color.b) < 0.0001)
    {
        COLOR = original_color;
    }

    //if (!enabled)
    //{
        //COLOR = vec4(1.0, 0.0, 0.0, 1.0);
    //}

    float best_distance = 999999.0;
    vec3 best_color = original_color.rgb;

    for (int i = 0; i < palette_size; i++)
    {
        vec2 palette_uv = vec2(
            0.5,
            (float(i) + 0.5) / float(palette_size)
        );

        vec3 palette_color = texture(
            palette_texture,
            palette_uv
        ).rgb;

        float color_distance = distance(
            original_color.rgb,
            palette_color
        );

        if (color_distance < best_distance)
        {
            best_distance = color_distance;
            best_color = palette_color;
        }
    }
	
    COLOR = vec4(best_color, original_color.a);
}