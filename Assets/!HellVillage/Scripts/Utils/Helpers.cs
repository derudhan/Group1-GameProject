using UnityEngine;

namespace HellVillage.Utils {
    /// <summary>
    /// This class contains utility methods for various common operations.
    /// </summary>
    public static class Helpers {
        /// <summary>
        /// Map a value from one range to another.
        /// This is useful for remapping values, such as from a slider to a specific range.
        /// If `clamp` is true, the value will be clamped to the new range.
        /// If `clamp` is false, the value will not be clamped and can exceed the new range.
        /// </summary>
        public static float Map(float value, float originalMin, float originalMax, float newMin, float newMax, bool clamp = false) {
            float newValue = (value - originalMin) / (originalMax - originalMin) * (newMax - newMin) + newMin;
            if (clamp) {
                newValue = Mathf.Clamp(newValue, newMin, newMax);
            }
            return newValue;
        }

        /// <summary>
        /// Set text with padding.
        /// </summary>
        public static string SetTextWithPadding(string text, int paddingSpaces, string paddingType = " ") {
            for (int i = 0; i < paddingSpaces; i++) {
                text = paddingType + text + paddingType;
            }
            return text;
        }

        /// <summary>
        /// Convert a linear value to decibels.
        /// </summary>
        public static float DecibelToLinear(float decibel) {
            return Mathf.Pow(10f, decibel / 20f);
        }

        /// <summary>
        /// Convert a linear value to decibels.
        /// </summary>
        public static float LinearToDecibel(float linear) {
            return 20f * Mathf.Log10(linear);
        }
    }
}
