namespace Minigolf;

public partial class GameManager
{
	[HostSync, Change( "OnStateChange" )]
	public GameState State { get; private set; } = GameState.Initialziation;

	private TimeUntil timeUntilStart = 2f;

	protected override void OnUpdate()
	{
		if ( !IsProxy )
		{
			var isHoleFinished = Clients.All( c => !c.IsValid() || c.Pawn is not GolfBall ball );
			if ( isHoleFinished && State is GameState.HoleFinished )
				HoleOutro();

			if ( timeUntilStart && State is GameState.WaitingForPlayers )
				State = GameState.InPlay;
		}
	}

	private void OnStateChange( GameState oldState, GameState newState )
	{
		Scene.Dispatch( new GameStateChangeEvent( oldState, newState ) );
		// TODO: See if we can hook into events to make this work instead.
		AssignStatePawn( newState );
	}

	private void AssignStatePawn( GameState state )
	{
		if ( !Client.Local.IsValid() )
			return;

		Log.Info( "called" );

		switch ( state )
		{
			case GameState.Initialziation:
				Client.Local.AssignPawn<Spectate>();
				break;
			case GameState.WaitingForPlayers:
				Client.Local.AssignPawn<Spectate>();
				break;
			case GameState.InPlay:
				Client.Local.AssignPawn<GolfBall>();
				break;
			case GameState.HoleFinished:
				Client.Local.AssignPawn<HoleOrbitCamera>();
				break;
			case GameState.GameFinished:
				Client.Local.AssignPawn<Spectate>();
				break;
		}
	}

	private async void HoleOutro()
	{
		State = GameState.HoleFinished;
		await GameTask.DelaySeconds( 5.0f );

		var isGameFinished = AssignNextHole();
		if ( isGameFinished )
		{
			EndGame();
			return;
		}

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
