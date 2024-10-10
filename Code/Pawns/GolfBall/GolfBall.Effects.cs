namespace Minigolf;

public partial class GolfBall
{
	[Property, Category( "Effects" )]
	public GameObject CircleEffect { get; set; }

	private PowerArrow PowerArrow { get; set; }

	private void ImpactEffects( Vector3 pos, float speed )
	{
		// TODO: New particles here.
	}

	private void UpdateCircleEffect()
	{
		CircleEffect.Enabled = !InPlay && !Cupped;
	}

	private void UpdatePowerArrow()
	{
		if ( ShotPower.AlmostEqual( 0 ) )
		{
			if ( PowerArrow != null )
			{
				PowerArrow.Delete();
				PowerArrow = null;
			}

			return;
		}

		if ( !PowerArrow.IsValid() )
			PowerArrow = new( Game.ActiveScene.SceneWorld );

		var direction = Angles.AngleVector( new Angles( 0, Camera.Transform.Rotation.Yaw(), 0 ) );
		var ballRadius = 3f;
		PowerArrow.Position = Transform.Position + Vector3.Down * ballRadius + Vector3.Up * 0.01f + direction * 5.0f;
		PowerArrow.Direction = direction;
		PowerArrow.Power = ShotPower;
	}

	private void BallRotation()
	{
		float pitchRotation = Velocity.x * Time.Delta * 20f;
		float rollRotation = -Velocity.y * Time.Delta * 20f;
		var ballPitch = Rotation.FromPitch( pitchRotation );
		var ballRoll = Rotation.FromRoll( rollRotation );
		Renderer.Transform.Rotation = ballPitch * ballRoll * Renderer.Transform.Rotation;
	}
}
