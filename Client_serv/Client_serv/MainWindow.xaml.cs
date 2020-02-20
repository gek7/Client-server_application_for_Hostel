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
        // {get;} - означает свойство у которого есть только метод get
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

        private void close_app_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

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

        // Облегчённая версия создания страниц, но сложнее в понимании
        private void table_rooms_Click(object sender, RoutedEventArgs e)
        {
            Frame f = GetFreePlaceInMultiPage();
            if (f != null)
            {
                RoomsPage p = new RoomsPage(f);
                f.Navigate(p);
                return;
            }
            else
            {
                // Объявляем экземпляр страницы, которая содержит таблицу комнат, заодно инициализируем отступ сверху в 10 пикселей
                RoomsPage p = new RoomsPage(this) { Margin = new Thickness(0, 10, 0, 0) };

                // Передаём этот экземпляр в функцию, которая принимает обобщённые типы, 
                // В функции с этим типом можно делать только то, что можно делать с 
                // каждым типом данных, потому что компилятор точно не знает, что вы передали
                AddNewTab<RoomsPage>(sender, p);
            }
        }

        // Далее идут однотипные обработчики событий, которые добавляют в TabControl новую вкладку

        private void Table_posts_Click(object sender, RoutedEventArgs e)
        {
            Frame f = GetFreePlaceInMultiPage();
            if (f!=null)
            {
                        PostsPage p = new PostsPage(f);
                        f.Navigate(p);
                        return;
                  
            }
            else
            {
                PostsPage p = new PostsPage(this) { Margin = new Thickness(0, 10, 0, 0) };
                AddNewTab<PostsPage>(sender,p);
            }
        }

        private void BtnGroups_Click(object sender, RoutedEventArgs e)
        {
            Frame f = GetFreePlaceInMultiPage();
            if (f != null)
            {
                GroupsPage p = new GroupsPage(f);
                f.Navigate(p);
                return;
            }
            else
            {
                GroupsPage p = new GroupsPage(this) { Margin = new Thickness(0, 10, 0, 0) };
                AddNewTab(sender, p);
            }
        }

        private void Table_building_Click(object sender, RoutedEventArgs e)
        {
            Frame f = GetFreePlaceInMultiPage();
            if (f != null)
            {
                BuildingsPage b = new BuildingsPage(f);
                f.Navigate(b);
            }
            else
            {
                BuildingsPage p = new BuildingsPage(this) { Margin = new Thickness(0, 10, 0, 0) };
                AddNewTab(sender, p);
            }
        }

        private void table_people_Click(object sender, RoutedEventArgs e)
        {
            Frame f = GetFreePlaceInMultiPage();
            if (f != null)
            {
                PeoplePage p = new PeoplePage(f);
                f.Navigate(p);
            }
            else
            {
                PeoplePage p = new PeoplePage(this) { Margin = new Thickness(0, 10, 0, 0) };
                AddNewTab(sender, p);
            }
        }

        private void Table_RoomTypes_Click(object sender, RoutedEventArgs e)
        {
            Frame f = GetFreePlaceInMultiPage();
            if (f != null)
            {
                RoomTypesPage p = new RoomTypesPage(f);
                f.Navigate(p);
            }
            else
            {
                RoomTypesPage p = new RoomTypesPage(this) { Margin = new Thickness(0, 10, 0, 0) };
                AddNewTab(sender, p);
            }
        }

        private void MultiTable_Click(object sender, RoutedEventArgs e)
        {
            MultiPage p = new MultiPage();
            AddNewTab(sender, p);
        }
        #endregion

      // После названия функции пишется <Любое название>, и после этот тип можно использовать в функции,
      // только функционал будет очень ограничен, т.к тип обобщённый, и компилятор точно не знает, что передано
       private void AddNewTab <T>(object sender,T page )
        {
            // Объявляем новую вкладку (Которая не относится ни к одному TabControl)
            TabItem t = new TabItem();
            // Изменяем название новой вкладки на текст второго элемента(textBlock) в содержимом кнопки
            t.Header = (((sender as Button).Content as Grid).Children[1] as TextBlock).Text;
            // Объявляем Frame, чтобы в него поместить страницу (страница содержит таблицу данных), Frame является своеобразным хранителем страниц
            Frame f = new Frame();
            f.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            // Передаём эту страницу во Frame
            f.Navigate(page);
            // В недавно созданную вкладку кладём новый Frame
            t.Content = f;
            // В TabControl добавляем новую вкладку
            pages.Items.Add(t);
        }

       // Если сейчас открыта вкладка с двумя таблицами, и какой-либо Frame свободен, то вернуть ссылку на свободный Frame
       private Frame GetFreePlaceInMultiPage()
       {
            // В bufFrame хранится ссылка на Frame, который находится в открытой вкладке, иначе null
            //зачем нужно выражение '?.' - https://metanit.com/sharp/tutorial/3.26.php
            Frame bufFrame = ((pages.SelectedItem as TabItem)?.Content as Frame);
            if ((pages.Items.Count > 0) && (bufFrame?.Content is MultiPage))
            {
                MultiPage mul = bufFrame.Content as MultiPage;
                Grid g = (mul.Content as Grid).Children[1] as Grid;
                foreach (var i in g.Children)
                {
                    if ( i is Frame && (i as Frame).Content == null)
                    {
                        PostsPage p = new PostsPage(i as Frame);
                        (i as Frame).Navigate(p);
                        return i as Frame;
                    }
                }
            }
            return null;
        }
    }
}
