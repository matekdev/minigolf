namespace Minigolf;

public partial class GameManager
{
	public static IReadOnlyList<Client> Clients => Instance?.InternalClients;

	[Property]
	public GameObject ClientPrefab { get; set; }

	[HostSync]
	private NetList<Client> InternalClients { get; set; } = new();

	protected override async Task OnLoad()
	{
		if ( !Game.IsPlaying )
			return;

		if ( !GameNetworkSystem.IsActive )
		{
			GameNetworkSystem.CreateLobby();
			InitializeHoles();
		}

		var clientObj = ClientPrefab.Clone();
		clientObj.SetupNetworking( Connection.Local, OwnerTransfer.Fixed, NetworkOrphaned.Destroy );

		var client = clientObj.Components.Get<Client>();
		client.AssignConnection();

		ClientConnected( client, Connection.Local.Id );

		await GameTask.CompletedTask;
	}

	[Broadcast]
	private void ClientConnected( Client client, Guid connectionId )
	{
		Scene.Dispatch( new ClientConnectedEvent( client ) );

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
			Scene.Dispatch( new ClientDisconnectedEvent( client ) );

			if ( Connection.Local.IsHost )
				InternalClients.Remove( client );
		}
	}
}
