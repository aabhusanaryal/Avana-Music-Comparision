using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class AudioRecorder : MonoBehaviour
{
    public AudioSource audiosrc;
    public int samplingRate = 44100;
    public int targetFrameRate = 60;
    public int maxAudioLength = 5;
    private bool ended = false;
    private int sampleNo;
    private float[] spectrum = new float[512];
    public int[][] domFreq;

    // Start is called before the first frame update
    void Start()
    {
        audiosrc = GetComponent<AudioSource>();
        Application.targetFrameRate = targetFrameRate;
        domFreq = new int[maxAudioLength * targetFrameRate][];

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int updateSpectrumArray()
    {
        if (audiosrc.isPlaying && sampleNo < (targetFrameRate * maxAudioLength))
        {
            domFreq[sampleNo] = new int[3];
            //Debug.Log("playing detected");

            audiosrc.GetSpectrumData(spectrum, 0, FFTWindow.Hamming);

            for (int i = 0; i < 3; i++)
            {
                var m = spectrum.Max();
                var freqIdx = Array.IndexOf(spectrum, m);
                if (m >= .00001f)
                {
                    domFreq[sampleNo][i] = freqIdx;
                    spectrum[freqIdx] = 0;
                }
                else
                    domFreq[sampleNo][i] = -1;
            }
            sampleNo++;
            return 0;
        }
        else
        {
            Debug.Log("Recorded Data ");
            return 1;

        }

    }

    public void StartRecording() {
        sampleNo = 0;
        // Record audio
        audiosrc.clip = Microphone.Start(Microphone.devices[0], false, maxAudioLength, samplingRate);
        StartCoroutine(endRecording());
    }

    IEnumerator endRecording()
    {
        yield return new WaitForSeconds(maxAudioLength);
        if (Microphone.IsRecording(Microphone.devices[0]))
            Microphone.End(Microphone.devices[0]);
        Debug.Log("recording ended");
        ended = true;
    }
}
