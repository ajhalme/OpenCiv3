using System.Collections.Generic;
using System.Linq;
using C7GameData;

namespace C7Engine;

public class ScoreVictory : IVictory {

	public string Header() => "Score (Turn)"; // TODO: Use proper score (average over turns)

	public VictoryStatus Evaluate(Player player, GameData gameData) {
		float score =  ComputeTurnScore(player, gameData);

		return new VictoryStatus {
			Player = player,
			TurnScore = score
		};
	}

	public bool HasVictory(VictoryStatus status) {
		return false;
	}

	public IEnumerable<string[]> GenerateStatusRows(VictoryStatus status, List<VictoryStatus> rivalStatuses) {
		yield return TurnScorePrint(status, rivalStatuses);
	}

	private string[] TurnScorePrint(VictoryStatus status, List<VictoryStatus> rivalStatuses) {
		var topRivalByScore = rivalStatuses.OrderByDescending(r => r.TurnScore).FirstOrDefault();

		var topRival = topRivalByScore?.Player?.civilization?.name ?? "";
		var topRivalScore = topRivalByScore == null ? "" : $"{topRivalByScore.TurnScore}";

		return [
			"Tie-breaker at time limit",
			"",
			"Current score:",
			$"{status.TurnScore}",
			topRival,
			topRivalScore
		];
	}

	public static float ComputeTurnScore(Player player, GameData gameData) {
		List<Tile> scoredTiles = player.tileKnowledge.ScoreTiles();

		List<CityResident> citizens = player.cities.SelectMany(c => c.residents.Where(r => r.citizenType.IsDefaultCitizen)).ToList();

		int happyCitizens = citizens.Count(c => c.mood == CityResident.Mood.Happy);
		int contentCitizens = citizens.Count(c => c.mood == CityResident.Mood.Content);
		int specialists = player.cities.Sum(c => c.residents.Count(r => !r.citizenType.IsDefaultCitizen));

		int futureTechs = 0; // TODO: future techs

		float difficultyFactor = 1f; // TODO: gameData.gameDifficulty.ScoreMultiplier

		float turnScore = (scoredTiles.Count + (2 * happyCitizens) + contentCitizens + specialists + futureTechs);
		turnScore *= difficultyFactor;

		return turnScore;
	}
}
