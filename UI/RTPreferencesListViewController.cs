using System.Linq;
using HMUI;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using BeatSaberMarkupLanguage.Components.Settings;
using System.ComponentModel;

namespace JDFixer.UI
{
    public class RTPreferencesListViewController : BSMLResourceViewController, INotifyPropertyChanged
    {
        public override string ResourceName => "JDFixer.UI.BSML.rtPreferencesList.bsml";
        //public event PropertyChangedEventHandler propertyChanged;


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


        [UIValue("min_rt_slider")]
        private int Min_RT_Slider => PluginConfig.Instance.minReactionTime;
        [UIValue("max_rt_slider")]
        private int Max_RT_Slider => PluginConfig.Instance.maxReactionTime;

        [UIComponent("rt_slider")]
        private SliderSetting RT_Slider;

        private float _new_rt = 500f;

        [UIValue("rt_value")]
        public float RT_Value
        {
            get => _new_rt;
            set
            {
                _new_rt = value;
            }
        }

        [UIAction("set_rt")]
        void Set_RT(float value)
        {
            RT_Value = value;
        }

        [UIAction("rt_slider_formatter")]
        private string RT_Slider_Formatter(float value) => value.ToString("0") + " ms";


        [UIComponent("pref_list")]
        public CustomListTableData prefList;
        private RTPref _selectedPref = null;

        [UIAction("select_pref")]
        private void Select_Pref(TableView tableView, int row)
        {
            //Logger.log.Debug("Selected row " + row);

            _selectedPref = PluginConfig.Instance.rt_preferredValues[row];
            //propertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(prefIsSelected)));
        }


        [UIAction("add_pressed")]
        private void Add_Pressed()
        {
            if (PluginConfig.Instance.rt_preferredValues.Any(x => x.njs == _new_njs))
            {
                PluginConfig.Instance.rt_preferredValues.RemoveAll(x => x.njs == _new_njs);
            }
            PluginConfig.Instance.rt_preferredValues.Add(new RTPref(_new_njs, _new_rt));
            Reload_List_From_Config();
        }


        [UIAction("remove_pressed")]
        private void Remove_Pressed()
        {
            if (_selectedPref == null)
                return;

            PluginConfig.Instance.rt_preferredValues.RemoveAll(x => x == _selectedPref);
            Reload_List_From_Config();
        }


        private void Reload_List_From_Config()
        {
            prefList.data.Clear();

            if (PluginConfig.Instance.rt_preferredValues == null)
                return;

            PluginConfig.Instance.rt_preferredValues.Sort((x, y) => y.njs.CompareTo(x.njs));

            foreach (var pref in PluginConfig.Instance.rt_preferredValues)
            {
                prefList.data.Add(new CustomListTableData.CustomCellInfo($"{pref.njs} NJS | {pref.reactionTime} ms"));
            }

            prefList.tableView.ReloadData();
            prefList.tableView.ClearSelection();
            _selectedPref = null;
            //prefIsSelected = prefIsSelected;
            //prefIsSelected = false;
            //propertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(prefIsSelected)));
        }


        //----------------------------------------------------------------------------

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
            if (!firstActivation)
            {
                Reload_List_From_Config();
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
}
