namespace Minigolf;

public partial class GameManager
{
	public static IReadOnlyList<Client> Clients => Instance?.InternalClients;

	[Property]
	public GameObject ClientPrefab { get; set; }

	[HostSync]
	private NetList<Client> InternalClients { get; set; } = new();

	protected override void OnStart()
	{
		if ( !Networking.IsActive )
		{
			Networking.CreateLobby();
			InitializeHoles();
		}

		var clientObj = ClientPrefab.Clone();
		clientObj.SetupNetworking( Connection.Local, OwnerTransfer.Fixed, NetworkOrphaned.Destroy );

		var client = clientObj.Components.Get<Client>();
		client.AssignConnection( Connection.Local );
		client.AssignPawn<Spectate>();
		ClientConnected( client, Connection.Local.Id );
	}

	[Broadcast]
	private void ClientConnected( Client client, Guid connectionId )
	{
		Scene.Dispatch( new ClientConnectedEvent( connectionId ) );

		if ( IsProxy || !client.IsValid() )
			return;

		client.AssignConnection( Connection.Find( connectionId ) );
		InternalClients.Add( client );
	}

	public void OnDisconnected( Connection channel )
	{
		Scene.Dispatch( new ClientDisconnectedEvent( channel.Id ) );

		if ( IsProxy )
			return;

		for ( int i = InternalClients.Count; i >= 0; i-- )
		{
			var client = InternalClients[i];
			if ( !client.IsValid() || client.ConnectionId == channel.Id )
				InternalClients.Remove( client );
		}
	}
}
