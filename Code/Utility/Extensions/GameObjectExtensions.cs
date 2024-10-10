namespace Minigolf;

public static class GameObjectExtensions
{
	public static void SetupNetworking(
		this GameObject obj,
		Connection owner = null,
		OwnerTransfer transfer = OwnerTransfer.Takeover,
		NetworkOrphaned orphaned = NetworkOrphaned.ClearOwner
	)
	{
		if ( !obj.IsValid() )
			return;

		obj.NetworkMode = NetworkMode.Object;

		if ( !obj.Network.Active )
			obj.NetworkSpawn( owner );
		else if ( GameNetworkSystem.IsActive && owner != null )
			obj.Network.AssignOwnership( owner );

		obj.Network.SetOwnerTransfer( transfer );
		obj.Network.SetOrphanedMode( orphaned );
	}

	/// <summary>
	/// Creates a GameObject that plays a sound.
	/// </summary>
	/// <param name="self"></param>
	/// <param name="sndEvent"></param>
	/// <param name="follow"></param>
	public static void PlaySound( this GameObject self, SoundEvent sndEvent, bool follow = true )
	{
		if ( !self.IsValid() )
			return;

		if ( sndEvent is null )
			return;

		var gameObject = self.Scene.CreateObject();
		gameObject.Name = sndEvent.ResourceName;

		if ( follow )
			gameObject.Parent = self;
		else
			gameObject.Transform.World = self.Transform.World;

		var emitter = gameObject.Components.Create<SoundEmitter>();
		emitter.SoundEvent = sndEvent;
		emitter.Play();
	}

	/// <inheritdoc cref="PlaySound(GameObject, SoundEvent, bool)"/>
	public static void PlaySound( this GameObject self, string sndPath, bool follow = true )
	{
		if ( ResourceLibrary.TryGet<SoundEvent>( sndPath, out var sndEvent ) )
			self.PlaySound( sndEvent, follow );
	}
}
