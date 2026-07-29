using System;
using Godot;

namespace C7.UIElements;

[GlobalClass]
[Tool]
public partial class Civ3HSlider : HSlider {
	public ImageTexture grabber { get; private set; }
	public ImageTexture grabberHighlight { get; private set; }
	public StyleBox sliderStyleBox { get; private set; }

	public Civ3HSlider AddGrabber(ImageTexture texture) =>
		SetIcon("grabber", texture, t => grabber = t);

	public Civ3HSlider AddGrabberHighlight(ImageTexture texture) =>
		SetIcon("grabber_highlight", texture, t => grabberHighlight = t);

	public Civ3HSlider AddSliderStyleBox(StyleBox styleBox) =>
		SetStyleBox("slider", styleBox, s => sliderStyleBox = s);

	private Civ3HSlider SetIcon(string name, ImageTexture texture, Action<ImageTexture> assign) {
		assign(texture);
		if (texture != null)
			AddThemeIconOverride(name, texture);
		return this;
	}

	private Civ3HSlider SetStyleBox(string name, StyleBox styleBox, Action<StyleBox> assign) {
		assign(styleBox);
		if (styleBox != null)
			AddThemeStyleboxOverride(name, styleBox);
		return this;
	}

	// Unlike Civ3TextureButton or Civ3TextureRect where properties like Texture, TextureNormal, etc,
	// are exposed in their parent classes, when we override a theme,
	// godot seems to do some behind the scenes (pun not intended) magic
	// so it's not enough to expose our custom texture fields,
	// and then use _ValidateProperty and Util.ApplyNoSaveFlag
	// to prevent godot from serializing the image textures.
	// We do this "hacky" thing where before godot saves the scene in the editor,
	// we remove all the theme overrides, godot saves the scenes, and re-applies them.
	// It needs a bigger setup than I would prefer, but currently seems to be the most reliable way
	// of preventing godot from serializing the textures.
	// This is the post I found that helped with this (had to use some chatgpt help to translate it to c#):
	// https://forum.godotengine.org/t/prevent-generated-resources-from-being-serialized-in-the-tscn/46562/3
	public override void _Notification(int what) {
		if (what == NotificationEditorPreSave) {
			RemoveThemeIconOverride("grabber");
			RemoveThemeIconOverride("grabber_highlight");
			RemoveThemeStyleboxOverride("slider");
		} else if (what == NotificationEditorPostSave) {
			if (grabber != null) {
				AddGrabber(grabber);
			}
			if (grabberHighlight != null) {
				AddGrabberHighlight(grabberHighlight);
			}

			if (sliderStyleBox != null) {
				AddSliderStyleBox(sliderStyleBox);
			}
		}
	}
}
