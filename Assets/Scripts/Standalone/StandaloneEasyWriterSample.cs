using UnityEngine;
using ZXing;
using ZXing.QrCode;

public class StandaloneEasyWriterSample : MonoBehaviour {

    [SerializeField]
    private string lastResult;
    [SerializeField]
    private Texture2D encoded;

    private IBarcodeWriter writer;
    private Color32[] generatedColorData;

    private void Start() {
        lastResult = "http://www.google.com";

        encoded = new Texture2D(256, 256);
        generatedColorData = new Color32[256 * 256];

        writer = new BarcodeWriter {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions {
                Height = encoded.height,
                Width = encoded.width
            }
        };
    }

    private void Update() {
        // encoding from last result
        generatedColorData = writer.Write(lastResult); // -> performance heavy method
        encoded.SetPixels32(generatedColorData); // -> performance heavy method
        encoded.Apply();
    }

    private void OnGUI() {
        // show encoded QR code on screen
        GUI.DrawTexture(new Rect(10, 10, 256, 256), encoded, ScaleMode.ScaleToFit);
    }
}
