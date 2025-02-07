shader_type canvas_item;

void fragment() {
    vec4 tex_color = texture(TEXTURE, UV);
    // Change black (and close to black) to the desired color
    if (tex_color.rgb == vec3(0.0)) {
        tex_color.rgb = vec3(1.0, 1.0, 1.0); // Red color
    }
    COLOR = tex_color;
}
