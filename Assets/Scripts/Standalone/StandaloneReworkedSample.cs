using System.Threading;
using UnityEngine;
using ZXing;
using ZXing.QrCode;

public class StandaloneReworkedSample : MonoBehaviour {

    private const int encodingWidth = 256;

    [SerializeField]
    private string lastResult;
    [SerializeField]
    private Texture2D encoded;

    private WebCamTexture camTexture;
    private Color32[] cameraColorData;
    private int width, height;
    private Rect screenRect;

    // Create the token source.
    private CancellationTokenSource cts = new CancellationTokenSource();

    // create a reader with a custom luminance source
    private BarcodeReader barcodeReader = new BarcodeReader {
        AutoRotate = false,
        Options = new ZXing.Common.DecodingOptions {
            TryHarder = false
        }
    };

    private BarcodeWriter writer;
    private Result result;

    private Color32[] generatedColorData;
    private bool startEncoding;
    private bool startDecoding;

    private void Start() {
        SetupWebcamTexture();
        PlayWebcamTexture();

        lastResult = "http://www.google.com";

        cameraColorData = new Color32[width * height];
        screenRect = new Rect(0, 0, Screen.width, Screen.height);

        encoded = new Texture2D(encodingWidth, encodingWidth);
        generatedColorData = new Color32[encodingWidth * encodingWidth];

        writer = new BarcodeWriter {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions {
                Height = encoded.height,
                Width = encoded.width
            }
        };

        // Pass the token to the cancelable operation - decoding and encoding.
        ThreadPool.QueueUserWorkItem(new WaitCallback(GetCodeFromImageData), cts.Token);
        ThreadPool.QueueUserWorkItem(new WaitCallback(EncodeNewFromLastResult), cts.Token);
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
        if (!startDecoding) {
            camTexture.GetPixels32(cameraColorData);

            startDecoding = !startDecoding;
        }

        if (!startEncoding && generatedColorData != null) {
            encoded.SetPixels32(generatedColorData);
            encoded.Apply();

            startEncoding = !startEncoding;
        }
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

        cts.Cancel();
        // Cancellation should have happened, so call Dispose.
        cts.Dispose();
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

    private void GetCodeFromImageData(object obj) {
        CancellationToken token = (CancellationToken)obj;

        while (!token.IsCancellationRequested) {
            // decode the current frame
            if (startDecoding && cameraColorData != null) {
                result = barcodeReader.Decode(cameraColorData, width, height);
                if (result != null) {
                    lastResult = result.Text + " " + result.BarcodeFormat;
                    print(lastResult);
                    startEncoding = true;
                }
                startDecoding = !startDecoding;
            }
        }
    }

    private void EncodeNewFromLastResult(object obj) {
        CancellationToken token = (CancellationToken)obj;

        while (!token.IsCancellationRequested) {
            if (startEncoding && lastResult != null) {
                generatedColorData = writer.Write(lastResult);
                startEncoding = !startEncoding;
            }
        }
    }
}
