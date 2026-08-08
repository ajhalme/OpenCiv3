using System.Collections.Generic;
using System.Linq;

namespace C7GameData;

public class VictoryStatusOld {
	public float TurnScore { get; set; }
	public bool OwnsUnitedNations { get; set; }
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

		return new VictoryStatusOld() {

			TurnScore = turnScore,

			OwnsUnitedNations = ownsUnitedNations,
		};
	}
}

