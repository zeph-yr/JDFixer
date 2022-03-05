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
            canvasTransform.position = new Vector3(0f, 2f, 3.8f);
            canvasTransform.localScale = Vector3.one;

            text = CreateText(canvas, new Vector2(0f, 0f));

        }

        private void Update()
        {
            if (text_shown == false && audioTime.songTime >= 3.5f)
            {
                text.gameObject.SetActive(true);

                //text.CrossFadeAlpha(1f, -30f, false); // This only fades out lol
                tween.AddTween(new FloatTween(0, 1, value => text.alpha = value, 3f, EaseType.InCubic), text);

                text_shown = true;
            }

            //else if (au)
        }

        private static TextMeshProUGUI CreateText(Canvas canvas, Vector2 position)
        {
            GameObject gameObject = new GameObject("CustomUIText");
            gameObject.SetActive(false);
            TextMeshProUGUI tmp = gameObject.AddComponent<TextMeshProUGUI>();

            tmp.rectTransform.SetParent(canvas.transform, false);
            tmp.rectTransform.transform.localPosition = Vector3.zero;
            tmp.rectTransform.anchoredPosition = position;

            tmp.text = "Hello there";
            tmp.fontSize = 0.15f;
            tmp.color = Color.white;
            tmp.alpha = 0f;
            tmp.alignment = TextAlignmentOptions.Center;

            return tmp;
        }
    }
}
