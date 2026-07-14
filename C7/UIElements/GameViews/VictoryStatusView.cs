using C7Engine;
using C7GameData;
using Godot;

[GlobalClass]
[Tool]
public partial class VictoryStatusView : Control {

	[Export] public TextureRect background;
	[Export] public GridContainer grid;
	[Export] public float LabelColumnWidth = 185f;
	[Export] public float ValueColumnWidth = 40f;

	private TextureButton _close;

	private const int GridColumns = 6;

	public VictoryStatusView() {
		MouseFilter = MouseFilterEnum.Stop;
	}


	public override void _Ready() {
		CreateUI();
		ConfigureGrid();
	}

	private void CreateUI() {
		background.Texture = TextureLoader.Load("screens.standing.victory_status.background");
		var histogramTexture = TextureLoader.Load("screens.standing.histogram.background");

		_close = AdvisorUtils.CreateExitButton(background);
		_close.Pressed += () => { this.GetParent<GameViews>().Hide(); };

		AdvisorUtils.CreateAdvisorTitle(background, background.Texture.GetWidth(), "VICTORY STATUS");
	}

	private void ConfigureGrid() {
		grid.Columns = GridColumns;
		grid.AddThemeConstantOverride("h_separation", 0); // horizontal gap between columns
		grid.AddThemeConstantOverride("v_separation", 5);
	}

	public void ShowView() {
		Show();

		EngineStorage.ReadGameData(DrawGrid);
	}

	private void DrawGrid(GameData gameData) {
		ClearGrid();

		Player player = gameData.GetFirstHumanPlayer();

		AddTitleRow("", "To Win", "", player.civilization.name, "", "Top Rival");

		AddHeaderRow("Domination");
		AddDataRow("% of world area:", "66", "Your % of world area:", "1", "America", "2");
		AddDataRow("% of world population:", "66", "Your % of world pop.:", "25", "America", "10");

		AddHeaderRow("Cultural");
		AddDataRow("One city", "20000", "Rome", "98", "Washington (America)", "98");
		AddDataRow("Entire civilization", "100000", player.civilization.name, "98", "America", "98");

		AddHeaderRow("Score");
		AddDataRow("Tie-breaker at time limit", "", "Current score:", "28", "America", "35");

		AddHeaderRow("Space Race");
		AddDataRow("Parts built:", "10", "Parts built:", "0", "", "");

		AddHeaderRow("Diplomatic");
		AddDataRow("Elected as leader", "", "", "", "The United Nations built by:", "no one");

		AddHeaderRow("Conquest");
		AddDataRow("Eliminate all rivals", "", "", "", "Rivals still alive:", "6");

		AddHeaderRow("Time Limits");
		AddDataRow("Turns in game:", "540", "", "", "Current turn:", "59");
	}

	private void ClearGrid() {
		foreach (Node child in grid.GetChildren())
			child.QueueFree();
	}


	/// Titles; column headers
	public void AddTitleRow(params string[] values) {
		for (int i = 0; i < GridColumns; i++) {
			bool isLabelCol = i % 2 == 1;

			if (isLabelCol) {
				string valueText = values[i] + "    ";
				var label = MakeLabel(valueText, ValueColumnWidth, HorizontalAlignment.Right);
				label.AddThemeFontSizeOverride("font_size", 18);
				grid.AddChild(label);
			} else {
				grid.AddChild(MakeSpacer(LabelColumnWidth));
			}
		}
	}

	/// A bold, full-width section header row (e.g. "Domination", "Cultural").
	public void AddHeaderRow(string text) {
		var label = MakeLabel(text, LabelColumnWidth, HorizontalAlignment.Left);
		label.AddThemeFontSizeOverride("font_size", 20);
		grid.AddChild(label);

		for (int i = 1; i < GridColumns; i++) {
			bool isLabelCol = i % 2 == 0;
			grid.AddChild(MakeSpacer(isLabelCol ? LabelColumnWidth : ValueColumnWidth));
		}
	}

	private Label MakeLabel(string text, float width, HorizontalAlignment align) {
		var label = new Label
		{
			Text = text,
			HorizontalAlignment = align,
			CustomMinimumSize = new Vector2(width, 0),
			SizeFlagsHorizontal = SizeFlags.ShrinkBegin // don't stretch beyond min width
		};
		return label;
	}

	private Control MakeSpacer(float width) {
		return new Control { CustomMinimumSize = new Vector2(width, 0) };
	}

	/// A data row: label, value; label, value; label, value
	public void AddDataRow(params string[] values) {
		for (int i = 0; i < GridColumns; i++) {
			string valueText = i < values.Length ? values[i] : "";
			bool isLabelCol = i % 2 == 0;

			var valueNode = new Label
			{
				Text = isLabelCol ? valueText : valueText + "      ",
				HorizontalAlignment = isLabelCol ? HorizontalAlignment.Left : HorizontalAlignment.Right,
				SizeFlagsHorizontal = SizeFlags.ExpandFill
			};
			if (isLabelCol) {
				valueNode.AutowrapMode = TextServer.AutowrapMode.WordSmart;
			}

			grid.AddChild(valueNode);
		}
	}
}
