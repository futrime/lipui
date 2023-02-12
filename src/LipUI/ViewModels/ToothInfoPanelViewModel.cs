using CommunityToolkit.Mvvm.ComponentModel;
namespace LipUI.ViewModels
{
    internal partial class ToothInfoPanelViewModel : ObservableObject
    {
        private LipNETWrapper.Class.LipPackage _info;
        public ToothInfoPanelViewModel(LipNETWrapper.Class.LipPackage info)
        {
            _info = info;
        }
        public string Author => _info.Author;
        public string Description => _info.Description;
        public string Homepage => _info.Homepage;
        public string Name => _info.Name;
        public string Tooth => _info.Tooth;
        public string Version => _info.Version;

        //[JsonProperty("files")] public LipFile[] Files { get; set; } 
        //[JsonProperty("versions")] public string[] Versions { get; set; }
    }
}
