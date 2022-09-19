using TMPro;
using UnityEngine;
using ZXing;

public class HololensEasyReaderSample : MonoBehaviour {

    [SerializeField]
    private TextMeshProUGUI resultText;
    [SerializeField]
    private string lastResult = "";

    private WebCamTexture camTexture;
    private Color32[] cameraColorData;
    private int width, height;

    // create a reader with a custom luminance source
    private IBarcodeReader barcodeReader = new BarcodeReader {
        AutoRotate = false,
        Options = new ZXing.Common.DecodingOptions {
            TryHarder = false
        }
    };

    private Result result;

    private void Start() {
        SetupWebcamTexture();
        PlayWebcamTexture();

        cameraColorData = new Color32[width * height];
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
            resultText.text = lastResult;
            print(lastResult);
        }
    }

    private void OnDestroy() {
        camTexture.Stop();
    }

    private void SetupWebcamTexture() {
        camTexture = new WebCamTexture(1920, 1080, 30);
    }

    private void PlayWebcamTexture() {
        if (camTexture != null) {
            camTexture.Play();
            width = camTexture.width;
            height = camTexture.height;
        }
    }
}
