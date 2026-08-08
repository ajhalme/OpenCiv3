using System.Collections.Generic;
using System.Linq;

namespace C7GameData;

public class VictoryStatusOld {

	public int TotalCultureLimit { get; set; }
	public int TotalCulture { get; set; }
	public int TopCityCultureLimit { get; set; }
	public int TopCityCulture { get; set; }
	public string TopCityName { get; set; }
	public float TurnScore { get; set; }
	public bool OwnsUnitedNations { get; set; }
	public int RivalsAliveLimit { get; set; }
	public int RivalsAlive { get; set; }
	public int TurnLimit { get; set; }
	public int CurrentTurn { get; set; }
}

public static class VictoryCalculator {
	public static VictoryStatusOld ComputeVictoryStatus(Player player, GameData gameData) {
		var allMapTiles = gameData.map.tiles;
		var allScoreTiles = allMapTiles.Where(t => t.IsCountedForScore()).ToList();

		List<Tile> ownedTiles = player.tileKnowledge.OwnedTiles();
		var scoredTiles = player.tileKnowledge.ScoreTiles();

		var citizens = player.cities.SelectMany(c => c.residents.Where(r => r.citizenType.IsDefaultCitizen)).ToList();
		var happyCitizens = citizens.Count(c => c.mood == CityResident.Mood.Happy);
		var contentCitizens = citizens.Count(c => c.mood == CityResident.Mood.Content);
		var specialists = player.cities.Sum(c => c.residents.Count(r => !r.citizenType.IsDefaultCitizen));

		var futureTechs = 0; // TODO: future techs

		var difficultyFactor = 1; // TODO: gameData.gameDifficulty.ScoreMultiplier
		var turnScore = (scoredTiles.Count + (2 * happyCitizens) + contentCitizens + specialists + futureTechs);
		turnScore *= difficultyFactor;

		var spaceRaceParts = 0; // TODO: space race

		var ownsUnitedNations = player.cities
			.Any(c => c.GetBuildings().Any(b => b.building.CanTriggerDiplomaticVictoryVote));

		var citiesByCulture = player.cities
			.Select(c => new { Name = c.name, Culture = c.GetCulture()})
			.OrderByDescending(c => c.Culture)
			.ToList();

		var totalCulture = citiesByCulture.Sum(c => c.Culture);
		var topCityCulture = citiesByCulture.FirstOrDefault();

		var rivalsAlive = gameData.GetRivals(player).Count(p => !p.defeated);

		var turnLimit = gameData.timeOptions.turnLimit;
		var currentTurn = gameData.turn;

		return new VictoryStatusOld() {

			TotalCultureLimit = 100000, // TODO: ruleset, total culture victory condition
			TotalCulture = totalCulture,
			TopCityCultureLimit = 20000, // TODO: ruleset, one city culture victory condition
			TopCityCulture = topCityCulture?.Culture ?? 0,
			TopCityName = topCityCulture?.Name ?? "None",

			TurnScore = turnScore,

			OwnsUnitedNations = ownsUnitedNations,

			RivalsAliveLimit = 0, // TODO: ruleset, Conquest victory condition
			RivalsAlive = rivalsAlive,

			TurnLimit = turnLimit,
			CurrentTurn = currentTurn
		};
	}
}

