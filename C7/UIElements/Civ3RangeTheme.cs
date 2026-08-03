using Godot;
using static C7.UIElements.RangeOverrides;

namespace C7.UIElements;

// Some are only for icons, and some only for style boxes,
// this just makes it easier to work with all of them in one place
public static class RangeOverrides {
	public const string SCROLL = "scroll";
	public const string SLIDER = "slider";

	public const string GRABBER = "grabber";
	public const string GRABBER_HIGHLIGHT = "grabber_highlight";
	public const string GRABBER_PRESSED = "grabber_pressed";

	public const string INCREMENT = "increment";
	public const string INCREMENT_HIGHLIGHT = "increment_highlight";
	public const string INCREMENT_PRESSED = "increment_pressed";

	public const string DECREMENT = "decrement";
	public const string DECREMENT_HIGHLIGHT = "decrement_highlight";
	public const string DECREMENT_PRESSED = "decrement_pressed";

	public static readonly string[] AllOverrideStrings = {
		SCROLL, SLIDER,
		GRABBER, GRABBER_HIGHLIGHT, GRABBER_PRESSED,
		INCREMENT, INCREMENT_HIGHLIGHT, INCREMENT_PRESSED,
		DECREMENT, DECREMENT_HIGHLIGHT, DECREMENT_PRESSED
	};
}

public class Civ3RangeTheme {
	// Slider and ScrollBar classes both inherit from Range,
	// so that's why this was chosen
	private readonly Range range;

	public Civ3RangeTheme(Range range) {
		this.range = range;
	}

	// Icons
	public ImageTexture Grabber { get; private set; }
	public ImageTexture GrabberHighlight { get; private set; }
	public ImageTexture GrabberPressed { get; private set; }

	public ImageTexture Increment { get; private set; }
	public ImageTexture IncrementHighlight { get; private set; }
	public ImageTexture IncrementPressed { get; private set; }

	public ImageTexture Decrement { get; private set; }
	public ImageTexture DecrementHighlight { get; private set; }
	public ImageTexture DecrementPressed { get; private set; }

	// StyleBoxes
	public StyleBox ScrollStyleBox { get; private set; }
	public StyleBox SliderStyleBox { get; private set; }

	public StyleBox GrabberStyleBox { get; private set; }
	public StyleBox GrabberHighlightStyleBox { get; private set; }
	public StyleBox GrabberPressedStyleBox { get; private set; }


	// Grabber Icons
	public Civ3RangeTheme AddGrabber(ImageTexture texture) {
		Grabber = texture;
		SetIcon(GRABBER, texture);
		return this;
	}
	public Civ3RangeTheme AddGrabberHighlight(ImageTexture texture) {
		GrabberHighlight = texture;
		SetIcon(GRABBER_HIGHLIGHT, texture);
		return this;
	}
	public Civ3RangeTheme AddGrabberPressed(ImageTexture texture) {
		GrabberPressed = texture;
		SetIcon(GRABBER_PRESSED, texture);
		return this;
	}

	// Increment Icons
	public Civ3RangeTheme AddIncrement(ImageTexture texture) {
		Increment = texture;
		SetIcon(INCREMENT, texture);
		return this;
	}
	public Civ3RangeTheme AddIncrementHighlight(ImageTexture texture) {
		IncrementHighlight = texture;
		SetIcon(INCREMENT_HIGHLIGHT, texture);
		return this;
	}
	public Civ3RangeTheme AddIncrementPressed(ImageTexture texture) {
		IncrementPressed = texture;
		SetIcon(INCREMENT_PRESSED, texture);

		return this;
	}

	// Decrement Icons
	public Civ3RangeTheme AddDecrement(ImageTexture texture) {
		Decrement = texture;
		SetIcon(DECREMENT, texture);
		return this;
	}
	public Civ3RangeTheme AddDecrementHighlight(ImageTexture texture) {
		DecrementHighlight = texture;
		SetIcon(DECREMENT_HIGHLIGHT, texture);
		return this;
	}
	public Civ3RangeTheme AddDecrementPressed(ImageTexture texture) {
		DecrementPressed = texture;
		SetIcon(DECREMENT_PRESSED, texture);
		return this;
	}

	// Scroll StyleBox
	public Civ3RangeTheme AddScrollStyleBox(StyleBox styleBox) {
		ScrollStyleBox = styleBox;
		SetStyleBox(SCROLL, styleBox);
		return this;
	}
	// Slider StyleBox
	public Civ3RangeTheme AddSliderStyleBox(StyleBox styleBox) {
		SliderStyleBox = styleBox;
		SetStyleBox(SLIDER, styleBox);
		return this;
	}

	// Grabber StyleBox
	public Civ3RangeTheme AddGrabberStyleBox(StyleBox styleBox) {
		GrabberStyleBox = styleBox;
		SetStyleBox(GRABBER, styleBox);
		return this;
	}
	public Civ3RangeTheme AddGrabberHighlightStyleBox(StyleBox styleBox) {
		GrabberHighlightStyleBox = styleBox;
		SetStyleBox(GRABBER_HIGHLIGHT, styleBox);
		return this;
	}
	public Civ3RangeTheme AddGrabberPressedStyleBox(StyleBox styleBox) {
		GrabberPressedStyleBox = styleBox;
		SetStyleBox(GRABBER_PRESSED, styleBox);
		return this;
	}

	private void ApplyOverrides() {
		SetIcon(GRABBER, Grabber);
		SetIcon(GRABBER_HIGHLIGHT, GrabberHighlight);
		SetIcon(GRABBER_PRESSED, GrabberPressed);

		SetIcon(INCREMENT, Increment);
		SetIcon(INCREMENT_HIGHLIGHT, IncrementHighlight);
		SetIcon(INCREMENT_PRESSED, IncrementPressed);

		SetIcon(DECREMENT, Decrement);
		SetIcon(DECREMENT_HIGHLIGHT, DecrementHighlight);
		SetIcon(DECREMENT_PRESSED, DecrementPressed);

		SetStyleBox(SCROLL, ScrollStyleBox);
		SetStyleBox(SLIDER, SliderStyleBox);

		SetStyleBox(GRABBER, GrabberStyleBox);
		SetStyleBox(GRABBER_HIGHLIGHT, GrabberHighlightStyleBox);
		SetStyleBox(GRABBER_PRESSED, GrabberPressedStyleBox);
	}

	private void RemoveOverrides() {
		foreach (var attr in AllOverrideStrings) {
			this.range.RemoveThemeIconOverride(attr);
			this.range.RemoveThemeStyleboxOverride(attr);
		}
	}

	private void SetIcon(string name, ImageTexture texture) {
		if (texture != null)
			this.range.AddThemeIconOverride(name, texture);
	}

	private void SetStyleBox(string name, StyleBox styleBox) {
		if (styleBox != null)
			this.range.AddThemeStyleboxOverride(name, styleBox);
	}

	// This gets called when working in the editor,
	// and we save the scene,
	// once before Godot saves the scene (NotificationEditorPreSave)
	// and once after (NotificationEditorPostSave).
	// It removes and re-applies respectively this custom theme 
	// so that Godot doesn't serialize the icon override textures.
	public void ClearAndRestoreOverrides(int what) {
		if (what == Node.NotificationEditorPreSave)
			this.RemoveOverrides();

		if (what == Node.NotificationEditorPostSave)
			this.ApplyOverrides();
	}
}
