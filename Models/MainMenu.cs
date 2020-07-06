using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using AccoutingDocs.DIalogContents;
using AccoutingDocs.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace AccoutingDocs.Models
{
    public partial class MainMenu
    {
        public MainMenu() 
        {
        }
        public string Title { get; set; }
        public UserControl View { get; set; }

        /// <summary>
        /// Главное меню программы
        /// </summary>
        /// <param name="Role">Роль пользователя</param>
        /// <returns>Коллекцию элементов главного меню</returns>
        public static ObservableCollection<MainMenu> GetMenu(
            string Role)
        {
            var Menu = new ObservableCollection<MainMenu>();
            switch (Role)
            {
                case "Администратор":
                    Menu.Add(new MainMenu { Title = "Пользователи", View = new Main() });
                    Menu.Add(new MainMenu { Title = "Регистр электронных входящих писем", View = new Mail() });
                    Menu.Add(new MainMenu { Title = "Справочники и учетные таблицы", View = new DirectoriesView() });
                    Menu.Add(new MainMenu { Title = "Роли", View = new RolesView() });
                    Menu.Add(new MainMenu { Title = "Учет документов", View = new documents() });
                    Menu.Add(new MainMenu { Title = "Отчеты", View = new ReportView() });
                    Menu.Add(new MainMenu { Title = "Журналы регистрации", View = new RegisterView() });
                    Menu.Add(new MainMenu { Title = "Личный кабинет", View = new ManageView() });
                    break;
                case "Секретарь":
                    Menu.Add(new MainMenu { Title = "Учет документов", View = new documents() });
                    Menu.Add(new MainMenu { Title = "Отчеты", View = new ReportView() });
                    Menu.Add(new MainMenu { Title = "Журналы регистрации", View = new RegisterView() });
                    Menu.Add(new MainMenu { Title = "Личный кабинет", View = new ManageView() });
                    break;
                case "IT-Сотрудник":
                    Menu.Add(new MainMenu { Title = "Регистр электронных входящих писем", View = new Mail() });
                    Menu.Add(new MainMenu { Title = "Справочники и учетные таблицы", View = new DirectoriesView() });
                    Menu.Add(new MainMenu { Title = "Личный кабинет", View = new ManageView() });
                    break;
                case "Руководитель":
                    Menu.Add(new MainMenu { Title = "Учет документов", View = new documents() });
                    Menu.Add(new MainMenu { Title = "Журналы регистрации", View = new RegisterView() });
                    Menu.Add(new MainMenu { Title = "Личный кабинет", View = new ManageView() });
                    break;
                default:
                    Menu.Add(new MainMenu { Title = "Личный кабинет", View = new ManageView() });
                    break;
            };
            return Menu;
        }
        /// <summary>
        /// Панель быстрого доступа
        /// </summary>
        /// <param name="Role">Роль пользователя</param>
        /// <returns>Коллекцию элементов панели быстрого доступа</returns>
        public static ObservableCollection<MainMenu> GetPanel(
            string Role)
        {
            var Menu = new ObservableCollection<MainMenu>();
            switch (Role)
            {
                case "Администратор":
                    Menu.Add(new MainMenu { Title = "Пользователи", View = new Main() });
                    Menu.Add(new MainMenu { Title = "Подразделения", View = new Staffs() });
                    Menu.Add(new MainMenu { Title = "Роли", View = new RolesView() });
                    break;
                case "Секретарь":
                    Menu.Add(new MainMenu { Title = "Учет документов", View = new documents() });
                    Menu.Add(new MainMenu { Title = "Журналы регистрации", View = new RegisterView() });
                    Menu.Add(new MainMenu { Title = "Отчеты", View = new ReportView() });
                    break;
                case "IT-Сотрудник":
                    Menu.Add(new MainMenu { Title = "Регистр электронных входящих писем", View = new Mail() });
                    Menu.Add(new MainMenu { Title = "Справочники и учетные таблицы", View = new DirectoriesView() });
                    Menu.Add(new MainMenu { Title = "Роли", View = new RolesView() });
                    break;
                case "Руководитель":
                    Menu.Add(new MainMenu { Title = "Личный кабинет", View = new ManageView() });
                    Menu.Add(new MainMenu { Title = "Журналы регистрации", View = new RegisterView() });
                    Menu.Add(new MainMenu { Title = "Учет документов", View = new documents() });
                    break;
                default:
                    Menu.Add(new MainMenu { Title = "Личный кабинет", View = new ManageView() });
                    break;
            };
            return Menu;
        }
        /// <summary>
        /// Панель справочников
        /// </summary>
        /// <returns>Список справочников</returns>
        public static ObservableCollection<MainMenu> GetDict()
        {
            var Dict = new ObservableCollection<MainMenu>();
            Dict.Add(new MainMenu { Title = "Статусы", View = new Status_() });
            Dict.Add(new MainMenu { Title = "Подразделения", View = new Staffs() });
            Dict.Add(new MainMenu { Title = "Виды документов", View = new KindView() });
            Dict.Add(new MainMenu { Title = "Организации", View = new OrganizationView() });
            Dict.Add(new MainMenu { Title = "Типы документов", View = new TypeView() });
            return Dict;
        }
    }
}
