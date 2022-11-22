using Zenject;
using System;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;

namespace JDFixer.UI
{
    public class MainMenuUI : IInitializable, IDisposable
    {
        public MainMenuUI()
        {

        }

        public void Initialize()
        {
            BeatSaberMarkupLanguage.Settings.BSMLSettings.instance.AddSettingsMenu("JDFixer", "JDFixer.UI.BSML.mainMenuUI.bsml", this);
        }

        public void Dispose()
        {
            if (BeatSaberMarkupLanguage.Settings.BSMLSettings.instance != null)
            {
                BeatSaberMarkupLanguage.Settings.BSMLSettings.instance.RemoveSettingsMenu(this);
            }
        }


        [UIValue("rt_display_value")]
        private bool RT_Display_Value
        {
            get => PluginConfig.Instance.rt_display_enabled;
            set
            {
                PluginConfig.Instance.rt_display_enabled = value;
            }
        }
        [UIAction("set_rt_display")]
        private void Set_RT_Display(bool value)
        {
            RT_Display_Value = value;
        }


        [UIValue("legacy_display_value")]
        private bool Legacy_Display_Value
        {
            get => PluginConfig.Instance.legacy_display_enabled;
            set
            {
                PluginConfig.Instance.legacy_display_enabled = value;
            }
        }
        [UIAction("set_legacy_display")]
        private void Set_Legacy_Display(bool value)
        {
            Legacy_Display_Value = value;
        }


        [UIComponent("lower_threshold_slider")]
        private SliderSetting Lower_Threshold_Slider;

        [UIValue("lower_threshold_value")]
        private float Lower_Threshold_Value
        {
            get => PluginConfig.Instance.lower_threshold;
            set
            {
                PluginConfig.Instance.lower_threshold = value;
            }
        }
        [UIAction("set_lower_threshold")]
        private void Set_Lower_Threshold(float value)
        {
            Lower_Threshold_Value = value;
        }


        [UIComponent("upper_threshold_slider")]
        private SliderSetting Upper_Threshold_Slider;

        [UIValue("upper_threshold_value")]
        private float Upper_Threshold_Value
        {
            get => PluginConfig.Instance.upper_threshold;
            set
            {
                PluginConfig.Instance.upper_threshold = value;
            }
        }
        [UIAction("set_upper_threshold")]
        private void Set_Upper_Threshold(float value)
        {
            Upper_Threshold_Value = value;
        }

        [UIValue("press_ok_text")]
        private string Press_Ok_Text = "<#ffffffff>Press OK to apply settings  <#ff0080ff>♡       <size=70%>v6.0.0 by Zephyr#9125<#00000000>--";
    }
}