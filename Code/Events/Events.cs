namespace Minigolf;

public record ClientConnectedEvent( Client Client ) : IGameEvent;

public record ClientDisconnectedEvent( Client Client ) : IGameEvent;

public record BallCuppedEvent( Client Client, HoleInfo HoleInfo, int Score ) : IGameEvent;

public record ForceScoreboardEvent( bool Open ) : IGameEvent;

public record ChatMessageEvent( string DisplayName, ulong SteamId, string Message ) : IGameEvent;

public record InfoChatMessageEvent( string Message ) : IGameEvent;
