using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AudioComp : MonoBehaviour
{
    bool comparing = false;
    public AudioRecorder a1;
    public AudioRecorder a2;
    public int lookahead = 200;
    public TMPro.TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
     
    }

    // Update is called once per frame
    void Update()
    {
        if (comparing)
        {

           int comp1 =  a1.updateSpectrumArray();
            int comp2 = a2.updateSpectrumArray();
            if(comp1 + comp2 == 2)
            {
                // Complete the spectrum filling process
                comparing = false;
                BeginAlgorithm();
            }
        }
    }

    public void BeginComparision()
    {
        comparing = true;
        a1.audiosrc.Play();
        a2.audiosrc.Play();
    }

    public void BeginAlgorithm()
    {
        float comparisionScore = 0;
        int validSamples = 0;
        var sample1 = 0;
        var sample2 = 0;
        var domFreq1 = a1.domFreq;
        var domFreq2 = a2.domFreq;

        int sampleCount = a1.maxAudioLength * a1.targetFrameRate;
        for (int i = 0; i < sampleCount && sample1 < sampleCount && sample2 < sampleCount; i++)
        {
            if (domFreq1[sample1] == null || domFreq2[sample2] == null)
            {
                continue;
            }
            if (domFreq1[sample1][0] != -1)
            {
                validSamples++;
                for (int j = 0; j < lookahead && (sample2 + j < sampleCount); j++)
                {
                    if (domFreq1[sample1][0] == domFreq2[sample2 + j][0])
                    {
                        float temp = 0;
                        temp += 1 ;
                        if (domFreq1[sample1][1] == domFreq2[sample2 + j][1] && domFreq1[sample2 + j][1] != -1 && j != 0)
                            temp += .3f ;
                        if (domFreq1[sample1][2] == domFreq2[sample2 + j][2] && domFreq1[sample2 + j][2] != -1 && j != 0)
                            temp += .2f ;
                        comparisionScore += Mathf.Clamp(temp, 0.0f, 1.0f);
                        sample2 = sample2 + j;
                        break;
                    }
                }
            }
            sample1++;
            sample2++;
        }
        float similarPercent = comparisionScore / validSamples * 100.0f;
        Debug.Log("score: " + similarPercent + " " + validSamples + " valid samples");
        text.text = "Similarity: " + similarPercent.ToString();
        return;
    }
}