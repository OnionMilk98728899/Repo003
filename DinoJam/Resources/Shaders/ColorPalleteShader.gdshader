shader_type canvas_item;

uniform sampler2D palette_texture : source_color, filter_nearest, repeat_disable;
uniform int palette_size = 8;

void fragment()
{
    vec4 original_color = texture(TEXTURE, UV);

    if (original_color.a <= 0.0)
    {
        COLOR = original_color;
    }
    else
    {
        float best_distance = 999999.0;
        vec3 best_color = original_color.rgb;

        for (int i = 0; i < palette_size; i++)
        {
            vec3 palette_color = texelFetch(
                palette_texture,
                ivec2(0, i),
                0
            ).rgb;

            vec3 difference = original_color.rgb - palette_color;
            float color_distance = dot(difference, difference);

            if (color_distance < best_distance)
            {
                best_distance = color_distance;
                best_color = palette_color;
            }
        }

        COLOR = vec4(best_color, original_color.a);
    }
}
