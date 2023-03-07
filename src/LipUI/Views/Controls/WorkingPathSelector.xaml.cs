﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LipUI.ViewModels;

namespace LipUI.Views.Controls
{
    /// <summary>
    /// Interaction logic for WorkingPathSelector.xaml
    /// </summary>
    public partial class WorkingPathSelector : UserControl
    {
        public WorkingPathSelectorViewModel ViewModel => (WorkingPathSelectorViewModel)DataContext;
        public WorkingPathSelector()
        {
            InitializeComponent();
            this.DataContext = new WorkingPathSelectorViewModel();
        } 
    }
}
