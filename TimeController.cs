using System.Linq;
using UnityEngine;
using Zenject;

namespace JDFixer
{
    public class TimeController : MonoBehaviour
    {
        internal static AudioTimeSyncController audioTime;
        internal static float length = 0.001f;

        [Inject]
        public void Construct(AudioTimeSyncController audioTimeSyncController)
        {
            audioTime = audioTimeSyncController;
            length = audioTime.songEndTime;
        }

        private void Start()
        {
            Logger.log.Debug("Start TimeController");

            // If game starts up before midnight kekeke

            /*if (PluginConfig.Instance.enabled && 
                DateTime.Compare(DateTime.Now, new DateTime(2022, 3, 31)) >= 0 && DateTime.Compare(DateTime.Now, new DateTime(2022, 4, 2)) < 0 && 
                PluginConfig.Instance.af_enabled)*/
            /*if (true)
            {
                audioTime = Resources.FindObjectsOfTypeAll<AudioTimeSyncController>().FirstOrDefault();
                length = audioTime.songEndTime;
            }*/
        }
    }
}
