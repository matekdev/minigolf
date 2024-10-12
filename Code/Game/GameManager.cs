namespace Minigolf;

public partial class GameManager : Component, Component.INetworkListener
{
	private static GameManager Instance { get; set; }

	public static HoleInfo? CurrentHole => Instance?.InternalHoles?.FirstOrDefault( h => h.HoleNumber == Instance?.CurrentHoleNumber );
	public static int CurrentHoleIndex => Instance?.InternalHoles?.IndexOf( CurrentHole.Value ) ?? -1;
	public static IReadOnlyList<HoleInfo> Holes => Instance?.InternalHoles ?? new();
	public static float TimeUntilStart => Instance?.StartTime ?? 0;

	[HostSync]
	private NetList<HoleInfo> InternalHoles { get; set; } = new();

	[HostSync]
	private int CurrentHoleNumber { get; set; } = -1;

	public const float WAITING_TIME = 1.0f;

	[HostSync]
	private float StartTime { get; set; }

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
			Log.Warning( "This map does NOT support minigolf... disconnecting..." );
			Game.Disconnect();
			return;
		}

		StartWaiting();
	}

	private async void StartWaiting()
	{
		StartTime = Time.Now + WAITING_TIME;
		await GameTask.DelaySeconds( WAITING_TIME );
		State = GameState.MovingToNextHole;
	}
}
