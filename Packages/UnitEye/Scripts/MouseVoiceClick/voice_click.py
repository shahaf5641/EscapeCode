import sounddevice as sd
import numpy as np
import pyautogui
from faster_whisper import WhisperModel

model = WhisperModel("whisper_models/tiny", compute_type="int8")

SAMPLERATE = 16000
CHUNK_DURATION = 1.0
CHUNK_SIZE = int(SAMPLERATE * CHUNK_DURATION)

def record_chunk():
    audio = sd.rec(CHUNK_SIZE, samplerate=SAMPLERATE, channels=1, dtype='float32')
    sd.wait()
    return np.squeeze(audio)

while True:
    audio_chunk = record_chunk()
    segments, _ = model.transcribe(audio_chunk, language="en")

    for segment in segments:
        text = segment.text.strip().lower()
        if text in [
            "click",
            "Click",
            "click.",
            "Click.",
            "click!",
            "Click!",
            "click?",
            "Click?",
            "clik",        # common misspell
            "Clik",
            "cliq",        # rare but possible
            "Klik",        # phonetic variation
            "click click", # echo or stutter
            "clickk",      # repetition
            "click .",     # space + dot
            "click .",     # odd formatting
            "clic",        # missing 'k'
            "click,",      # comma
            "Click,",      
            "clickk.",     # extra k + punctuation
            "click "       # with trailing space
        ]:
            pyautogui.click()