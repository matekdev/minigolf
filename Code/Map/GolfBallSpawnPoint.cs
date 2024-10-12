namespace Minigolf;

public class GolfBallSpawnPoint : Component
{
	/// <summary>
	/// Tne number of the current hole.
	/// </summary>
	[Property]
	public int HoleNumber { get; set; } = 1;

	/// <summary>
	/// How many strokes should this hole be done in.
	/// </summary>
	[Property]
	public int HolePar { get; set; } = 3;

	protected override void DrawGizmos()
	{
		Gizmo.Draw.WorldText(
			$"Hole Spawn #{HoleNumber}",
			new Transform() { Position = new Vector3( 0.0f, 0.0f, 10.0f ), Scale = new Vector3( 0.1f ) },
			"Roboto",
			28
		);

		Gizmo.Draw.Model( "models/golf_ball/golf_ball.vmdl" );
	}
}
