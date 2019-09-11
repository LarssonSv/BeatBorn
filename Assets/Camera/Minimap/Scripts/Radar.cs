#pragma warning disable 0649
using UnityEngine;
using UnityEngine.UI;

public class Radar : MonoBehaviour {
    [Header("Setup:")]
    [SerializeField, Tooltip("Scan Radius")] private float radius;
    private LayerMask mask = 1 << 12;

    [SerializeField] private Image radarImage;
    private int textureSize = 128;
    private int pixelOffset = 20;

    private Vector3[] radarPos = new Vector3[0];

    private void Start() {
        //pixelColorsDefault = radarImage.texture.GetPixels();
        DisplayEnemiesUI();
    }

    private void FixedUpdate() {
        Collider[] found = Physics.OverlapSphere(transform.position, radius, mask, QueryTriggerInteraction.Ignore);
        radarPos = new Vector3[found.Length];

        for(int i = 0; i < found.Length; i++) {
            radarPos[i] = found[i].transform.InverseTransformPoint(transform.position);
        }

        DisplayEnemiesUI();
    }

    private void DisplayEnemiesUI() {
        float q = textureSize / 3;

        float r = q * q + q * q;

        Texture2D texture = new Texture2D(textureSize, textureSize);
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 128, 128), Vector2.zero);
        radarImage.sprite = sprite;

        for(int y = 0; y < texture.height; y++) {
            for(int x = 0; x < texture.width; x++) {
                Color pixelColour;
                if(/*x - pixelSize / 2 == q && x + pixelSize / 2 >= q && */x - pixelOffset == q && y - pixelOffset == q) {
                    pixelColour = new Color(1, 0, 1, 1);
                    texture.SetPixel(x, y, pixelColour);
                    continue;
                }
                float d = Mathf.Pow(x - q - pixelOffset, 2) + Mathf.Pow(y - q - pixelOffset, 2);
                if(d >= r) {
                    pixelColour = new Color(1, 1, 1, 0); //Outside of the circle
                } else {
                    pixelColour = new Color(0, 0, 0, 1);
                }
                texture.SetPixel(x, y, pixelColour);
            }
        }
        texture.Apply();
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue;
        foreach(Vector3 x in radarPos) {
            Gizmos.DrawCube(x, Vector3.one);
        }
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}