using UnityEngine;

public static class ExtensionMethods {
    private static float KINDA_SMALL_NUMBER = Mathf.Epsilon;
    public static bool NearZero(this Vector3 v) {
        return v.x <= KINDA_SMALL_NUMBER && v.x >= -KINDA_SMALL_NUMBER &&
            v.y <= KINDA_SMALL_NUMBER && v.y >= -KINDA_SMALL_NUMBER &&
            v.z <= KINDA_SMALL_NUMBER && v.z >= -KINDA_SMALL_NUMBER;
    }
}