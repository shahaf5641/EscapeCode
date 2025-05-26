using MediaPipe.Holistic;
using Unity.Mathematics;
using UnityEngine;

namespace UnitEye
{
    public class Functions
    {
        // Buffer Texture2D to avoid memory leak
        private static Texture2D _flipTextureBuffer = new Texture2D(256, 256);

        /// <summary>
        /// Horizontally flips a Texture that can be converted into a Texture2D (not a RenderTexture)
        /// </summary>
        public static Texture FlipTexture(Texture source)
        {
            if (source.width == 0 || source.height == 0)
            {
                _flipTextureBuffer.Reinitialize(1, 1);
                return _flipTextureBuffer;
            }

            var sourceWidth = source.width;
            var sourceHeight = source.height;

            if (_flipTextureBuffer == null || !_flipTextureBuffer)
                _flipTextureBuffer = new Texture2D(sourceWidth, sourceHeight);

            _flipTextureBuffer.Reinitialize(sourceWidth, sourceHeight);

            var pixelArray = ((Texture2D)source).GetPixels32();

            for (int i = 0; i < sourceHeight; i++)
                System.Array.Reverse(pixelArray, i * sourceWidth, sourceWidth);

            _flipTextureBuffer.SetPixels32(pixelArray);
            _flipTextureBuffer.Apply();
            return _flipTextureBuffer;
        }

        // Buffer Texture2D to avoid memory leak
        private static Texture2D _getEyeTextureBuffer = new Texture2D(256, 256);

        /// <summary>
        /// Calculates EyeCrops similar to EyeMU and returns them as a Texture
        /// </summary>
        public static Texture GetEyeTexture(HolisticPipeline holisticPipeline, WebCamTexture source, int leftVertex, int rightVertex, int imageSize = 128)
        {
            float2 leftCorner = holisticPipeline.facePipeline.GetFaceRegionVertex(leftVertex).xy;
            float2 rightCorner = holisticPipeline.facePipeline.GetFaceRegionVertex(rightVertex).xy;

            int cropSize = imageSize;
            int leftX = 0;
            int yBot = 0;

            float scaleFactor = (float)source.height / source.width;
            float eyeLength = leftCorner.x - rightCorner.x;
            float xShift = eyeLength * 0.2f;
            eyeLength += 2f * xShift;
            float yShift = eyeLength * 0.5f;
            float yRef = (leftCorner.y + rightCorner.y) * 0.5f;

            yRef -= yShift;
            yRef = (yRef - ((1f - scaleFactor) * 0.5f)) / scaleFactor;
            yRef = Mathf.Clamp(yRef, 0.0f, 1.0f);

            cropSize = (int)(eyeLength * source.width);
            leftX = (int)((rightCorner.x - xShift) * source.width);
            yBot = (int)(yRef * source.height);

            if (source != null && cropSize > 0 &&
                leftX >= 0 && leftX <= source.width - cropSize &&
                yBot >= 0 && yBot <= source.height - cropSize)
            {
                if (_getEyeTextureBuffer == null || !_getEyeTextureBuffer)
                    _getEyeTextureBuffer = new Texture2D(cropSize, cropSize);
                else
                    _getEyeTextureBuffer.Reinitialize(cropSize, cropSize);

                _getEyeTextureBuffer.SetPixels(source.GetPixels(leftX, yBot, cropSize, cropSize));
                _getEyeTextureBuffer.Apply();
            }

            return _getEyeTextureBuffer;
        }

        /// <summary>
        /// Converts pixels to mm using Unity Screen.dpi
        /// </summary>
        public static float PixelsToMm(float pixels)
        {
            return PixelsToMm(pixels, Screen.dpi);
        }

        /// <summary>
        /// Converts pixels to mm using custom dpi
        /// </summary>
        public static float PixelsToMm(float pixels, float dpi)
        {
            return pixels * 25.4f / dpi;
        }

        /// <summary>
        /// Preprocess Image using a shader to provide the correct image format for the model
        /// </summary>
        public static RenderTexture PreprocessImage(RenderTexture source, RenderTexture destination, ComputeShader preprocessCS, int imageSize = 128)
        {
            preprocessCS.SetTexture(0, "_Texture", source);
            preprocessCS.SetTexture(0, "_Tensor", destination);
            preprocessCS.SetInt("_ImageSize", imageSize);
            preprocessCS.Dispatch(0, imageSize, imageSize, 1);

            return destination;
        }

        /// <summary>
        /// Quits the application. If in Editor it just stops playing
        /// </summary>
        public static void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
