// Source: adapted from Unity forum and public domain examples
using UnityEngine;
using System.IO;
using System;

public static class WavUtility
{
    public static byte[] FromAudioClip(AudioClip clip)
    {
        MemoryStream stream = new MemoryStream();
        int sampleCount = clip.samples * clip.channels;
        int headerSize = 44;
        int byteRate = clip.frequency * clip.channels * 2;

        byte[] header = new byte[headerSize];
        byte[] data = new byte[sampleCount * 2];

        clip.GetData(new float[sampleCount], 0); // just to trigger loading

        float[] samples = new float[sampleCount];
        clip.GetData(samples, 0);

        int offset = 0;
        for (int i = 0; i < samples.Length; i++)
        {
            short intSample = (short)(samples[i] * short.MaxValue);
            byte[] byteArr = BitConverter.GetBytes(intSample);
            byteArr.CopyTo(data, offset);
            offset += 2;
        }

        using (BinaryWriter writer = new BinaryWriter(stream))
        {
            // RIFF header
            writer.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));
            writer.Write(headerSize + data.Length - 8);
            writer.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"));

            // fmt chunk
            writer.Write(System.Text.Encoding.ASCII.GetBytes("fmt "));
            writer.Write(16); // size of fmt
            writer.Write((ushort)1); // PCM
            writer.Write((ushort)clip.channels);
            writer.Write(clip.frequency);
            writer.Write(byteRate);
            writer.Write((ushort)(clip.channels * 2)); // block align
            writer.Write((ushort)16); // bits per sample

            // data chunk
            writer.Write(System.Text.Encoding.ASCII.GetBytes("data"));
            writer.Write(data.Length);
            writer.Write(data);
        }

        return stream.ToArray();
    }
}
