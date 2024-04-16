using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundGen : MonoBehaviour
{
    float sampleRate;
    AudioSource audioSource;
    public Dictionary<int, List<float>> frequencies;
    List<float> phase, increment; 

    void Awake()
    {
        sampleRate = AudioSettings.outputSampleRate;
        phase = new List<float>{0,0};
        increment = new List<float>{0,0};
        frequencies = new Dictionary<int, List<float>>();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = true;
    }

    public void OnKey(int keyNumber)
    {
        float freq = 440 * Mathf.Pow(2, ((float)keyNumber-69f)/12f); 
        frequencies[keyNumber] = new List<float>{freq, 0};

        // Iniciamos un temporizador para detener la nota después de 1 segundo
        StartCoroutine(StopNoteAfterDelay(keyNumber, 1f));
    }
    public void OnKeyOff(int keyNumber)
    {
        frequencies.Remove(keyNumber);
    }


    IEnumerator StopNoteAfterDelay(int keyNumber, float duration)
    {
        yield return new WaitForSeconds(duration);
        frequencies.Remove(keyNumber);
    }

    public void ChangePitch(int keyNumber, float pitch)
    {
        try
        {
            frequencies[keyNumber][1] = pitch;
        }
        catch
        {
            Debug.Log("Not yet here");
        }
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        int counter = 0;
        try
        {
            foreach (var item in frequencies.Keys)
            {
                for(int i = 0; i < data.Length; i+= channels)
                {          
                    float freq = frequencies[item][0];
                    float vibratoAmount = frequencies[item][1];
                    float incrementAmount = (freq + freq/10*vibratoAmount) * 2f * Mathf.PI/ sampleRate;
                    phase[counter] += incrementAmount;
                    data[i] += (float) (Mathf.Sin(phase[counter]));
                    if(phase[counter] > (Mathf.PI*2f))
                    {
                        phase[counter] = 0f; 
                    }
                }
                counter ++;
            }
        }
        catch
        {
            Debug.Log("Accessing while changing the frequency");
        }
    }
}
