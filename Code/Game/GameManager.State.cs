namespace Minigolf;

public partial class GameManager
{
	[HostSync]
	public GameState State { get; private set; } = GameState.WaitingForPlayers;

	protected override void OnUpdate()
	{
		if ( !IsProxy )
		{
			var isHoleFinished = Clients.All( c => !c.IsValid() || c.Pawn is not GolfBall ball );
			if ( isHoleFinished && State is GameState.HoleFinished )
				HoleOutro();
		}
	}

	private async void HoleOutro()
	{
		State = GameState.HoleFinished;

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
		State = GameState.InPlay;
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
