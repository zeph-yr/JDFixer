/*using System.Linq;
using HMUI;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using BeatSaberMarkupLanguage.Components.Settings;
using System.ComponentModel;

namespace JDFixer.UI
{
    public class PreferencesListViewController : BSMLResourceViewController, INotifyPropertyChanged
    {
        public override string ResourceName => "JDFixer.UI.BSML.preferencesList.bsml";
        public event PropertyChangedEventHandler PropertyChanged;


        [UIComponent("njs_slider")]
        private SliderSetting NJS_Slider;

        private float _new_njs = 16f;

        [UIValue("njs_value")]
        public float NJS_Value
        {
            get => _new_njs;
            set
            {
                _new_njs = value;
            }
        }
        [UIAction("set_njs")]
        void Set_NJS(float value)
        {
            NJS_Value = value;
        }


        [UIValue("min_jd_slider")]
        private int Min_JD_Slider => Get_Min_Slider(); //PluginConfig.Instance.minJumpDistance;
        [UIValue("max_jd_slider")]
        private int Max_JD_Slider => Get_Max_Slider(); //PluginConfig.Instance.maxJumpDistance;

        public int Get_Min_Slider()
        {
            if (PluginConfig.Instance.rt_enabled)
            {
                return (int)PluginConfig.Instance.minReactionTime;
            }

            else
            {
                return (int)PluginConfig.Instance.minJumpDistance;
            }
        }

        public int Get_Max_Slider()
        {
            if (PluginConfig.Instance.rt_enabled)
            {
                return (int)PluginConfig.Instance.maxReactionTime;
            }

            else
            {
                return (int)PluginConfig.Instance.maxJumpDistance;
            }
        }


        [UIComponent("jd_slider")]
        private SliderSetting JD_Slider;

        private float _new_jd = 23f;
        private float _new_rt = 500f;

        [UIValue("jd_value")]
        public float JD_Value
        {
            get
            {
                if (PluginConfig.Instance.rt_enabled)
                    return _new_rt;

                else
                    return _new_jd;
            }

            set
            {
                if (PluginConfig.Instance.rt_enabled)
                {
                    _new_rt = value;
                }

                else
                {
                    _new_jd = value;
                }
            }
        }

        [UIAction("set_jd")]
        void Set_JD(float value)
        {
            JD_Value = value;
        }


        [UIComponent("pref_list")]
        public CustomListTableData prefList;

        private JDPref _selected_jd_pref = null;
        private RTPref _selected_rt_pref = null;

        //[UIValue("prefIsSelected")]
        //public bool prefIsSelected
        //{
        //    get => _selectedPref != null;
        //    set
        //    {
        //        //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(prefIsSelected)));
        //    }
        //}

        //[UIComponent("leftButton")]
        //private RectTransform leftButton;
        
        //[UIComponent("rightButton")]
        //private RectTransform rightButton;

        [UIAction("select_pref")]
        private void Select_Pref(TableView tableView, int row)
        {
            //Logger.log.Debug("Selected row " + row);
            
            if (PluginConfig.Instance.rt_enabled)
            {
                _selected_rt_pref = PluginConfig.Instance.rt_preferredValues[row];
            }

            else
            {
                _selected_jd_pref = PluginConfig.Instance.preferredValues[row];
            }

            //prefIsSelected = prefIsSelected;
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(prefIsSelected)));
        }


        [UIAction("add_pressed")]
        private void Add_Pressed()
        {
            if (PluginConfig.Instance.rt_enabled)
            {
                if (PluginConfig.Instance.rt_preferredValues.Any(x => x.njs == _new_njs))
                {
                    PluginConfig.Instance.rt_preferredValues.RemoveAll(x => x.njs == _new_njs);
                }
                PluginConfig.Instance.rt_preferredValues.Add(new RTPref(_new_njs, _new_rt));
            }


            else // Or preferences is set to JD or Off
            {
                if (PluginConfig.Instance.preferredValues.Any(x => x.njs == _new_njs))
                {
                    PluginConfig.Instance.preferredValues.RemoveAll(x => x.njs == _new_njs);
                }
                PluginConfig.Instance.preferredValues.Add(new JDPref(_new_njs, _new_jd));
            }

            Reload_List_From_Config();
        }


        [UIAction("remove_pressed")]
        private void Remove_Pressed()
        {
            if (PluginConfig.Instance.rt_enabled)
            {
                if (_selected_rt_pref == null)
                {
                    return;
                }

                PluginConfig.Instance.rt_preferredValues.RemoveAll(x => x == _selected_rt_pref);
            }

            else // Or preferences is set to JD or Off
            {
                if (_selected_jd_pref == null)
                {
                    return;
                }

                PluginConfig.Instance.preferredValues.RemoveAll(x => x == _selected_jd_pref);
            }

            Reload_List_From_Config();
        }


        private void Reload_List_From_Config()
        {
            prefList.data.Clear();

            if (PluginConfig.Instance.rt_enabled)
            {
                if (PluginConfig.Instance.rt_preferredValues == null)
                {
                    return;
                }

                PluginConfig.Instance.rt_preferredValues.Sort((x, y) => y.njs.CompareTo(x.njs));

                foreach (var pref in PluginConfig.Instance.rt_preferredValues)
                {
                    prefList.data.Add(new CustomListTableData.CustomCellInfo($"{pref.njs} NJS | {pref.reactionTime} ms"));
                }
            }

            else
            {
                if (PluginConfig.Instance.preferredValues == null)
                {
                    return;
                }

                PluginConfig.Instance.preferredValues.Sort((x, y) => y.njs.CompareTo(x.njs));

                foreach (var pref in PluginConfig.Instance.preferredValues)
                {
                    prefList.data.Add(new CustomListTableData.CustomCellInfo($"{pref.njs} NJS | {pref.jumpDistance} Jump Distance"));
                }
            }

            prefList.tableView.ReloadData();
            prefList.tableView.ClearSelection();
            _selected_jd_pref = null;
            //prefIsSelected = prefIsSelected;
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(prefIsSelected)));
        }

        HMUI.CustomFormatRangeValuesSlider jd_slider_range;

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
            if (!firstActivation)
            {
                Reload_List_From_Config();

                jd_slider_range = JD_Slider.slider.GetComponentInChildren<HMUI.CustomFormatRangeValuesSlider>();

                jd_slider_range.minValue = Get_Min_Slider();
                jd_slider_range.maxValue = Get_Max_Slider();

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Min_JD_Slider)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Max_JD_Slider)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(JD_Value)));
            }
        }

        protected override void DidDeactivate(bool removedFromHierarchy, bool screenSystemDisabling)
        {
            base.DidDeactivate(removedFromHierarchy, screenSystemDisabling);
        }


        [UIAction("#post-parse")]
        private void PostParse()
        {
            Reload_List_From_Config();
        }
    }
}*/
