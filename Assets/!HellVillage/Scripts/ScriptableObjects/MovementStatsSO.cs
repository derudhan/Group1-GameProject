using UnityEngine;

namespace HellVillage {
    [CreateAssetMenu(menuName = "Movement Stats")]
    public class MovementStats : ScriptableObject {
        [Header("Walk")]
        [Range(1f, 100f)] public float MaxWalkSpeed = 5f;
        [Range(0.25f, 50f)] public float Acceleration = 2f;
        [Range(0.25f, 50f)] public float Deceleration = 10f;

        [Header("Run")]
        [Range(1f, 100f)] public float MaxRunSpeed = 10f;

        [Header("Dash")]
        public bool CanDash = false;
        [Range(0f, 1f)] public float DashTime = 0.11f;
        [Range(1f, 200f)] public float DashSpeed = 40f;
        [Range(0, 5)] public int NumberOfDashes = 1;
        public readonly Vector2[] DashDirections = new Vector2[]
        {
            new(0, 0), // In Place
            new(0, 1), // Top
            new(1, 0), // Right
            new(0, -1), // Down
            new(-1, 0), // Left
            new Vector2(-1, 1).normalized, // Left-Top
            new Vector2(1, 1).normalized, // Top-Right
            new Vector2(1, -1).normalized, // Right-Down
            new Vector2(-1, -1).normalized, // Bottom-Left
        };
    }
}
