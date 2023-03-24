using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LipNETWrapper.Class.Tooth;

namespace LipUI.ViewModels
{
    public partial class ToothJsonViewModel : ObservableObject
    {
        public ToothJsonViewModel(ToothJson data)
        {
            _toothJson = data;
        }
        [ObservableProperty] private ToothJson _toothJson;
        [RelayCommand]
        private void RemoveDependency(string key)
        {
            ToothJson.Dependencies.Remove(key);
        }
        [RelayCommand]
        private void RemoveDependencyVersion(string key )
        {
            //ToothJson.Dependencies[key].RemoveAt(key);
        }
        [RelayCommand]
        private void AddDependencyVersion(string key)
        {
            if (!ToothJson.Dependencies.ContainsKey(key))
            {
                ToothJson.Dependencies[key] = new();
            }
            ToothJson.Dependencies[key].Add(new());
        }
        [RelayCommand]
        private void AddDependency()
        {
            ToothJson.Dependencies.Add("new_dependency", new());
        }
    }
}
