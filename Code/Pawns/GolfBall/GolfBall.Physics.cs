namespace Minigolf;

public partial class GolfBall
{
	public bool InWater { get; private set; }
	public Angles Direction { get; private set; }
	public Vector3 BaseVelocity { get; private set; }

	[Sync]
	public Vector3 Velocity { get; private set; }

	[Sync]
	public Rotation Rotation { get; private set; } = Rotation.Identity;

	private void Move()
	{
		var mover = new MoveHelper( WorldPosition, Velocity );
		mover.Trace = Scene.Trace.Radius( 1.5f ).IgnoreGameObject( GameObject );
		mover.MaxStandableAngle = 50.0f;
		mover.GroundBounce = 0.25f; // TODO: Get from ground surface?
		mover.WallBounce = 0.5f;

		var groundTrace = mover.TraceDirection( Vector3.Down * 0.5f );

		if ( groundTrace.GameObject.IsValid() && groundTrace.GameObject.Components.TryGet<Rigidbody>( out var rigidbody ) )
			mover.GroundVelocity = rigidbody.Velocity;

		// Apply gravity
		mover.Velocity += Vector3.Down * 800 * Time.Delta;

		if ( groundTrace.Hit && groundTrace.Normal.Angle( Vector3.Up ) < 1.0f )
		{
			mover.Velocity = ProjectOntoPlane( mover.Velocity, groundTrace.Normal );
		}

		mover.TryMove( Time.Delta );
		mover.TryUnstuck();

		if ( InWater )
		{
			mover.ApplyFriction( 5.0f, Time.Delta );
		}

		// Apply friction based on our ground surface
		if ( groundTrace.Hit )
		{
			var friction = groundTrace.Surface.Friction;

			// Apply more friction if the ball is close to stopping
			if ( mover.Velocity.Length < 1.0f )
				friction = 5.0f;

			mover.ApplyFriction( friction, Time.Delta );
		}
		else
		{
			// Air drag
			mover.ApplyFriction( 0.5f, Time.Delta );
		}

		WorldPosition = mover.Position;
		BaseVelocity = mover.GroundVelocity;
		Velocity = mover.Velocity;

		if ( mover.HitWall )
			ImpactEffects( mover.Position, mover.Velocity.Length );

		if ( Velocity.Length > 16.0f )
			Direction = Velocity.Normal.WithZ( 0 ).EulerAngles;
	}

	private Vector3 ProjectOntoPlane( Vector3 v, Vector3 normal, float overBounce = 1.0f )
	{
		float backoff = v.Dot( normal );

		if ( overBounce != 1.0 )
		{
			if ( backoff < 0 )
				backoff *= overBounce;
			else
				backoff /= overBounce;
		}

		return v - backoff * normal;
	}
}
