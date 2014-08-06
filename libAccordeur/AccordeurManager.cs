using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.CoreAudioApi;
using NAudio.Mixer;
using NAudio.Wave;

namespace libAccordeur
{
    public class AccordeurManager
    {
        public WaveIn sourceStream {get;set;}

        const float eGrave = 82.4f;
        const float a = 110.0f;
        const float d = 146.8f;
        const float g = 196.0f;
        const float b = 246.9f;
        const float eAigu = 329.5f;

        public bool IsInProgress = false;
        public WaveInProvider wvProvider { get; set; }
        public DirectSoundOut waveOut { get; set; }
        private float frequency { get; set; }
        private String note { get; set; }
        public float frequencyInProgress{
            get { return frequency; }
        }
        public string noteInProgress
        {
            get { return note; }
        }

        public AccordeurManager(int sampleRate, int channel, int deviceNumber) {
            sourceStream = new WaveIn();
            sourceStream.WaveFormat = new WaveFormat(sampleRate, channel);
            sourceStream.DeviceNumber = deviceNumber;
            sourceStream.DataAvailable += waveIn_DataAvailable;
            sourceStream.RecordingStopped += OnRecordingStopped;

            //wvProvider = new WaveInProvider(sourceStream);
            //waveOut = new DirectSoundOut();
            //waveOut.Init(wvProvider);
        }
        public void demarrer() {
            sourceStream.StartRecording();
            MixerLine mx = sourceStream.GetMixerLine();
            IsInProgress = true;
            //waveOut.Play();

        }
        void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            byte[] buffer = e.Buffer;
            int bytesRecorded = e.BytesRecorded;
            float[] fl = new float[e.BytesRecorded / 2];
            int id = 0;
            for (int index = 0; index < e.BytesRecorded; index += 2)
            {
                short sample = (short)((buffer[index + 1] << 8) |
                                        buffer[index + 0]);
                float sample32 = sample / 32768f;
                fl[id] = sample;
                id++;
            }
            AutoCorrelator au = new AutoCorrelator(24000);
            frequency = au.DetectPitch(fl, fl.Length);
            DeterminerNote(frequency);
            
            //lbFreq.Content = res;
            //lblNote.Content = DeterminerNote(res);
        }
        void OnRecordingStopped(object sender, StoppedEventArgs e)
        {
            //Stop((WaveIn)sender);
        }
        public void Stop()
        {
            sourceStream.StopRecording();
            IsInProgress = false;
        }
        private String DeterminerNote(float freq)
        {
            String res = string.Empty;
            if (IsDivisble(freq, eGrave) || (freq > eGrave - 10.0f && freq < eGrave + 10.0f))
            {
                res = "E";
            }
            else if (IsDivisble(freq, a) || (freq > a - 10.0f && freq < a + 10.0f))
            {
                res = "A";
            }
            else if (IsDivisble(freq, d) || (freq > d - 10.0f && freq < d + 10.0f))
            {
                res = "D";
            }
            else if (IsDivisble(freq, g) || (freq > g - 10.0f && freq < g + 10.0f))
            {
                res = "G";
            }
            else if (IsDivisble(freq, b) || (freq > b - 10.0f && freq < b + 10.0f))
            {
                res = "B";
            }
            //if (freq > eGrave - 10.0f && freq < eGrave + 10.0f)
            //{
            //   res = "E";
            //} else if (freq > a - 10.0f && freq < a + 10.0f)
            //{
            //    res = "A";
            //} else if (freq > d - 10.0f && freq < d + 10.0f)
            //{
            //    res = "D";
            //} else if (freq > g - 10.0f && freq < g + 10.0f)
            //{
            //    res = "G";
            //} else if (freq > b - 10.0f && freq < b + 10.0f)
            //{
            //    res = "B";
            //} if (freq > eAigu - 10.0f && freq < eAigu + 10.0f)
            //{
            //    res = "E";
            //}
            note = res;
            return res;
        }

        private bool IsDivisble(float x, float n)
        {
            float result;
            if (x / n < 2)
            {
                result = x - (x % n);
            }
            else
            {
                result = (x % n);
            }
            if (result < 10 && result > -10)
            {
                return true;
            }
            else { return false; }

        }
        static public Dictionary<String,int> getAllDevices() {
            Dictionary<String, int> devices = new Dictionary<String, int>();

            for (int i = 0; i < WaveIn.DeviceCount; i++)
            {
                devices.Add(WaveIn.GetCapabilities(i).ProductName, i);
            }
            return devices;
        }

    }
}
