using UnityEngine;
using CSCore;
using CSCore.SoundIn;
using CSCore.Codecs.WAV;
using CSCore.DSP;
using CSCore.Streams;
using System.Linq;
using System.Collections.Generic;
using System;

public class SoundCapture : MonoBehaviour
{
    public int numBars = 30;
    public int minFreq = 10;
    public int maxFreq = 10000;
    public bool logScale = true;
    public bool isAverage = false;

    public float highScaleAverage = 2.0f;
    public float highScaleNotAverage = 3.0f;

    SpectrumBase lineSpectrum;
    BasicSpectrumProvider spectrumProvider;

    WasapiCapture capture;
    WaveWriter writer;
    SingleBlockNotificationStream notificationSource;
    IWaveSource finalSource;

    FftSize fftSize;
    float[] fftBuffer;
    float[] rawBuffer;
    float[] resData;
    public float minRawVolume;
    public float maxRawVolume;
    private int sampleRate;

    void Start()
    {
        // This uses the wasapi api to get any sound data played by the computer
        capture = new WasapiLoopbackCapture();

        capture.Initialize();

        // Get our capture as a source
        IWaveSource source = new SoundInSource(capture);


        // From https://github.com/filoe/cscore/blob/master/Samples/WinformsVisualization/Form1.cs

        // This is the typical size, you can change this for higher detail as needed
        fftSize = FftSize.Fft8192;

        // Actual fft data
        fftBuffer = new float[(int)fftSize];
        rawBuffer = new float[(int)fftSize];

        // These are the actual classes that give you spectrum data
        // The specific vars of lineSpectrum are changed below in the editor so most of these aren't that important here
        sampleRate = capture.WaveFormat.SampleRate;
        spectrumProvider = new BasicSpectrumProvider(capture.WaveFormat.Channels, sampleRate, fftSize);

        lineSpectrum = new SpectrumBase()
        {
            FftSize = fftSize,
            SpectrumProvider = spectrumProvider,
            UseAverage = isAverage,
            // BarCount = numBars,
            // BarSpacing = 2,
            IsXLogScale = logScale,
            ScalingStrategy = ScalingStrategy.Linear
        };

        // Tells us when data is available to send to our spectrum
        var notificationSource = new SingleBlockNotificationStream(source.ToSampleSource());

        notificationSource.SingleBlockRead += NotificationSource_SingleBlockRead;

        // We use this to request data so it actualy flows through (figuring this out took forever...)
        finalSource = notificationSource.ToWaveSource();

        capture.DataAvailable += Capture_DataAvailable;
        capture.Start();
    }

    private void Capture_DataAvailable(object sender, DataAvailableEventArgs e)
    {
        finalSource.Read(e.Data, e.Offset, e.ByteCount);
    }

    int rIndex = 0;
    private void NotificationSource_SingleBlockRead(object sender, SingleBlockReadEventArgs e)
    {
        spectrumProvider.Add(e.Left, e.Right);

        rawBuffer[rIndex] = (e.Left + e.Right) / 2.0f;
        rIndex = (rIndex + 1) % (int)fftSize; 

        if (rIndex == 0) {
            maxRawVolume = rawBuffer.Max();
            minRawVolume = rawBuffer.Min();
        }
    }

    void OnApplicationQuit()
    {
        if (enabled)
        {
            capture.Stop();
            capture.Dispose();
        }
    }


    public float[] barData;
    public object barLock = new object();

    public object bufferLock = new object();

    public int GetFftBinSize()
    {
        return (int)spectrumProvider.FftSize;
    }

    public float GetFftMax()
    {
        lock(bufferLock)
        {
            if (resData?.Length > 0)
            {
                return resData.Max();
            }
        }

        return 0;
    }

    public List<Tuple<float, float>> GetMagnitudeByFrequence()
    {
        List<Tuple<float, float>> freqMap = new List<Tuple<float, float>>();
        float f = (float)sampleRate / 2.0f;
        if (resData == null || resData.Length == 0)
        {
            return freqMap;
        }

        lock(bufferLock)
        {
            for (int i = 0; i < resData.Length; i++)
            {
                float frequency = (2.0f * f * (float)i) / (float)fftSize;
                freqMap.Add(new Tuple<float, float>(frequency, resData[i]));
            }
        }

        return freqMap;
    }

    public int GetFftSize()
    {
        return (int) spectrumProvider.FftSize;
    }

    public int GetFftBandIndex(float freq)
    {
        if (spectrumProvider == null)
        {
            return 0;
        }

        return spectrumProvider.GetFftBandIndex(freq);
    }

    public float[] GetFFtData()
    {
        lock (barLock)
        {
            if (numBars != barData.Length)
            {
                barData = new float[numBars];
            }
        }

        if (spectrumProvider?.IsNewDataAvailable == true)
        {
            lineSpectrum.MinimumFrequency = minFreq;
            lineSpectrum.MaximumFrequency = maxFreq;
            lineSpectrum.IsXLogScale = logScale;
            lineSpectrum.UseAverage = isAverage;
            lineSpectrum.SpectrumProvider.GetFftData(fftBuffer, this);
            return fftBuffer;
        }
        else
        {
            return null;
        }
    }

    void Update()
    {
        int numBars = barData.Length;

        lock(bufferLock)
        {
            resData = GetFFtData();
        }

        if (resData == null)
        {
            return;
        }

        lock (barLock)
        {
            for (int i = 0; i < numBars && i < resData.Length; i++)
            {
                // Make the data between 0.0 and 1.0
                barData[i] = resData[i] / 100.0f;
            }

            for (int i = 0; i < numBars && i < resData.Length; i++)
            {
                if (lineSpectrum.UseAverage)
                {
                    // Scale the data because for some reason bass is always loud and treble is soft
                    barData[i] = barData[i] + highScaleAverage * Mathf.Sqrt(i / (numBars + 0.0f)) * barData[i];
                }
                else
                {
                    barData[i] = barData[i] + highScaleNotAverage * Mathf.Sqrt(i / (numBars + 0.0f)) * barData[i];
                }
            }
        }

    }
}
