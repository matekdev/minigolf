namespace Minigolf;

public partial class Client : Component
{
	public static Client Local { get; private set; }

	[HostSync, Sync]
	public Guid ConnectionId { get; set; }
	public Connection Connection => Connection.Find( ConnectionId );
	public ulong SteamId => Connection?.SteamId ?? 0;
	public string DisplayName => Connection?.DisplayName;

	[Sync]
	public Pawn Pawn { get; private set; }

	public Pawn AssignPawn<T>()
	{
		var pawnAttribute = TypeLibrary.GetType<T>().GetAttribute<PawnAttribute>();
		var obj = SceneUtility.GetPrefabScene( ResourceLibrary.Get<PrefabFile>( pawnAttribute.PrefabPath ) ).Clone();

		var pawn = obj.Components.Get<Pawn>();
		if ( pawn is null )
		{
			obj.Destroy();
			Log.Warning( $"Assigned GameObject ({obj.Name}) with no pawn component!" );
			return null;
		}

		if ( Pawn.IsValid() )
			Pawn.OnDePossess(); // TODO: Don't destroy, kill

		obj.SetupNetworking( Connection, OwnerTransfer.Fixed, NetworkOrphaned.Destroy );
		obj.Name = $"{DisplayName} - {obj.Name.ToUpper()} Pawn";

		Pawn = pawn;
		Pawn.OnPossess( this );

		return pawn;
	}

	public void AssignConnection( Connection connection )
	{
		ConnectionId = connection.Id;
		GameObject.Name = $"{Connection.DisplayName} - CLIENT";

		if ( connection == Connection.Local )
			Local = this;
	}
}
