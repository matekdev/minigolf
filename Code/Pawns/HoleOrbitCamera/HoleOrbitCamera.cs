namespace Minigolf;

[Pawn( "hole_orbit_camera.prefab" )]
public partial class HoleOrbitCamera : Pawn
{
	[Property, Category( "Core" )]
	public GameObject Camera { get; set; }

	private float LerpSpeed => 4.0f;
	private float DistanceAwayFromHole => 250.0f;

	public Vector3 OrbitPosition { get; set; }
	private Vector3 TargetPosition { get; set; }
	private Rotation TargetRotation { get; set; }
	float _rotation = 0.0f;
	private TimeUntil _timeUntilSpectate;

	public void Setup( Vector3 pos, Rotation rot, Vector3 orbitPos )
	{
		OrbitPosition = orbitPos;
		Transform.Position = pos;
		Transform.Rotation = rot;
		Transform.ClearInterpolation();
	}

	protected override void OnStart()
	{
		Camera.Enabled = !IsProxy;
		_timeUntilSpectate = 3f;
	}

	protected override void OnUpdate()
	{
		_rotation += RealTime.Delta * 10.0f;

		Rotation rot = Rotation.FromYaw( _rotation );

		Vector3 dir = (Vector3.Up * 0.35f) + (Vector3.Forward * rot);
		dir = dir.Normal;

		TargetPosition = OrbitPosition + Vector3.Up * 50.0f + dir * DistanceAwayFromHole;
		TargetRotation = Rotation.From( (-dir).EulerAngles );

		// Slerp slerp
		Transform.Position = Transform.Position.LerpTo( TargetPosition, RealTime.Delta * LerpSpeed );
		Transform.Rotation = Rotation.Slerp( Transform.Rotation, TargetRotation, RealTime.Delta * LerpSpeed );

		if ( !IsProxy && Owner.IsValid() && _timeUntilSpectate )
			Owner.AssignPawn<Spectate>();
	}
}
