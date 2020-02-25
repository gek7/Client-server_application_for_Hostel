using Client_serv.Pages;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Client_serv
{
    // enum - перечисление. Перечисление - это набор констант
    // По умолчанию константы используют тип int. Можно самому указать любой целочисленный тип.
    // Пример: public enum mode : byte
    public enum mode
    {
        // Дальше объявляются константы, которые можно привести к числу. 
        // Первая константа равна 0, также можно явно объявить чему будет равна константа
        // Значение след. константы = значение предыдущей константы + 1 
        Add, // Add = 0
        Copy=25,
        Update,  // Update = 26
    }

    public partial class MainWindow : Window
    {
        DispatcherTimer timeTimer = new DispatcherTimer();
        // {get;} - означает свойство у которого есть только метод get
        public TabControl MainTabControl { get; }
        // Переменные и методы с модификатором static относятся к классу, т.е необязательно создавать экземпляр класса, чтобы обращаться к методам и переменным с модификатором static
        // Заранее поместив строку подключения в файл App.config, мы можем получать строку по её имени.
        public static string connectionString = ConfigurationManager.ConnectionStrings["AdoNet"].ConnectionString;
        public MainWindow()
        {
            InitializeComponent();
            // Каждые 0.5 секунды будет выполняться функция TimeTimerTick
            // += значит, что при выполнение какого-то события в данном случае Tick будет выполняться какой либо обработчик события (TimeTimerTick)
            timeTimer.Tick += TimeTimer_Tick;
            // Установка интервала в полсекунды
            timeTimer.Interval = TimeSpan.FromSeconds(0.5);
            // Запуск таймера
            timeTimer.Start();
            // В textblock присвоить текущее время
            CurTime.Text = DateTime.Now.ToString();
        }

        #region Обработчики событий

        // Выполняется каждые 0.5 секунд
        private void TimeTimer_Tick(object sender, EventArgs e)
        {
            // В textblock(Находится в статус баре MainWindow.xaml) присвоить текущее время
            CurTime.Text = DateTime.Now.ToString();
        }

        private void close_app_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

         //Событие, срабатывающее при смене вкладки (Нужно, чтобы оставалась сортировка)
        private void pages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Если выбрана какая-нибудь вкладка и событие произло в объекте TabControl
            // События могут происходить во вложенных объектах и это событие тоже будет срабатывать
            // Вот подробнее - https://metanit.com/sharp/wpf/6.php
            if (pages.SelectedItem != null && (e.OriginalSource is TabControl))
            {
                // Создать экземпляр интерфейса и присвоить туда ссылку на содержимое выбранной вкладки
                // (Чтобы посмотреть для чего нужен этот интерфейс зажмите ctrl и нажмите на Ipage)
                Ipage ipage = ((pages.SelectedItem as TabItem).Content as Frame).Content as Ipage;
                    // Обновление таблицы данных у вкладки, которая сейчас открыта
                    ipage.UpdateGrid();
            }
        }

        private void table_rooms_Click(object sender, RoutedEventArgs e)
        {
                // Объявляем экземпляр страницы, которая содержит таблицу комнат, заодно инициализируем отступ сверху в 10 пикселей
                RoomsPage p = new RoomsPage(this) { Margin = new Thickness(0, 10, 0, 0) };
                // В метод AddNewTab передаём ссылку на страницу и название вкладки, в которой будет находиться эта страница
                AddNewTab(p,"Комнаты" );
            
        }

        // Далее идут однотипные обработчики событий, которые добавляют в TabControl новую вкладку

        private void Table_posts_Click(object sender, RoutedEventArgs e)
        {
                PostsPage p = new PostsPage(this) { Margin = new Thickness(0, 10, 0, 0) };
                AddNewTab(p, "Должности");
        }

        private void BtnGroups_Click(object sender, RoutedEventArgs e)
        {
                GroupsPage p = new GroupsPage(this) { Margin = new Thickness(0, 10, 0, 0) };
                AddNewTab(p, "Группы");
        }

        private void Table_building_Click(object sender, RoutedEventArgs e)
        {
                BuildingsPage p = new BuildingsPage(this) { Margin = new Thickness(0, 10, 0, 0) };
                AddNewTab(p, "Корпуса");
        }

        private void table_people_Click(object sender, RoutedEventArgs e)
        {
                PeoplePage p = new PeoplePage(this) { Margin = new Thickness(0, 10, 0, 0) };
                AddNewTab(p, "Люди");
        }

        private void Table_RoomTypes_Click(object sender, RoutedEventArgs e)
        {
                RoomTypesPage p = new RoomTypesPage(this) { Margin = new Thickness(0, 10, 0, 0) };
                AddNewTab(p, "Виды комнат");
        }

        private void MultiTable_Click(object sender, RoutedEventArgs e)
        {
            MultiPage p = new MultiPage();
            AddNewTab(p, "Корпуса/Комнаты");
        }
        #endregion

      // Первым параметром передаётся страница, т.к всё наследуется от object,
      // то мы можем любую страницу передать через этот параметр
       private void AddNewTab (object page,string TabHeader )
        {
            // Объявляем новую вкладку (Которая сейчас не относится ни к одному TabControl)
            TabItem t = new TabItem();
            // Присваиваем названию вкладки значение параметра TabHeader
            t.Header = TabHeader;
            // Объявляем Frame, чтобы в него поместить страницу (страница содержит таблицу данных), Frame является своеобразным хранителем страниц
            Frame f = new Frame();
            // скрываем навигационный интерфейс у фрейма (Стрелки вперёд и назад* Фрейм может переключаться по страницам вперёд и назад как браузер, сейчас нам это не нужно)
            f.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            // Мы можем без проблем передать во Frame ссылку на страницу, так как метод Navigate принимает тип данных object
            f.Navigate(page);
            // В созданную вкладку кладём новый Frame
            t.Content = f;
            // В TabControl добавляем новую вкладку
            pages.Items.Add(t);
        }
    }
}
