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

		VictoryConditions conditions = gameData.victoryConditions;

		Player player = gameData.GetFirstHumanPlayer();
		List<Player> rivals = gameData.GetKnownRivals(player);

		VictoryStatus victoryStatus = VictoryCalculator.ComputeVictoryStatus(player, gameData);
		Dictionary<Player, VictoryStatus> rivalStatuses =
			rivals.ToDictionary(r => r, r => VictoryCalculator.ComputeVictoryStatus(r, gameData));

		// Render

		AddTitleRow("", "To Win", "", player.civilization.name, "", "Top Rival");

		if (conditions.AllowDominationVictory) {
			AddDominationVictory(victoryStatus, rivalStatuses);
		}

		// AddCulturalVictory(victoryStatus, rivalStatuses);
		// AddScore(victoryStatus, rivalStatuses);
		// AddSpaceRaceVictory(victoryStatus, rivalStatuses);
		// AddDiplomaticVictory(player, victoryStatus, rivalStatuses);

		if (conditions.AllowConquestVictory) {
			AddConquestVictory(victoryStatus);
		}

		AddTimeLimits(victoryStatus);
	}

	private void AddDominationVictory(VictoryStatus vs, Dictionary<Player, VictoryStatus> rivalStatuses) {
		AddHeaderRow("Domination");

		var topRivalByDominationArea =
			rivalStatuses.OrderByDescending(r => r.Value.DominationArea).FirstOrDefault();

		var topAreaRival = topRivalByDominationArea.Key?.civilization?.name ?? "";
		var topAreaRivalValue = topRivalByDominationArea.Value?.DominationArea ?? float.NaN;

		AddDataRow("% of world area:", $"{vs.DominationAreaLimit}",
			"Your % of world area:", $"{vs.DominationArea:F0}",
			topAreaRival,
			float.IsNaN(topAreaRivalValue) ? "" : $"{topAreaRivalValue:F0}");


		var topRivalByDominationPopulation =
			rivalStatuses.OrderByDescending(r => r.Value.DominationPopulation).FirstOrDefault();

		var topPopRival = topRivalByDominationPopulation.Key?.civilization?.name ?? "";
		var topPopRivalValue = topRivalByDominationPopulation.Value?.DominationPopulation ?? float.NaN;

		AddDataRow("% of world population:", $"{vs.DominationPopulationLimit}",
			"Your % of world population:", $"{vs.DominationPopulation:F0}",
			topPopRival,
			float.IsNaN(topPopRivalValue) ? "" : $"{topPopRivalValue:F0}");
	}

	private void AddCulturalVictory(VictoryStatus vs, Dictionary<Player, VictoryStatus> rivalStatuses) {
		var topRivalByCultureOneCity =
			rivalStatuses.OrderByDescending(r => r.Value.TopCityCulture).First();

		var topRivalByCulture =
			rivalStatuses.OrderByDescending(r => r.Value.TotalCulture).First();

		AddHeaderRow("Cultural");

		AddDataRow("One city", $"{vs.TopCityCultureLimit}",
			vs.TopCityName, $"{vs.TopCityCulture}",
			$"{topRivalByCultureOneCity.Value.TopCityName} ({topRivalByCultureOneCity.Key.civilization.name})",
			$"{topRivalByCultureOneCity.Value.TopCityCulture}");

		AddDataRow("Entire civilization", $"{vs.TotalCultureLimit}",
			"Entire civilization", $"{vs.TotalCulture}",
			$"{topRivalByCulture.Key.civilization.name}",
			$"{topRivalByCulture.Value.TopCityCulture}");
	}

	private void AddScore(VictoryStatus vs, Dictionary<Player, VictoryStatus> rivalStatuses) {
		var topRivalByScore =
			rivalStatuses.OrderByDescending(r => r.Value.TurnScore).First();

		AddHeaderRow("Score (Turn)"); // TODO: Use proper score (average over turns)

		AddDataRow("Tie-breaker at time limit", "",
			"Current score:", $"{vs.TurnScore}",
			$"{topRivalByScore.Key.civilization.name}",
			$"{topRivalByScore.Value.TurnScore}");
	}

	private void AddSpaceRaceVictory(VictoryStatus vs, Dictionary<Player, VictoryStatus> rivalStatuses) {
		// TODO: Space race

		AddHeaderRow("Space Race");
		AddDataRow("Parts built:", "??", "Parts built:", "??", "", "");
	}

	private void AddDiplomaticVictory(Player player, VictoryStatus vs, Dictionary<Player, VictoryStatus> rivalStatuses) {
		KeyValuePair<Player, VictoryStatus> ownsUnitedNations
			= rivalStatuses.FirstOrDefault(x => x.Value.OwnsUnitedNations);

		Player owner = ownsUnitedNations.Key ?? (vs.OwnsUnitedNations ? player : null);
		string builtBy = owner == null ? "" : $"\n{owner.civilization.name}";
		string builtByPlaceholder = owner == null ? "No one" : "";

		AddHeaderRow("Diplomatic");
		AddDataRow("Elected as leader", "", "", "", $"The United Nations built by:{builtBy}", builtByPlaceholder);
	}

	private void AddConquestVictory(VictoryStatus vs) {
		AddHeaderRow("Conquest");
		AddDataRow("Eliminate all rivals", "", "", "", "Rivals still alive:", $"{vs.RivalsAlive}");
	}

	private void AddTimeLimits(VictoryStatus vs) {
		AddHeaderRow("Time Limits");
		AddDataRow("Turns in game:", $"{vs.TurnLimit}", "", "", "Current turn:", $"{vs.CurrentTurn}");
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
