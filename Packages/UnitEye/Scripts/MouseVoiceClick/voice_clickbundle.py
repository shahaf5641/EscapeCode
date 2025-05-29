from faster_whisper import WhisperModel

# This will download the model to cache
model = WhisperModel("tiny", download_root="whisper_models")
