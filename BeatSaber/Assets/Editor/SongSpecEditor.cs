using BeatSaber.ScriptableObjects;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace BeatSaber
{
    [CustomEditor(typeof(SongSpec))]
    public class SongSpecEditor : Editor
    {
        const float THRESHOLD_GAIN = 1.2f;
        AudioClip _cachedAudioClip;
        float _cachedBpm;

        public override void OnInspectorGUI()
        {
            SongSpec songSpec = (SongSpec)target;
            GUILayoutOption[] noOptions = new GUILayoutOption[0];

            // IDisposable.Dispose() 함수 호출 보장 구문
            // 개발자가 Dispose 호출을 실수로 누락하면 메모리누수가 일어날 위험이있으므로 
            // 코드영역을 지정하여 Dispose 호출을 해주도록 보장함
            using (new EditorGUILayout.VerticalScope())
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    FieldInfo field = songSpec.GetType().GetField("<title>k__BackingField",
                        System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

                    string value = (string)field.GetValue(songSpec);
                    string changed = EditorGUILayout.TextField("Title", value, noOptions);
                    field.SetValue(songSpec, changed);
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    FieldInfo field = songSpec.GetType().GetField("<description>k__BackingField",
                        System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

                    string value = (string)field.GetValue(songSpec);
                    string changed = EditorGUILayout.TextField("Description", value, noOptions);
                    field.SetValue(songSpec, changed);
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    FieldInfo field = songSpec.GetType().GetField("<videoClip>k__BackingField",
                        System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

                    Object value = (Object)field.GetValue(songSpec);
                    Object changed = EditorGUILayout.ObjectField("Video Clip", value, typeof(AudioClip), false);
                    field.SetValue(songSpec, changed);
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    FieldInfo field = songSpec.GetType().GetField("<audioClip>k__BackingField",
                        System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

                    Object value = (Object)field.GetValue(songSpec);
                    Object changed = EditorGUILayout.ObjectField("Audio Clip", value, typeof(AudioClip), false);                
                    field.SetValue(songSpec, changed);
                    _cachedAudioClip = changed as AudioClip;
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    FieldInfo field = songSpec.GetType().GetField("<bpm>k__BackingField",
                        System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

                    float value = (float)field.GetValue(songSpec);
                    float changed = EditorGUILayout.FloatField("BPM", value, noOptions);
                    field.SetValue(songSpec, changed);
                    _cachedBpm = changed;
                }

                using (new EditorGUILayout.VerticalScope())
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Audio spectrum sampling", EditorStyles.boldLabel);

                    FieldInfo field = songSpec.GetType().GetField("<peaks>k__BackingField",
                        System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

                    if (_cachedAudioClip != null)
                    {
                        if (GUILayout.Button("Bake peaks"))
                        {
                            List<float> bakedPeaks = BakePeaks(_cachedAudioClip, _cachedBpm);
                            field.SetValue(songSpec, bakedPeaks);
                        }
                    }

                    List<float> value = (List<float>)field.GetValue(songSpec);

                    for (int i = 0; i < value.Count; i++)
                    {
                        value[i] = EditorGUILayout.FloatField(value[i], noOptions);
                    }
                    
                    field.SetValue(songSpec, value);
                }
            }
        }


        List<float> BakePeaks(AudioClip audioClip, float bpm)
        {
            int sampleCount = audioClip.samples;
            int channelCount = audioClip.channels;
            float[] raw = new float[sampleCount * channelCount];
            audioClip.GetData(raw, 0);

            float[] mono = new float[sampleCount];

            for (int i = 0; i < sampleCount; i++)
            {
                float sum = 0f;

                for (int j = 0; j < channelCount; j++)
                {
                    sum += raw[i + j * sampleCount];
                }

                mono[i] = sum;
            }

            // 볼륨(소리크기) 값을 확인하기위함
            float windowSize = 24 * audioClip.frequency / bpm; // 상수값은 테스트하면서 보정해야함
            int windowCount = Mathf.FloorToInt(sampleCount / windowSize);
            float[] rmsArr = new float[windowCount];
            float meanRms = 0f;

            for (int w = 0; w < windowCount; w++)
            {
                int startIndex = w * Mathf.FloorToInt(windowSize);
                float acc = 0f;

                for (int i = 0; i < windowSize; i++)
                {
                    float s = mono[startIndex + i];
                    acc += s * s;
                }

                float rms = Mathf.Sqrt(acc);
                rmsArr[w] = rms;
                meanRms += rms;
            }

            meanRms /= windowCount;

            double stdSum = 0;

            // 표준편차
            for (int w = 0; w < windowCount; w++)
            {
                float d = rmsArr[w] - meanRms;
                stdSum = d * d;
            }

            stdSum /= (double)(windowCount - 1);
            float stdRms = Mathf.Sqrt((float)stdSum);

            // 임계값 (평균값 + 임계 계수 * 표준편차)
            float thresHold = meanRms + THRESHOLD_GAIN * stdRms;

            // 임계값 이상만 추출
            List<float> peaks = new List<float>();
            float windowSec = windowSize / (float)audioClip.frequency;

            for (int w = 0; w < windowCount; w++)
            {
                if (rmsArr[w] >= thresHold)
                {
                    float time = (w + 0.5f) * windowSec;
                    peaks.Add(time);
                }
            }

            return peaks;
        }
    }
}