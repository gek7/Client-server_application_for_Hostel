using Client_serv.Pages;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
        // {get;} - означает свойство у котороего есть только метод get
        public TabControl MainTabControl { get; }
        // Переменные и методы с модификатором static относятся к классу, т.е необязательно создавать экземпляр класса, чтобы обращаться к методам и переменным с модификатором static
        public static string connectionString = "Data Source=DESKTOP-CS9U3G2;Initial Catalog=HOSTEL;Integrated Security=True";
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

        // Выполняется при нажатии кнопки выход
        private void close_app_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // Далее идут однотипные обработчики событий, которые добавляют в TabControl новую вкладку
        private void table_rooms_Click(object sender, RoutedEventArgs e)
        {
            // Объявляем новую вкладку (Которая не относится ни к одному TabControl)
            TabItem t = new TabItem();
            // Изменяем название новой вкладки
            t.Header = "Комнаты";
            // Объявляем Frame, чтобы в него поместить страницу (страница содержит таблицу данных), Frame является своеобразным хранителем страниц
            Frame f = new Frame();
            // Объявляем экземпляр страницы, которая содержит таблицу комнат
            RoomsPage p = new RoomsPage(this);
            // Устанавлиаем отступ в 10 пикселей сверху
            p.Margin = new Thickness(0, 10, 0, 0);
            // Передаём эту страницу во Frame
            f.Navigate(p);
            // В недавно созданную вкладку ложим новый Frame
            t.Content = f;
            // В TabControl добавляем новую вкладку
            pages.Items.Add(t);
        }

        private void Table_posts_Click(object sender, RoutedEventArgs e)
        {
            TabItem t = new TabItem();
            t.Header = "Должности";
            Frame f = new Frame();
            PostsPage p = new PostsPage(this);
            p.Margin = new Thickness(0, 10, 0, 0);
            f.Navigate(p);
            t.Content = f;
            pages.Items.Add(t);
        }

        private void BtnGroups_Click(object sender, RoutedEventArgs e)
        {
            TabItem t = new TabItem();
            t.Header = "Группы";
            Frame f = new Frame();
            GroupsPage p = new GroupsPage(this);
            p.Margin = new Thickness(0, 10, 0, 0);
            f.Navigate(p);
            t.Content = f;
            pages.Items.Add(t);
        }

        private void Table_building_Click(object sender, RoutedEventArgs e)
        {
            TabItem t = new TabItem();
            t.Header = "Корпуса";
            Frame f = new Frame();
            BuildingsPage b = new BuildingsPage(this);
            f.Navigate(b);
            t.Content = f;
            pages.Items.Add(t);
        }

        private void table_people_Click(object sender, RoutedEventArgs e)
        {
            TabItem t = new TabItem();
            t.Header = "Люди";
            Frame f = new Frame();
            PeoplePage p = new PeoplePage(this);
            p.Margin = new Thickness(0, 10, 0, 0);
            f.Navigate(p);
            t.Content = f;
            pages.Items.Add(t);
        }
        #endregion

        // Событие, срабатывающее при смене вкладки (Нужно, чтобы оставалась сортировка)
        private void pages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Если выбрана какая-нибудь вкладка
            if (pages.SelectedItem != null)
            {
                // Создать экземпляр интерфейса и присвоить туда ссылку на содержимое выделенной вкладки
                // (Чтобы посмотреть для чего нужен этот интерфейс зажмите ctrl и нажмите на Ipage)
                Ipage ipage = ((pages.SelectedItem as TabItem).Content as Frame).Content as Ipage;

                // в dg присваивается ссылка на таблицу данных вкладки, которая сейчас открыта
                DataGrid dg = ipage.PageDataGrid;

                // Сохраняет визуальную составляющую сортировки (Стрелочки сверху колонок) до обновления таблицы
                // Queue - это очередь. Работает по принципу FIFO (Первым вошел, первым вышел)
                // ListSortDirection - это тип данных, который есть у каждого столбца. 
                // ? после типа данных означает, что он может принимать NULL значения
                Queue<ListSortDirection?> SortQueueColumns = new Queue<ListSortDirection?>
                    (
                        // Здесь происходит инициализация коллекции
                        // dg.Columns - это коллекция колонок, что-то вроде динамического массива
                        // Если вызвать метод Select, то можно привести все объекты коллекции к другому виду
                        // tmp - это любой объект коллекции. Это название может быть любым другим (Не может повторять названия уже объвяленных переменных)
                        // После знака '=>' (Лямбда-оператор) написанно к какому виду привести все объекты коллекции
                        // В данном случае select вернёт новую коллекцию содержащую направление каждой колонки
                        dg.Columns.Select(tmp => tmp.SortDirection)
                    );
                // Объявление коллекции, которая в будущем будет хранить по каким полям сортировать и в каком порядке
                SortDescriptionCollection SortList = new SortDescriptionCollection();

                // Добавляет в коллекцию SortList сортировку каждой колонки до обновления таблицы данных
                foreach (var i in dg.Items.SortDescriptions) SortList.Add(i);

                // Обновление таблицы данных у вкладки, которая сейчас открыта
                ipage.updateGrid();

                // В следующий двух циклах в таблицу данных обратно присваиваются значения сортировки, сохранённые до обновления данных
                for (int i = 0; i < SortList.Count; i++) dg.Items.SortDescriptions.Add(SortList[i]);
                for (int i = 0; i < dg.Columns.Count; i++) dg.Columns[i].SortDirection = SortQueueColumns.Dequeue();
            }
        }
    }
}
