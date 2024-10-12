namespace Minigolf;

public partial class GameManager
{
	public static GameState CurrentState => Instance?.State ?? GameState.WaitingForPlayers;

	[HostSync, Change( nameof( OnStateChange ) )]
	private GameState State { get; set; } = GameState.WaitingForPlayers;

	protected override void OnUpdate()
	{
		if ( !IsProxy )
		{
			var isHoleFinished = Clients.All( c => !c.IsValid() || c.Pawn is not GolfBall ball ) && State is GameState.InPlay;
			if ( isHoleFinished )
				State = GameState.HoleComplete;
		}
	}

	private void OnStateChange( GameState oldState, GameState newState )
	{
		Scene.Dispatch( new GameStateChangeEvent( oldState, newState ) );

		switch ( State )
		{
			case GameState.WaitingForPlayers:
			case GameState.GameFinished:
				if ( Client.Local.IsValid() )
					Client.Local.AssignPawn<Spectate>();
				break;
			case GameState.HoleComplete:
				DisplayHoleResults();
				break;
			case GameState.MovingToNextHole:
				MoveToNextHole();
				break;
		}
	}

	private async void DisplayHoleResults()
	{
		if ( IsProxy )
			return;

		await GameTask.DelaySeconds( 4f );
		// UI is shown during this state.
		State = GameState.MovingToNextHole;
		await GameTask.DelaySeconds( 2f );
	}

	private void MoveToNextHole()
	{
		if ( IsProxy )
			return;

		var nextHole = GetNextHole();
		if ( nextHole is null )
		{
			State = GameState.GameFinished;
			return;
		}

		CurrentHoleNumber = nextHole.Value.HoleNumber;
		SpawnBallAtHole( nextHole.Value );
		State = GameState.InPlay;
	}

	private HoleInfo? GetNextHole()
	{
		if ( CurrentHoleIndex == -1 )
			return InternalHoles.FirstOrDefault();

		var nextHoleIndex = CurrentHoleIndex + 1;
		if ( nextHoleIndex >= InternalHoles.Count )
			return null;

		return InternalHoles.ElementAt( CurrentHoleIndex + 1 );
	}

	[Broadcast]
	private void SpawnBallAtHole( HoleInfo holeInfo )
	{
		var pawn = Client.Local.AssignPawn<GolfBall>();
		if ( pawn is GolfBall ball )
			ball.Respawn( holeInfo );
	}
}
