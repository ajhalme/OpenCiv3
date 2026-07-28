using System;
using Godot;

namespace C7.UIElements;

[GlobalClass]
[Tool]
public partial class Civ3HSlider : HSlider {
	[Export] public ImageTexture grabber { get; private set; }
	public StyleBox grabberStyleBox { get; private set; }
	[Export] public ImageTexture grabberHighlight { get; private set; }
	public StyleBox grabberHighlightStyleBox { get; private set; }
	[Export] public ImageTexture slider { get; private set; }
	public StyleBox sliderStyleBox { get; private set; }

	public Civ3HSlider AddGrabber(ImageTexture texture) => SetIcon("grabber", texture, t => grabber = t);
	public Civ3HSlider AddGrabberStyleBox(StyleBox styleBox) => SetStyleBox("grabber", styleBox, s => grabberStyleBox = s);
	public Civ3HSlider AddGrabberHighlight(ImageTexture texture) => SetIcon("grabber_highlight", texture, t => grabberHighlight = t);
	public Civ3HSlider AddGrabberHighlightStyleBox(StyleBox styleBox) => SetStyleBox("grabber_highlight", styleBox, s => grabberHighlightStyleBox = s);
	public Civ3HSlider AddSlider(ImageTexture texture) => SetIcon("slider", texture, t => slider = t);
	public Civ3HSlider AddSliderStyleBox(StyleBox styleBox) => SetStyleBox("slider", styleBox, s => sliderStyleBox = s);

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

	public override void _ValidateProperty(Godot.Collections.Dictionary property) {
		Util.ApplyNoSaveFlag(property, [
				PropertyName.grabber,
				PropertyName.grabberHighlight,
				PropertyName.slider,
			]);
	}
}
