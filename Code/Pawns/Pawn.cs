namespace Minigolf;

public abstract class Pawn : Component
{
	[Sync]
	public Client Owner { get; private set; }

	public virtual void OnPossess( Client client )
	{
		Owner = client;
	}

	public virtual void OnDePossess()
	{
		if ( GameObject.IsValid() )
			GameObject.Destroy();
	}
}
