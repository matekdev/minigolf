namespace Minigolf;

public class HoleGoal : Component
{
	/// <summary>
	/// Tne number of the current hole. Make sure to match this with the spawn point!
	/// </summary>
	[Property]
	public int HoleNumber { get; set; } = 1;

	[RequireComponent]
	public BoxCollider BoxCollider { get; set; }

	protected override void OnAwake()
	{
		BoxCollider.IsTrigger = true;
		BoxCollider.OnTriggerEnter += OnTriggerEnter;
	}

	private void OnTriggerEnter( Collider other )
	{
		if ( !other.IsValid() || !other.GameObject.IsValid() )
			return;

		if ( !other.GameObject.Components.TryGet<GolfBall>( out var ball ) )
			return;

		if ( !ball.IsValid() || !ball.Owner.IsValid() || ball.Owner.IsProxy )
			return;

		var currentHole = GameManager.CurrentHole;
		if ( currentHole is null )
			return;

		if ( currentHole.Value.HoleNumber != HoleNumber )
		{
			// TODO: Reset ball?
			return;
		}

		CuppedEvent( ball, currentHole.Value );

		if ( !ball.IsProxy )
			ball.Cup( WorldPosition );
	}

	[Broadcast]
	private void CuppedEvent( GolfBall ball, HoleInfo currentHole )
	{
		Scene.Dispatch( new BallCuppedEvent( ball.Owner, currentHole, ball.Owner.GetCurrentHolePar() ) );
	}

	protected override void DrawGizmos()
	{
		Gizmo.Draw.WorldText(
			$"Hole End #{HoleNumber}",
			new Transform() { Position = new Vector3( 0.0f, 0.0f, 10.0f ), Scale = new Vector3( 0.1f ) },
			"Roboto",
			28
		);

		if ( Gizmo.IsSelected )
		{
			Gizmo.Draw.Color = Color.FromBytes( 117, 184, 46 );
			Gizmo.Draw.SolidBox( BBox.FromPositionAndSize( BoxCollider.Center, BoxCollider.Scale ) );
		}
	}
}
