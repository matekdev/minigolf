namespace Minigolf;

[Pawn( "spectate.prefab" )]
public partial class Spectate : Pawn
{
	[Property, Category( "Components" )]
	public CameraComponent Camera { get; set; }

	private Angles EyeAngles { get; set; }

	protected override void OnStart()
	{
		Camera.Enabled = !IsProxy;
	}

	protected override void OnUpdate()
	{
		EyeAngles += Input.AnalogLook;
		EyeAngles = EyeAngles.WithPitch( EyeAngles.pitch.Clamp( -90, 90 ) );
		WorldRotation = EyeAngles.ToRotation();
	}
}
