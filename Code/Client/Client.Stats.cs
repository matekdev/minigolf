namespace Minigolf;

public partial class Client
{
	/// <summary>
	/// Maps hole number to the client's current par.
	/// </summary>
	[Sync]
	private NetDictionary<int, int> ParScores { get; set; } = new();

	public void AddPar()
	{
		var currentHole = GameManager.CurrentHole;
		if ( currentHole is null )
			return;

		if ( ParScores.ContainsKey( currentHole.Value.HoleNumber ) )
			ParScores[currentHole.Value.HoleNumber] += 1;
		else
			ParScores[currentHole.Value.HoleNumber] = 1;
	}

	public int GetPar( int holeNumber )
	{
		return ParScores?.ContainsKey( holeNumber ) ?? false ? ParScores[holeNumber] : 0;
	}

	public int GetCurrentHolePar()
	{
		var currentHole = GameManager.CurrentHole;
		if ( currentHole is null )
			return -1;

		return ParScores?.ContainsKey( currentHole.Value.HoleNumber ) ?? false ? ParScores[currentHole.Value.HoleNumber] : -1;
	}

	public int GetTotalPar()
	{
		return ParScores.Sum( entry => entry.Value );
	}
}
