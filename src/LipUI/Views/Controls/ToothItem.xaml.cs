using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using Wpf.Ui.Controls.Interfaces;

namespace LipUI.Views.Controls
{
    /// <summary>
    /// Interaction logic for ToothItem.xaml
    /// </summary>
    public partial class ToothItem
    {
        public ViewModels.ToothItemViewModel ViewModel => (ViewModels.ToothItemViewModel)DataContext;
        public ToothItem()
        {
            InitializeComponent();
        }
        private void Timeline_OnCompleted(object sender, EventArgs e)
        {
            try
            {
                DoubleAnimation animation = new DoubleAnimation();
                animation.From = main.ActualHeight;
                animation.To = 0;
                animation.Duration = TimeSpan.FromSeconds(1);

                Storyboard storyboard = new Storyboard();
                storyboard.Children.Add(animation);

                Storyboard.SetTarget(animation, main);
                Storyboard.SetTargetProperty(animation, new PropertyPath(HeightProperty));
                main.Resources.Add("animation", storyboard);
                storyboard.Begin();
                storyboard.Completed += (s, _) =>
                {
                    //main.Visibility = Visibility.Collapsed;
                    try
                    {
                        main.Resources.Remove("animation");

                    }
                    catch
                    {
                        // ignored
                    }
                };
            }
            catch
            {
                // ignored
            }

        }
    }
}
