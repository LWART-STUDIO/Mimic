// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)
// Do not manually edit - this file is generated by MathfsCodegen.cs

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Freya {

	/// <summary>An optimized uniform 1D Cubic catmull-rom segment, with 4 control points</summary>
	[Serializable] public struct CatRomCubic1D : IParamSplineSegment<Polynomial,Matrix4x1> {

		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

		[SerializeField] Matrix4x1 pointMatrix;
		[NonSerialized] Polynomial curve;
		[NonSerialized] bool validCoefficients;

		/// <summary>Creates a uniform 1D Cubic catmull-rom segment, from 4 control points</summary>
		/// <param name="p0">The first control point of the catmull-rom curve. Note that this point is not included in the curve itself, and only helps to shape it</param>
		/// <param name="p1">The second control point, and the start of the catmull-rom curve</param>
		/// <param name="p2">The third control point, and the end of the catmull-rom curve</param>
		/// <param name="p3">The last control point of the catmull-rom curve. Note that this point is not included in the curve itself, and only helps to shape it</param>
		public CatRomCubic1D( float p0, float p1, float p2, float p3 ) : this(new Matrix4x1(p0, p1, p2, p3)){}
		/// <summary>Creates a uniform 1D Cubic catmull-rom segment, from 4 control points</summary>
		/// <param name="pointMatrix">The matrix containing the control points of this spline</param>
		public CatRomCubic1D( Matrix4x1 pointMatrix ) => (this.pointMatrix,curve,validCoefficients) = (pointMatrix,default,false);

		public Polynomial Curve {
			get {
				if( validCoefficients )
					return curve; // no need to update
				validCoefficients = true;
				return curve = new Polynomial(
					P1,
					(-P0+P2)/2,
					P0-(5/2f)*P1+2*P2-(1/2f)*P3,
					-(1/2f)*P0+(3/2f)*P1-(3/2f)*P2+(1/2f)*P3
				);
			}
		}
		public Matrix4x1 PointMatrix {[MethodImpl( INLINE )] get => pointMatrix; [MethodImpl( INLINE )] set => _ = ( pointMatrix = value, validCoefficients = false ); }
		/// <summary>The first control point of the catmull-rom curve. Note that this point is not included in the curve itself, and only helps to shape it</summary>
		public float P0{ [MethodImpl( INLINE )] get => pointMatrix.m0; [MethodImpl( INLINE )] set => _ = ( pointMatrix.m0 = value, validCoefficients = false ); }
		/// <summary>The second control point, and the start of the catmull-rom curve</summary>
		public float P1{ [MethodImpl( INLINE )] get => pointMatrix.m1; [MethodImpl( INLINE )] set => _ = ( pointMatrix.m1 = value, validCoefficients = false ); }
		/// <summary>The third control point, and the end of the catmull-rom curve</summary>
		public float P2{ [MethodImpl( INLINE )] get => pointMatrix.m2; [MethodImpl( INLINE )] set => _ = ( pointMatrix.m2 = value, validCoefficients = false ); }
		/// <summary>The last control point of the catmull-rom curve. Note that this point is not included in the curve itself, and only helps to shape it</summary>
		public float P3{ [MethodImpl( INLINE )] get => pointMatrix.m3; [MethodImpl( INLINE )] set => _ = ( pointMatrix.m3 = value, validCoefficients = false ); }
		/// <summary>Get or set a control point position by index. Valid indices from 0 to 3</summary>
		public float this[ int i ] {
			get => i switch { 0 => P0, 1 => P1, 2 => P2, 3 => P3, _ => throw new ArgumentOutOfRangeException( nameof(i), $"Index has to be in the 0 to 3 range, and I think {i} is outside that range you know" ) };
			set { switch( i ){ case 0: P0 = value; break; case 1: P1 = value; break; case 2: P2 = value; break; case 3: P3 = value; break; default: throw new ArgumentOutOfRangeException( nameof(i), $"Index has to be in the 0 to 3 range, and I think {i} is outside that range you know" ); }}
		}
		public static bool operator ==( CatRomCubic1D a, CatRomCubic1D b ) => a.pointMatrix == b.pointMatrix;
		public static bool operator !=( CatRomCubic1D a, CatRomCubic1D b ) => !( a == b );
		public bool Equals( CatRomCubic1D other ) => P0.Equals( other.P0 ) && P1.Equals( other.P1 ) && P2.Equals( other.P2 ) && P3.Equals( other.P3 );
		public override bool Equals( object obj ) => obj is CatRomCubic1D other && pointMatrix.Equals( other.pointMatrix );
		public override int GetHashCode() => pointMatrix.GetHashCode();
		public override string ToString() => $"({pointMatrix.m0}, {pointMatrix.m1}, {pointMatrix.m2}, {pointMatrix.m3})";

		public static explicit operator BezierCubic1D( CatRomCubic1D s ) =>
			new BezierCubic1D(
				s.P1,
				-(1/6f)*s.P0+s.P1+(1/6f)*s.P2,
				(1/6f)*s.P1+s.P2-(1/6f)*s.P3,
				s.P2
			);
		public static explicit operator HermiteCubic1D( CatRomCubic1D s ) =>
			new HermiteCubic1D(
				s.P1,
				(-s.P0+s.P2)/2,
				s.P2,
				(-s.P1+s.P3)/2
			);
		public static explicit operator UBSCubic1D( CatRomCubic1D s ) =>
			new UBSCubic1D(
				(7/6f)*s.P0-(2/3f)*s.P1+(5/6f)*s.P2-(1/3f)*s.P3,
				-(1/3f)*s.P0+(11/6f)*s.P1-(2/3f)*s.P2+(1/6f)*s.P3,
				(1/6f)*s.P0-(2/3f)*s.P1+(11/6f)*s.P2-(1/3f)*s.P3,
				-(1/3f)*s.P0+(5/6f)*s.P1-(2/3f)*s.P2+(7/6f)*s.P3
			);
		/// <summary>Returns a linear blend between two catmull-rom curves</summary>
		/// <param name="a">The first spline segment</param>
		/// <param name="b">The second spline segment</param>
		/// <param name="t">A value from 0 to 1 to blend between <c>a</c> and <c>b</c></param>
		public static CatRomCubic1D Lerp( CatRomCubic1D a, CatRomCubic1D b, float t ) =>
			new(
				Mathfs.Lerp( a.P0, b.P0, t ),
				Mathfs.Lerp( a.P1, b.P1, t ),
				Mathfs.Lerp( a.P2, b.P2, t ),
				Mathfs.Lerp( a.P3, b.P3, t )
			);
	}
}
