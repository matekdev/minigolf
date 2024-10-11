namespace Minigolf;

public partial class GolfBall
{
	private const float MIN_DISTANCE = 100.0f;
	private const float MAX_DISTANCE = 300.0f;
	private const float DISTANCE_STEP = 10.0f;

	private Angles EyeAngles { get; set; }
	private Rotation TargetRotation { get; set; }

	private float CurrentDistance { get; set; }
	private float TargetDistance { get; set; }

	private void CameraUpdate()
	{
		TargetRotation = Rotation.From( EyeAngles );
		Camera.WorldRotation = Rotation.Slerp( Camera.WorldRotation, TargetRotation, RealTime.Delta * 10.0f );

		Camera.WorldPosition = WorldPosition + Vector3.Up * 10;
		TargetDistance = TargetDistance.LerpTo( CurrentDistance, RealTime.Delta * 5.0f );
		Camera.WorldPosition += Camera.WorldRotation.Backward * TargetDistance;

		CurrentDistance = Math.Clamp( CurrentDistance + -Input.MouseWheel.y * DISTANCE_STEP, MIN_DISTANCE, MAX_DISTANCE );
		EyeAngles = EyeAngles with { yaw = EyeAngles.yaw + Input.AnalogLook.yaw };

		if ( !Input.Down( InputAction.LeftClick ) )
		{
			EyeAngles = EyeAngles with { pitch = EyeAngles.pitch + Input.AnalogLook.pitch };
			EyeAngles = EyeAngles with { pitch = EyeAngles.pitch.Clamp( 0, 89 ) };
		}
	}
}
