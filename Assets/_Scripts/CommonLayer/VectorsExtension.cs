using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

// ReSharper disable CompareOfFloatsByEqualityOperator

namespace CommonLayer
{
  public static class VectorsExtension
  {
    private const float SMALL_VALUE = 0.01F;
    private const float ZERO = 0;
    private static readonly Color _emptyColor = new Color();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsZero(this float4 value) => math.abs(value.x) < SMALL_VALUE &&
                                                    math.abs(value.y) < SMALL_VALUE &&
                                                    math.abs(value.z) < SMALL_VALUE &&
                                                    math.abs(value.w) < SMALL_VALUE;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPreciselyZero(this float4 value) => value.x == 0 &&
                                                             value.y == 0 &&
                                                             value.z == 0 &&
                                                             value.w == 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsZero(this float3 value) => math.abs(value.x) < SMALL_VALUE &&
                                                    math.abs(value.y) < SMALL_VALUE &&
                                                    math.abs(value.z) < SMALL_VALUE;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPreciselyZero(this float3 value) => value.x == 0 &&
                                                             value.y == 0 &&
                                                             value.z == 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsZero(this float2 value) => math.abs(value.x) < SMALL_VALUE &&
                                                    math.abs(value.y) < SMALL_VALUE;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPreciselyZero(this float2 value) => value.x == 0 &&
                                                             value.y == 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsZero(this Vector3 vector) => math.abs(vector.x) < SMALL_VALUE &&
                                                                  math.abs(vector.y) < SMALL_VALUE &&
                                                                  math.abs(vector.z) < SMALL_VALUE;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPreciselyZero(this Vector3 vector) => vector.x == ZERO &&
                                                                           vector.y == ZERO &&
                                                                           vector.z == ZERO;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsZero(this Vector2 vector) => math.abs(vector.x) < SMALL_VALUE &&
                                                                  math.abs(vector.y) < SMALL_VALUE;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPreciselyZero(this Vector2 vector) => vector.x == ZERO &&
                                                                           vector.y == ZERO;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEmpty(this Color color) => color == _emptyColor;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEmpty(this Color32 color) => color == _emptyColor;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float3 Normalize(this float3 value) => math.normalize(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Magnitude(this float3 value) => math.sqrt(math.pow(value.x, 2) + math.pow(value.y, 2) + math.pow(value.z, 2));
  }
}
