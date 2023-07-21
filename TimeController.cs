using System;
using TMPro;
using Tweening;
using UnityEngine;
using Zenject;

namespace JDFixer
{
    internal class TimeController : MonoBehaviour
    {
        internal static AudioTimeSyncController audioTime;
        internal static float length = 0.001f;

        private static Canvas canvas;
        private static TextMeshProUGUI text;
        private static bool text_shown;
        private static TimeTweeningManager tween = null;
        private static bool earthday = false;

        [Inject]
        internal void Construct(AudioTimeSyncController audioTimeSyncController, TimeTweeningManager timeTweeningManager)
        {
            audioTime = audioTimeSyncController;
            length = audioTime.songEndTime;

            text_shown = false;
            tween = timeTweeningManager;
        }

        private void Start()
        {
            //Plugin.Log.Debug("TimeController Start");

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
            if (earthday && text_shown == false && audioTime.songTime >= 2)
            {
                text.gameObject.SetActive(true);
                tween.AddTween(new FloatTween(0, 1, value => text.alpha = value, 3.5f, EaseType.InCubic), text);
                text_shown = true;
                return;
            }

            if (text_shown == false && audioTime.songTime >= 0.25 * length)
            {
                text.gameObject.SetActive(true);
                tween.AddTween(new FloatTween(0, 1, value => text.alpha = value, 3.5f, EaseType.InCubic), text);
                text_shown = true;
            }
            else if (!earthday && text.gameObject.activeSelf && audioTime.songTime >= 0.5 * length)
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

            if (DateTime.Compare(DateTime.Now, new DateTime(DateTime.Now.Year, 4, 22)) >= 0 && DateTime.Compare(DateTime.Now, new DateTime(DateTime.Now.Year, 4, 23)) < 1)
            {
                earthday = true;
                tmp.text = "Hello there.\nMaking mods is hard work. If JDFixer has helped you,\nI ask one favor in return.\n <#ffff00>Today is Earth Day. We are in a climate emergency.\nI ask you to do anything and everything you can to preserve our and your future.\nWe CAN do this together.";
            }
            else if (DateTime.Compare(DateTime.Now, new DateTime(DateTime.Now.Year, 4, 1)) >= 0 && DateTime.Compare(DateTime.Now, new DateTime(DateTime.Now.Year, 4, 2)) < 1)
            {
                tmp.text = "Hello, Happy April Fools and have fun with this new game mode!\nHint - If you want out, you may turn it off in the config ^^";
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
