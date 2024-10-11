namespace Minigolf;

public partial class GameManager
{
	public static IReadOnlyList<Client> Clients => Instance?.InternalClients;

	[Property]
	public GameObject ClientPrefab { get; set; }

	[HostSync]
	private NetList<Client> InternalClients { get; set; } = new();

	// TODO: Rework init logic, the client connection data isn't set up in time...
	protected override async Task OnLoad()
	{
		if ( !Game.IsPlaying )
			return;

		if ( !Networking.IsActive )
		{
			Networking.CreateLobby();
			InitializeHoles();
		}

		var clientObj = ClientPrefab.Clone();
		clientObj.SetupNetworking( Connection.Local, OwnerTransfer.Fixed, NetworkOrphaned.Destroy );

		var client = clientObj.Components.Get<Client>();
		client.AssignConnection();
		AssignStatePawn( State );
		ClientConnected( client, Connection.Local.DisplayName, Connection.Local.Id );

		await GameTask.CompletedTask;
	}

	[Broadcast]
	private void ClientConnected( Client client, string name, Guid connectionId )
	{
		Scene.Dispatch( new ClientConnectedEvent( name, connectionId ) );

		if ( Connection.Local.IsHost )
		{
			client.ConnectionId = connectionId;
			InternalClients.Add( client );
		}
	}

	public void OnDisconnected( Connection channel )
	{
		var client = InternalClients?.FirstOrDefault( c => c?.ConnectionId == channel?.Id );
		if ( client is not null )
		{
			Scene.Dispatch( new ClientDisconnectedEvent( client.DisplayName, client.ConnectionId ) );

			if ( Connection.Local.IsHost )
				InternalClients.Remove( client );
		}
	}
}
