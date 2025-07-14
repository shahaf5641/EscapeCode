using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class VideoController : MonoBehaviour
{
    [Header("🎥 Components")]
    public VideoPlayer videoPlayer;
    public RawImage videoDisplay;
    public Slider progressSlider;
    public Button playPauseButton;
    public Button skipButton;

    [Header("🎨 Icons")]
    public Sprite playIcon;
    public Sprite pauseIcon;

    [Header("🔄 Scene Settings")]
    public string nextSceneName;

    private bool isDragging = false;

    void Start()
    {
        // Use value from VideoSession if not set manually
        if (string.IsNullOrEmpty(nextSceneName) && !string.IsNullOrEmpty(VideoSession.nextSceneAfterVideo))
        {
            nextSceneName = VideoSession.nextSceneAfterVideo;
        }

        if (videoPlayer == null || videoDisplay == null || progressSlider == null || playPauseButton == null || skipButton == null)
        {
            Debug.LogError("Assign all references in the inspector!");
            enabled = false;
            return;
        }

        // Setup RenderTexture
        RenderTexture renderTexture = new RenderTexture(1920, 1080, 0);
        videoPlayer.targetTexture = renderTexture;
        videoDisplay.texture = renderTexture;

        // Events
        videoPlayer.loopPointReached += OnVideoEnd;
        videoPlayer.Play();
        UpdatePlayPauseIcon();

        // Button listeners
        playPauseButton.onClick.AddListener(TogglePlayPause);
        skipButton.onClick.AddListener(SkipVideo);
        progressSlider.onValueChanged.AddListener(OnSliderChanged);
    }

    void Update()
    {
        if (videoPlayer.isPlaying && videoPlayer.length > 0 && !isDragging)
        {
            progressSlider.value = (float)(videoPlayer.time / videoPlayer.length);
        }
    }

    void TogglePlayPause()
    {
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
        }
        else
        {
            videoPlayer.Play();
        }
        UpdatePlayPauseIcon();
    }

    void UpdatePlayPauseIcon()
    {
        Image iconImage = playPauseButton.GetComponent<Image>();
        if (iconImage != null)
        {
            iconImage.sprite = videoPlayer.isPlaying ? pauseIcon : playIcon;
        }
    }

    void OnSliderChanged(float value)
    {
        if (!videoPlayer.canSetTime || videoPlayer.length <= 0) return;

        isDragging = true;
        videoPlayer.time = value * videoPlayer.length;
        isDragging = false;
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        UpdatePlayPauseIcon();
    }

    void SkipVideo()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("Next scene name not set.");
        }
    }
}
