using System;
using TMPro;
using Tweening;
using UnityEngine;
using Zenject;

namespace JDFixer
{
    public class TimeController : MonoBehaviour
    {
        internal static AudioTimeSyncController audioTime;
        internal static float length = 0.001f;

        private static Canvas canvas;
        private static TextMeshProUGUI text;
        private static bool text_shown;
        private static TimeTweeningManager tween = null;

        [Inject]
        public void Construct(AudioTimeSyncController audioTimeSyncController, TimeTweeningManager timeTweeningManager)
        {
            audioTime = audioTimeSyncController;
            length = audioTime.songEndTime;

            text_shown = false;
            tween = timeTweeningManager;
        }

        private void Start()
        {
            Logger.log.Debug("TimeController Start");

            // If game starts up before midnight kekeke

            /*if (PluginConfig.Instance.enabled && 
                DateTime.Compare(DateTime.Now, new DateTime(2022, 3, 31)) >= 0 && DateTime.Compare(DateTime.Now, new DateTime(2022, 4, 2)) < 0 && 
                PluginConfig.Instance.af_enabled)*/
            /*if (true)
            {
                audioTime = Resources.FindObjectsOfTypeAll<AudioTimeSyncController>().FirstOrDefault();
                length = audioTime.songEndTime;
            }*/


            GameObject canvasGo = new GameObject("Canvas");
            canvasGo.transform.parent = transform;
            canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;

            var canvasTransform = canvas.transform;
            canvasTransform.position = new Vector3(0f, 3f, 3.8f);
            canvasTransform.localScale = Vector3.one;

            text = CreateText(canvas, new Vector2(0f, 0f));
        }

        private void Update()
        {
            if (text_shown == false && audioTime.songTime >= 0.5 * length)
            {
                text.gameObject.SetActive(true);
                tween.AddTween(new FloatTween(0, 1, value => text.alpha = value, 3.5f, EaseType.InCubic), text);
                text_shown = true;
            }
            else if (text.gameObject.activeSelf && audioTime.songTime >= 0.5 * length + 20f)
            {
                //text.CrossFadeAlpha(0f, -3.5f, false); // This doesnt work
                tween.AddTween(new FloatTween(1, 0, value => text.alpha = value, 3.5f, EaseType.InCubic), text);
            }
        }

        private static TextMeshProUGUI CreateText(Canvas canvas, Vector2 position)
        {
            GameObject gameObject = new GameObject("CustomUIText");
            gameObject.SetActive(false);
            TextMeshProUGUI tmp = gameObject.AddComponent<TextMeshProUGUI>();

            tmp.rectTransform.SetParent(canvas.transform, false);
            tmp.rectTransform.transform.localPosition = Vector3.zero;
            tmp.rectTransform.anchoredPosition = position;

            if (DateTime.Compare(DateTime.Now, new DateTime(2022, 4, 1)) >= 0)
            {
                tmp.text = "Hello, Happy April Fools and have fun! \nHint - If you want out, look in the config ^^";
            }
            else if (DateTime.Compare(DateTime.Now, new DateTime(2022, 4, 22)) >= 0)
            {
                tmp.text = "Hello there.\nMaking mods is hard work. If you've been enjoying using JDFixer,\nI ask one favor of you in return.\n <#ffff00>Today is Earth Day. We are in a climate emergency.\nI ask you to do anything and everything in your power to protect our and your future.\n<#ffff00>No action is too small.";
            }
            else
            {
                tmp.text = "";
            }
            tmp.fontSize = 0.12f;
            tmp.color = new Color(1f, 0f, 0.5f);
            tmp.alpha = 0f;
            tmp.alignment = TextAlignmentOptions.Center;

            return tmp;
        }
    }
}
