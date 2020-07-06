using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AccoutingDocs.ViewModels;

namespace AccoutingDocs
{
    /// <summary>
    /// Логика взаимодействия для HeadMenu.xaml
    /// </summary>
    public partial class HeadMenu : Window
    {
        public HeadMenu()
        {
            InitializeComponent();
            App.window = this;
            App.MainMenuDispatcher = this.Dispatcher;
            App.model = (MainMenuViewModel)DataContext;
        }
        /// <summary>
        /// Вызов справки меню
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.F1)
            {
                App.model.Help.Execute(null);
            }
        }
        /// <summary>
        /// Закрытие приложения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!App.model.exec)
                Application.Current.Shutdown();
        }
    }
}
