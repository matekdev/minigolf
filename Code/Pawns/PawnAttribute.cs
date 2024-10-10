namespace Minigolf;

[AttributeUsage( AttributeTargets.Class )]
public class PawnAttribute : Attribute
{
	public string PrefabPath { get; private set; }

	public PawnAttribute( string prefabName )
	{
		PrefabPath = "prefabs/pawns/" + prefabName;
	}
}
