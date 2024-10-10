namespace Minigolf;

public partial class GameManager
{
	[ConCmd( "mg_restart" )]
	public static void DebugRestart()
	{
		Instance.CurrentHoleNumber = Instance.InternalHoles.FirstOrDefault().HoleNumber;
		Instance.SpawnBallAtHole( CurrentHole.Value );
	}
}
