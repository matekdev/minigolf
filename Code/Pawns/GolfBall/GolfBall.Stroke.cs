namespace Minigolf;

public partial class GolfBall
{
	[Property, Category( "Sounds" )]
	public SoundEvent SwingSound { get; set; }

	public const float POWER_MULTIPLIER = 2500.0f;

	public float ShotPower { get; private set; } = 0.0f;
	public float LastShotPower { get; private set; } = 0.0f;

	private void StrokeInput()
	{
		if ( Cupped || InPlay )
			return;

		if ( Input.Down( InputAction.LeftClick ) )
		{
			float delta = Input.AnalogLook.pitch * RealTime.Delta;
			ShotPower = Math.Clamp( ShotPower - delta, 0, 1 );
		}

		if ( ShotPower >= 0.01f && !Input.Down( InputAction.LeftClick ) )
		{
			Stroke( Camera.WorldRotation.Forward, ShotPower );
			LastShotPower = ShotPower;
			ShotPower = 0;
		}
	}

	private void Stroke( Vector3 direction, float power )
	{
		Client.Local.AddPar();
		GameObject.PlaySound( SwingSound );

		direction = direction.WithZ( 0 ).Normal;
		power = Math.Clamp( power, 0, 1 );

		// gradient the power, smaller shots have less power
		// y = 2.78(0.5x + 0.1)^2
		power = 2.78f * MathF.Pow( 0.5f * power + 0.1f, 2.0f );

		Direction = direction.EulerAngles;
		Velocity = direction * power * POWER_MULTIPLIER;
	}
}
