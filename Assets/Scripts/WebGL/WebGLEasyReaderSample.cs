using System.Collections;
using UnityEngine;
using ZXing;

public class WebGLEasyReaderSample : MonoBehaviour {

    [SerializeField]
    private string lastResult = "";

    private WebCamTexture camTexture;
    private Color32[] cameraColorData;
    private Rect screenRect;

    private bool finalWebcamSetup;

    // create a reader with a custom luminance source
    private IBarcodeReader barcodeReader = new BarcodeReader {
        AutoRotate = false,
        Options = new ZXing.Common.DecodingOptions {
            TryHarder = false
        }
    };

    private Result result;

    // Use this for initialization
    IEnumerator Start() {
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        if (Application.HasUserAuthorization(UserAuthorization.WebCam)) {
            Debug.Log("webcam found");
            SetupWebcamTexture();
            PlayWebcamTexture();
        } else {
            Debug.Log("no webcams found");
        }
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
        if (camTexture != null && camTexture.isPlaying) {
            // waiting for the browser to finally acknowledge the webcam
            if (camTexture.width < 100 && camTexture.height < 100) {
                return;
            }

            if (!finalWebcamSetup) {
                cameraColorData = new Color32[camTexture.width * camTexture.height];
                screenRect = new Rect(0, 0, camTexture.width, camTexture.height);
                finalWebcamSetup = !finalWebcamSetup;
            }

            camTexture.GetPixels32(cameraColorData); // -> performance heavy method 
            result = barcodeReader.Decode(cameraColorData, camTexture.width, camTexture.height); // -> performance heavy method
            if (result != null) {
                lastResult = result.Text + " " + result.BarcodeFormat;
                print(lastResult);
            }
        }
    }

    private void OnGUI() {
        if (finalWebcamSetup) {
            // show camera image on screen
            GUI.DrawTexture(screenRect, camTexture, ScaleMode.ScaleToFit);
            // show decoded text on screen
            GUI.TextField(new Rect(10, 10, 256, 25), lastResult);
        }
    }

    private void OnDestroy() {
        camTexture.Stop();
    }

    private void SetupWebcamTexture() {
        camTexture = new WebCamTexture();
    }

    private void PlayWebcamTexture() {
        if (camTexture != null) {
            camTexture.Play();
        }
    }
}
