namespace Minigolf;

public partial class PowerArrow : SceneCustomObject
{
	public Material Material = Material.Load( "materials/power_arrow/power_arrow.vmat" );
	public Vector3 Direction = Vector3.Zero;
	public float Power = 0.0f;

	public PowerArrow( SceneWorld sceneWorld ) : base( sceneWorld )
	{

	}

	protected void DrawArrow( Vector3 startPos, Vector3 endPos, Vector3 direction, Vector3 size, Color color )
	{
		// vbos are drawn relative to world position
		startPos -= Position;
		endPos -= Position;

		VertexBuffer vertexBuffer = new();
		vertexBuffer.Init( true );

		// Line
		Vertex a = new( startPos - size, Vector3.Up, Vector3.Right, new Vector4( 0, 1, 0, 0 ) );
		Vertex b = new( startPos + size, Vector3.Up, Vector3.Right, new Vector4( 1, 1, 0, 0 ) );
		Vertex c = new( endPos + size, Vector3.Up, Vector3.Right, new Vector4( 1, 0, 0, 0 ) );
		Vertex d = new( endPos - size, Vector3.Up, Vector3.Right, new Vector4( 0, 0, 0, 0 ) );

		vertexBuffer.Add( a );
		vertexBuffer.Add( b );
		vertexBuffer.Add( c );
		vertexBuffer.Add( d );

		vertexBuffer.AddTriangleIndex( 4, 3, 2 );
		vertexBuffer.AddTriangleIndex( 2, 1, 4 );

		// Add the arrow tip
		Vertex e = new( endPos + size * 1.75f, Vector3.Up, Vector3.Right, new Vector4( 1, 0, 0, 0 ) );
		Vertex f = new( endPos - size * 1.75f, Vector3.Up, Vector3.Right, new Vector4( 0, 0, 0, 0 ) );
		Vertex g = new( endPos + direction * 8, Vector3.Up, Vector3.Right, new Vector4( 1, 0, 0, 0 ) );

		vertexBuffer.Add( e );
		vertexBuffer.Add( f );
		vertexBuffer.Add( g );
		vertexBuffer.AddTriangleIndex( 1, 2, 3 );

		Graphics.Attributes.Set( "color", color );

		vertexBuffer.Draw( Material );
	}

	public override void RenderSceneObject()
	{
		if ( Power.AlmostEqual( 0.0f ) )
			return;

		Graphics.SetupLighting( this );

		var startPos = Position;
		var endPos = Position + Direction * Power * 100;
		var size = Vector3.Cross( Direction, Vector3.Up ) * 3f;

		// var color = ColorConvert.HSLToRGB( 120 - (int)(Power * Power * 120), 1.0f, 0.5f );
		DrawArrow( startPos, endPos, Direction, size, Color.Red );
	}
}
