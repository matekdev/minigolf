namespace Minigolf;

public partial class GameManager : Component, Component.INetworkListener
{
	private static GameManager Instance { get; set; }

	public static HoleInfo? CurrentHole => Instance?.InternalHoles?.FirstOrDefault( h => h.HoleNumber == Instance?.CurrentHoleNumber );
	public static int CurrentHoleIndex => Instance?.InternalHoles?.IndexOf( CurrentHole.Value ) ?? -1;
	public static IReadOnlyList<HoleInfo> Holes => Instance?.InternalHoles ?? new();

	[HostSync]
	private NetList<HoleInfo> InternalHoles { get; set; } = new();

	[HostSync]
	private int CurrentHoleNumber { get; set; } = -1;

	public GameManager()
	{
		Instance = this;
	}

	private void InitializeHoles()
	{
		foreach ( var hole in Scene.GetAllComponents<GolfBallSpawnPoint>().OrderBy( spawn => spawn.HoleNumber ) )
		{
			var goal = Scene.GetAllComponents<HoleGoal>().Where( goal => goal.HoleNumber == hole.HoleNumber ).FirstOrDefault();
			if ( !goal.IsValid() )
			{
				Log.Warning( $"Hole #{hole.HoleNumber} has no ending hole goal." );
				continue;
			}

			InternalHoles.Add( new HoleInfo()
			{
				HoleNumber = hole.HoleNumber,
				Par = hole.HolePar,
				SpawnPosition = hole.WorldPosition,
				SpawnAngle = hole.WorldRotation.Angles()
			} );
		}

		if ( Holes.Count == 0 )
		{
			Log.Warning( "This map has no golf holes..." );
			Game.Disconnect();
		}

		StartGame();
	}

	private async void StartGame()
	{
		await GameTask.DelaySeconds( 2f );
		State = GameState.InPlay;
	}
}
