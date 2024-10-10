namespace Minigolf;

public static class Scores
{
	private static readonly Dictionary<int, string> _scoreText = new()
	{
		{ 4, "Condor" },
		{ 3, "Double Eagle" },
		{ 2, "Eagle" },
		{ 1, "Birdie" },
		{ 0, "Par" },
		{ -1, "Bogey" },
		{ -2, "Double Bogey" },
		{ -3, "Triple Bogey" },
		{ -4, "Quadruple Bogey" },
		{ -5, "Quintuple Bogey" },
		{ -6, "Sextuple Bogey" },
		{ -7, "Septuple Bogey" },
		{ -8, "Octuple Bogey" },
		{ -9, "Nonuple Bogey" },
		{ -10, "Decuple Bogey" },
	};

	public static string GetScoreText( int par, int score )
	{
		return _scoreText.GetValueOrDefault( par - score, $"{par - score} Over Par" );
	}

	public static string GetParScreenScoreText( int score )
	{
		return _scoreText.GetValueOrDefault( score, $"WTF {score}" );
	}
}
