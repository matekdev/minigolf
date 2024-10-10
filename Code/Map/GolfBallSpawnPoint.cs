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
		Gizmo.Draw.Model( "models/golf_ball/golf_ball.vmdl" );
	}
}
