using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class TutorialVideoManager : MonoBehaviour
{
    public VideoPlayer playerA;
    public VideoPlayer playerB;
    public RawImage videoImage;
    public Slider progressSlider;
    public Button playPauseBtn, nextBtn, prevBtn;
    public Sprite playIcon, pauseIcon;
    public VideoClip[] tutorialClips;
    public AudioSource audioSourceA;
    public AudioSource audioSourceB;

    private RenderTexture rtA, rtB;
    private int currentIndex = 0;
    private bool isUsingA = true;
    private bool isDragging = false;

    void Start()
    {
        if (TutorialVideoConfig.clipsToPlay == null || TutorialVideoConfig.clipsToPlay.Length == 0)
        {
            Debug.LogError("No clips assigned! Skipping to next scene.");
            SceneManager.LoadScene(TutorialVideoConfig.nextSceneName);
            return;
        }

        tutorialClips = TutorialVideoConfig.clipsToPlay;

        rtA = new RenderTexture(1920, 1080, 0);
        rtB = new RenderTexture(1920, 1080, 0);

        SetupPlayer(playerA, rtA, audioSourceA);
        SetupPlayer(playerB, rtB, audioSourceB);

        videoImage.texture = rtA;
        SetupButtons();

        PlayClip(GetCurrentPlayer(), currentIndex, () =>
        {
            PreloadNext();
        });
    }

    void Update()
    {
        VideoPlayer currentPlayer = GetCurrentPlayer();

        if (currentPlayer.isPlaying && !isDragging)
        {
            progressSlider.value = (float)(currentPlayer.time / currentPlayer.length);
        }

        if (currentPlayer.frame > 0 && currentPlayer.isPrepared && !currentPlayer.isPlaying && currentPlayer.time >= currentPlayer.length)
        {
            GoToNextClip();
        }
    }

    void SetupPlayer(VideoPlayer vp, RenderTexture rt, AudioSource audioSource)
    {
        vp.renderMode = VideoRenderMode.RenderTexture;
        vp.targetTexture = rt;
        vp.audioOutputMode = VideoAudioOutputMode.AudioSource;
        vp.SetTargetAudioSource(0, audioSource);
        vp.skipOnDrop = true;
    }

    void SetupButtons()
    {
        playPauseBtn.onClick.AddListener(TogglePlayPause);
        nextBtn.onClick.AddListener(GoToNextClip);
        //prevBtn.onClick.AddListener(GoToPreviousClip);
        progressSlider.onValueChanged.AddListener(OnSliderChanged);
    }

    void PlayClip(VideoPlayer player, int clipIndex, System.Action onPrepared = null)
    {
        if (clipIndex < 0 || clipIndex >= tutorialClips.Length)
        {
            EndTutorial();
            return;
        }

        player.Stop();
        player.clip = tutorialClips[clipIndex];
        player.time = 0;

        player.prepareCompleted -= OnVideoPrepared;
        player.prepareCompleted += OnVideoPrepared;

        player.Prepare();

        void OnVideoPrepared(VideoPlayer vp)
        {
            vp.prepareCompleted -= OnVideoPrepared;
            vp.Play();

            audioSourceA.mute = !isUsingA;
            audioSourceB.mute = isUsingA;

            playPauseBtn.image.sprite = pauseIcon;
            onPrepared?.Invoke();
        }
    }

    void PreloadNext()
    {
        if (currentIndex + 1 >= tutorialClips.Length)
            return;

        var preloadPlayer = GetInactivePlayer();
        preloadPlayer.Stop();
        preloadPlayer.clip = tutorialClips[currentIndex + 1];
        preloadPlayer.Prepare();
    }

    void GoToNextClip()
    {
        currentIndex++;
        if (currentIndex >= tutorialClips.Length)
        {
            EndTutorial();
            return;
        }

        isUsingA = !isUsingA;
        videoImage.texture = isUsingA ? rtA : rtB;

        audioSourceA.mute = !isUsingA;
        audioSourceB.mute = isUsingA;

        PlayClip(GetCurrentPlayer(), currentIndex, () =>
        {
            PreloadNext();
        });
    }


    void TogglePlayPause()
    {
        var currentPlayer = GetCurrentPlayer();
        var currentAudio = isUsingA ? audioSourceA : audioSourceB;

        if (currentPlayer.isPlaying)
        {
            currentPlayer.Pause();
            currentAudio.Pause();
            playPauseBtn.image.sprite = playIcon;
        }
        else
        {
            currentPlayer.Play();
            currentAudio.Play();
            playPauseBtn.image.sprite = pauseIcon;
        }
    }

    void OnSliderChanged(float value)
    {
        isDragging = true;
        var currentPlayer = GetCurrentPlayer();
        currentPlayer.time = value * currentPlayer.length;
        isDragging = false;
    }

    VideoPlayer GetCurrentPlayer()
    {
        return isUsingA ? playerA : playerB;
    }

    VideoPlayer GetInactivePlayer()
    {
        return isUsingA ? playerB : playerA;
    }

    void EndTutorial()
    {
        SceneManager.LoadScene(TutorialVideoConfig.nextSceneName);
    }
}