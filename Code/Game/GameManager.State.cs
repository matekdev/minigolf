namespace Minigolf;

public partial class GameManager
{
	public static bool IsMovingToNextHole => IsHoleEnding;

	[HostSync]
	public static bool IsHoleEnding { get; private set; }

	protected override void OnUpdate()
	{
		if ( !IsProxy )
		{
			var isHoleFinished = Clients.All( c => !c.IsValid() || c.Pawn is not GolfBall ball );
			if ( isHoleFinished && !IsHoleEnding )
				HoleOutro();
		}
	}

	private async void HoleOutro()
	{
		IsHoleEnding = true;

		await GameTask.DelaySeconds( 3.5f );
		UI.Scoreboard.ForceScoreboard( true );

		await GameTask.DelaySeconds( 5.0f );

		var isGameFinished = AssignNextHole();
		if ( isGameFinished )
		{
			EndGame();
			return;
		}

		UI.Scoreboard.ForceScoreboard( false );
		IsHoleEnding = false;
	}

	private bool AssignNextHole()
	{
		var nextHole = GetNextHole();
		if ( nextHole is null )
			return true;

		CurrentHoleNumber = nextHole.Value.HoleNumber;
		SpawnBallAtHole( nextHole.Value );

		return false;
	}

	[Broadcast]
	private void SpawnBallAtHole( HoleInfo holeInfo )
	{
		var pawn = Client.Local.AssignPawn<GolfBall>();
		if ( pawn is GolfBall ball )
			ball.Respawn( holeInfo );
	}

	private HoleInfo? GetNextHole()
	{
		return Holes.ElementAtOrDefault( CurrentHoleIndex + 1 );
	}

	private void EndGame()
	{

	}
}
