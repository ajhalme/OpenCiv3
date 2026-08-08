using System.Collections.Generic;
using System.Linq;
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

	private List<IVictory> _victoryConditions;

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

		if (_victoryConditions == null) {
			RegisterVictoryConditions(gameData);
		}

		Player player = gameData.GetFirstHumanPlayer();
		List<Player> rivals = gameData.GetKnownRivals(player);

		// Render

		AddTitleRow("", "To Win", "", player.civilization.name, "", "Top Rival"); // TODO: Draw on separate, 3-col grid

		foreach (IVictory vc in _victoryConditions!) {
			ProcessVictoryCondition(vc, gameData, player, rivals);
		}
	}

	private void RegisterVictoryConditions(GameData gameData) { // TODO: push into gameData
		_victoryConditions = [];

		VictoryConditions conditions = gameData.victoryConditions;

		if (conditions.AllowDominationVictory) {
			var dominationAreaLimit = 66f; // TODO: ruleset, dom area victory condition
			var dominationPopulationLimit = 66f; // TODO: ruleset, dom pop victory condition
			_victoryConditions.Add(new DominationVictory(dominationAreaLimit, dominationPopulationLimit));
		}

		if (conditions.AllowCulturalVictory) {
			var totalCultureLimit = 100000; // TODO: ruleset, total culture victory condition
			var topCityCultureLimit = 20000; // TODO: ruleset, one city culture victory condition
			_victoryConditions.Add(new CulturalVictory(totalCultureLimit, topCityCultureLimit));
		}

		// Always render score (not a victory condition in itself)
		_victoryConditions.Add(new ScoreVictory());

		if (conditions.AllowSpaceRaceVictory) {
			var partsToBuild = 10; // TODO: ruleset, space race victory condition
			_victoryConditions.Add(new SpaceRaceVictory(partsToBuild));
		}

		if (conditions.AllowDiplomaticVictory) {
			_victoryConditions.Add(new DiplomaticVictory());
		}

		if (conditions.AllowConquestVictory) {
			// TODO: ruleset, Conquest victory condition
			_victoryConditions.Add(new ConquestVictory(rivalsAliveLimit: 0));
		}

		// TODO: Does the original have a switch to have the game never end?
		// Always add time limits
		_victoryConditions.Add(new TimeLimitVictory(gameData.timeOptions.turnLimit));
	}

	private void ProcessVictoryCondition(IVictory dv, GameData gameData, Player player, List<Player> rivals) {
		VictoryStatus status = dv.Evaluate(player, gameData);
		List<VictoryStatus> rivalStatuses = rivals.Select(r => dv.Evaluate(r, gameData)).ToList();

		AddHeaderRow(dv.Header());

		foreach (string[] output in dv.GenerateStatusRows(status, rivalStatuses)) {
			AddDataRow(output);
		}
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
