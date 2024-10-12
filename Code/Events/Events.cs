namespace Minigolf;

public record GameStateChangeEvent( GameState OldState, GameState NewState ) : IGameEvent;

public record ClientConnectedEvent( Guid ConnectionId ) : IGameEvent;

public record ClientDisconnectedEvent( Guid ConnectionId ) : IGameEvent;

public record BallCuppedEvent( Client Client, HoleInfo HoleInfo, int Score ) : IGameEvent;

public record ChatMessageEvent( string DisplayName, ulong SteamId, string Message ) : IGameEvent;

public record InfoChatMessageEvent( string Message ) : IGameEvent;
