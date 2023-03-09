using UnityEngine;

public class CameraResolutionAdaptDevices : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] bool fixedWidth, fixedHeight;

    [Tooltip("standard screen size")]
    [SerializeField] Vector2 standardScreen = new Vector2(1080, 1920);

    [Tooltip("standard camera orthographic size")]
    [SerializeField] float standardOrthographicSize = 9.6f;

    float widthScreen, hightScreen;
    float pixelPerUnitWorld;

    public float WidthScreen { get => widthScreen; }
    public float HightScreen { get => hightScreen; }
    public float PixelPerUnitWorld { get => pixelPerUnitWorld; }

    public float LeftView
    {
        get => transform.position.x - widthScreen / 2;
        set { transform.position = new Vector3(value + widthScreen / 2, transform.position.y, transform.position.z); }
    }

    public float RightView
    {
        get => transform.position.x + widthScreen / 2;
        set { transform.position = new Vector3(value - widthScreen / 2, transform.position.y, transform.position.z); }
    }

    private void Awake()
    {
        if (cam == null)
            cam = Camera.main;

        if (fixedHeight && fixedWidth)
        {
            Debug.LogError("The screen is only fixed by 1 dimension, hight will be taken by default");
            fixedWidth = false;
        }

        if (fixedHeight)
        {
            hightScreen = cam.orthographicSize * 2;
            pixelPerUnitWorld = Screen.height / 2 / cam.orthographicSize;
            widthScreen = Screen.width / pixelPerUnitWorld;

        }

        if (fixedWidth)
        {
            float standarPixelPerUnitWorld = standardScreen.y / 2 / standardOrthographicSize;
            widthScreen = standardScreen.x / standarPixelPerUnitWorld;
            pixelPerUnitWorld = Screen.width / widthScreen;
            hightScreen = Screen.height / pixelPerUnitWorld;
            cam.orthographicSize = hightScreen / 2;
        }
    }
}
