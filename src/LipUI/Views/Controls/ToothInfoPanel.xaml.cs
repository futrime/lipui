using System.Windows;
using System.Windows.Controls;

namespace LipUI.Views.Controls
{
    /// <summary>
    /// Interaction logic for ToothInfoPanel.xaml
    /// </summary>
    public partial class ToothInfoPanel : UserControl
    {
        public ToothInfoPanel()
        {
            InitializeComponent();
        }
        //content property
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
                       "Content", typeof(object), typeof(ToothInfoPanel), new PropertyMetadata(default(object)));

        public new object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }
    }
}
