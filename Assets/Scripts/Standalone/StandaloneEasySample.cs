using UnityEngine;
using ZXing;
using ZXing.QrCode;

public class StandaloneEasySample : MonoBehaviour {

    [SerializeField]
    private string lastResult;
    [SerializeField]
    private Texture2D encoded;

    private WebCamTexture camTexture;
    private Color32[] cameraColorData;
    private Color32[] generatedColorData;
    private int width, height;
    private Rect screenRect;

    // create a reader with a custom luminance source
    private BarcodeReader barcodeReader = new BarcodeReader {
        AutoRotate = false,
        Options = new ZXing.Common.DecodingOptions {
            TryHarder = false
        }
    };

    private BarcodeWriter writer;
    private Result result;

    private void Start() {
        SetupWebcamTexture();
        PlayWebcamTexture();

        lastResult = "http://www.google.com";

        cameraColorData = new Color32[width * height];
        screenRect = new Rect(0, 0, Screen.width, Screen.height);

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

    private void OnEnable() {
        PlayWebcamTexture();
    }

    private void OnDisable() {
        if (camTexture != null) {
            camTexture.Pause();
        }
    }

    private void Update() {
        // decoding from camera image
        camTexture.GetPixels32(cameraColorData); // -> performance heavy method 
        result = barcodeReader.Decode(cameraColorData, width, height); // -> performance heavy method
        if (result != null) {
            lastResult = result.Text + " " + result.BarcodeFormat;
            print(lastResult);
        }

        // encoding from last result
        generatedColorData = writer.Write(lastResult); // -> performance heavy method
        encoded.SetPixels32(generatedColorData); // -> performance heavy method
        encoded.Apply();
    }

    private void OnGUI() {
        // show camera image on screen
        GUI.DrawTexture(screenRect, camTexture, ScaleMode.ScaleToFit);
        // show encoded QR code on screen
        GUI.DrawTexture(new Rect(10, 10, 256, 256), encoded, ScaleMode.ScaleToFit);
        // show decoded text on screen
        GUI.TextField(new Rect(10, 10, 256, 25), lastResult);
    }

    private void OnDestroy() {
        camTexture.Stop();
    }

    private void SetupWebcamTexture() {
        camTexture = new WebCamTexture();
        camTexture.requestedHeight = Screen.height;
        camTexture.requestedWidth = Screen.width;
    }

    private void PlayWebcamTexture() {
        if (camTexture != null) {
            camTexture.Play();
            width = camTexture.width;
            height = camTexture.height;
        }
    }
}
