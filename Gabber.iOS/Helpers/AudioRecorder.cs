using System;
using System.IO;
using AVFoundation;
using Foundation;

namespace Gabber.iOS.Helpers
{
    public class AudioRecorder
    {
        AVAudioRecorder recorder;
        // The recorder manages state of file to reduce logic in controllers
        string filename;
		// Setup on construction to reduce load time when recording
        public AudioRecorder() => SetupRecorder();

        void SetupRecorder()
        {
            filename = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), filename);

            var audioSession = AVAudioSession.SharedInstance();
            var err = audioSession.SetCategory(AVAudioSessionCategory.PlayAndRecord);
            err = audioSession.SetActive(true);

            NSObject[] values = {
                NSNumber.FromFloat (44100.0f),
                NSNumber.FromInt32 ((int)AudioToolbox.AudioFormatType.LinearPCM),
                NSNumber.FromInt32 (2),
                NSNumber.FromInt32 (16),
                NSNumber.FromBoolean (false),
                NSNumber.FromBoolean (false)
            };

            NSObject[] keys = {
                AVAudioSettings.AVSampleRateKey,
                AVAudioSettings.AVFormatIDKey,
                AVAudioSettings.AVNumberOfChannelsKey,
                AVAudioSettings.AVLinearPCMBitDepthKey,
                AVAudioSettings.AVLinearPCMIsBigEndianKey,
                AVAudioSettings.AVLinearPCMIsFloatKey
            };

            recorder = AVAudioRecorder.Create(
                NSUrl.FromFilename(path), 
                new AudioSettings(NSDictionary.FromObjectsAndKeys(values, keys)), 
                out var error
            );
        }

        public void Record() => recorder.Record();
        public bool IsRecording() => recorder.Recording;
        public int CurrentTime() => (int)recorder.currentTime;

        public string FinishRecording () 
        {
            recorder.Stop();
            recorder.Dispose();
            return filename;
        }
    }
}