namespace Minigolf;

[Pawn( "golfball.prefab" )]
public partial class GolfBall : Pawn
{
	[Property, Category( "Core" ), Sync]
	public ModelRenderer Renderer { get; set; }

	[Property, Category( "Core" )]
	public GameObject Camera { get; set; }

	[Sync]
	public bool Cupped { get; private set; }

	[Sync]
	public bool InPlay { get; private set; }

	protected override void OnStart()
	{
		Camera.Enabled = !IsProxy;
	}

	public void Respawn( HoleInfo holeInfo )
	{
		Transform.ClearInterpolation();
		WorldPosition = holeInfo.SpawnPosition;
		EyeAngles = holeInfo.SpawnAngle;
	}

	public void Cup( Vector3 holePosition )
	{
		Cupped = true;

		if ( !Owner.IsValid() ) // this shouldn't ever not be the case...
			return;

		var pos = Camera.WorldPosition;
		var rot = EyeAngles;

		var pawn = Owner.AssignPawn<HoleOrbitCamera>();
		if ( pawn is HoleOrbitCamera camera )
			camera.Setup( pos, rot, holePosition );
	}

	protected override void OnFixedUpdate()
	{
		if ( IsProxy )
			return;

		Move();

		if ( Velocity.Length >= 2.5f )
			InPlay = true;

		// Check if our ball has pretty much stopped (waiting for 0 is nasty)
		if ( Velocity.Length.AlmostEqual( 0.0f, 2.5f ) )
		{
			Velocity = Vector3.Zero;
			InPlay = false;
		}
	}

	protected override void OnUpdate()
	{
		if ( !IsProxy )
		{
			CameraUpdate();
			StrokeInput();

			UpdateCircleEffect();
			UpdatePowerArrow();
		}

		BallRotation();
	}
}
