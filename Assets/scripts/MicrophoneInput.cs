using UnityEngine;
using System.Collections;
//using UnityEngine.Microphone;

public class MicrophoneInput : MonoBehaviour {

	public AudioSource audio;
	private float[] samples;
	private float[] lastSamples;
	public EQView _eqView;
	public bool useMicrophone = true;
	public bool useClamp = true;
	public float leftClamp = 0.0f;
	public float rightClamp = 1.0f;
	public float smoothing = 0.0f;
	public int inputHz = 40000;
	public int fTarget = 1000;
	public int fWidth = 120;
	private int numSamples = 8192;
	private float fMax;
	// Use this for initialization
	void Start () {
		audio.loop = true;
		samples = new float[numSamples];
		lastSamples = new float[numSamples];
		fMax = AudioSettings.outputSampleRate;
		//fMax = inputHz;
		Debug.Log(fMax.ToString());
		if (useMicrophone) {
			audio.clip = Microphone.Start("Built-in Microphone", true, 10, inputHz);
			while (Microphone.GetPosition(null) <= 0){}
		}
		//audio.mute = true;
		audio.Play();
		audio.GetSpectrumData (lastSamples, 0, FFTWindow.Hanning);
	}
	
	// Update is called once per frame
	void Update () {
		System.Array.Copy(samples,lastSamples,numSamples);
		audio.GetSpectrumData (samples, 0, FFTWindow.Hanning);
		//audio.GetOutputData (samples, 0);
		for (int i = 0; i < numSamples; i++) {
			samples[i] = smoothing * lastSamples[i] + (1/smoothing) * samples[i];
		}

		// int xScale = 8;
		// int yScale = 400;
		// int xoffset = 3000;
		// int yoffset = 1000;
		// for (int i = 1; i < numSamples-1; i++) {
		//  Debug.DrawLine (new Vector3 ((i - 1)*xScale-xoffset, samples[i]*yScale-yoffset, 0),
		// 				new Vector3 (i*xScale-xoffset, samples[i + 1]*yScale-yoffset, 0), Color.red);

		// 	Debug.DrawLine (new Vector3 ((i - 1)*xScale-xoffset, Mathf.Log(samples[i])*yScale-yoffset, 0),
		// 				new Vector3 (i*xScale-xoffset, Mathf.Log(samples[i + 1])*yScale-yoffset, 0), Color.cyan);

		// 	Debug.DrawLine (new Vector3 (Mathf.Log(i - 1)*xScale-xoffset, samples[i]*yScale-yoffset, 0),
		// 				new Vector3 (Mathf.Log(i)*xScale-xoffset, samples[i + 1]*yScale-yoffset, 0), Color.green);

		// 	Debug.DrawLine (new Vector3 (Mathf.Log(i - 1)*xScale-xoffset, Mathf.Log(samples[i])*yScale-yoffset, 0),
		// 				new Vector3 (Mathf.Log(i)*xScale-xoffset, Mathf.Log(samples[i + 1])*yScale-yoffset, 0), Color.yellow);
		//  }


		if (useClamp) {
			//Used for updating eq with % clamps on either side
			_eqView.UpdateEQClamped(samples, leftClamp, rightClamp);
		} else {
			//Used for updating eq with a target range specified in Hz
			_eqView.UpdateEQHzRange(samples, fTarget, fWidth, fMax/2);
		}
		
		//Debug.Log(samples[512].ToString());
	}
}