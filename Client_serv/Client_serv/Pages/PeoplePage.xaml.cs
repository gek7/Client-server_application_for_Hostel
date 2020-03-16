using Client_serv.Dialogs;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
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
using System.Drawing;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Client_serv.Pages
{
    /// <summary>
    /// Логика взаимодействия для PeoplePage.xaml
    /// </summary>
    public partial class PeoplePage : Page,Ipage
    {
        // Окно, которое создало эту страницу. Нужно для удаления вкладки по кнопке (BtnClosePage_Click)
        MainWindow OwnerPage;
        public PeoplePage()
        {
            InitializeComponent();
        }
        public PeoplePage(MainWindow _OwnerPage) : this()
        {
            UpdateGrid();
            OwnerPage = _OwnerPage;
        }

        // SelectID - запись с каким ID сделать выбранной после обновления таблицы
        public void UpdateGrid(int selectID)
        {
            // Вызывается созданный статический метод для сохранения сортировки таблицы данных (после обновления данных сортировка сбрасывается)
            HelperClass.SaveSortDataGrid(dg);
            //Если ID=-1, то изменить параметр selectID на ID записи, которая была выбрана до обновления таблицы
            // Если же её нет, то присвоить -1 | dg.SelectedValue возвращает ID выбранной записи. Про ? и ?? - https://metanit.com/sharp/tutorial/3.26.php
            
            if (selectID == -1) selectID = (int)(dg?.SelectedValue ?? -1);
            // Создания подключения
            using (HOSTELEntities db = new HOSTELEntities())
            {
                // Источник для компонента DataGrid, должен храниться локально, поэтому обращаемся к базе данных,
                // к таблице People, и вызываем метод Load(), который перенесёт данные из БД в db.People.Local()
                db.People.Load();
                // var - неявный тип данных, т.е тип данных становится известен после присвоения какого-либо значени (Обычно используется для сокращения кода)
                var PeopleDB = from p in db.People.Local // Данное выражения звучит примерно так
                               select new                // Из коллекции db.People.Local взять объект p
                               {                         // и на выходе выдать поля:
                                 id = p.PeopleID,        // PeopleID, которое будет называться id
                                 p.Name,                 // Name,BirthDate,PspNum,PhoneNum,Email
                                 p.BirthDate,            
                                 p.PspNum,
                                 p.PhoneNum,
                                 p.Email
                               };

                //selectedValuePath - указывает на то, какое значени будет хранить каждая запись(строка) таблицы
                dg.SelectedValuePath = "id";  // Для этого мы полю PeopleID дали имя id, если не давать имя, то нужно использовать названия поля (PeopleID)
                // Указываем источник данных для таблицы данных, это коллекция, которая была инициализирована выше
                dg.ItemsSource = PeopleDB; 
            }
            // Выше мы указали, что каждая строка хранит id записи, которую она хранит
            // dg.SelectedValue - указывает на id выбранной записи, также через это поле мы можем указать id записи, которую выбрать. Если записи с таким id нет, то ничего не произойдёт
            dg.SelectedValue = selectID;
            // -1 = ничего не выбрано, след-но условие: если выбрана какая-нибудь запись
            if (dg.SelectedIndex > -1)
            {
                dg.ScrollIntoView(dg.SelectedItem); // Прокрутить до выбранной записи
            }
            else if (dg.SelectedIndex == -1 && dg.Items.Count > 0)
            {
                dg.SelectedIndex = 0;
            } 
            HelperClass.LoadSortDataGrid(dg); // В самом начале мы сохраняли сортировку, теперь выгружаем обратно.
            // Вызвать функцию для обновления картинки
            UpdateImage();
        }

        // Этот метод вызывается из MainWindow для обновления данных при фокусе на эту страницу
        public void UpdateGrid()
        {
            UpdateGrid(-1);
        }

        // Обработчик клика кнопки 'Добавить'
        private void add_Click(object sender, RoutedEventArgs e)
        {
            // Создать экземпляр диалогового окна, передать туда действие (Перечисление) и ссылку на эту страницу.
            PeopleDialog dialog = new PeopleDialog(mode.Add, this);
            // Показать диалоговое окно
            dialog.ShowDialog();
        }

        private void copy_Click(object sender, RoutedEventArgs e)
        {
            // Если выбрана какая-нибудь запись
            if (dg.SelectedIndex > -1)
            {
                                                     //Действие, ссылка на эту страницу, id выбранной записи
                PeopleDialog dialog = new PeopleDialog(mode.Copy, this, (int)dg.SelectedValue);
                dialog.ShowDialog();
            }
            else
            {
                // Вывести диалоговое окно:
                MessageBox.Show("Не выбрано поле!");
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (dg.SelectedIndex > -1)
            {
                PeopleDialog dialog = new PeopleDialog(mode.Update, this, (int)dg.SelectedValue);
                dialog.ShowDialog();
            }
            else
            {
                MessageBox.Show("Не выбрано поле!");
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (dg.SelectedIndex > -1)
            {
                // MessageBox.Show() возвращает результат, который указывает куда нажал пользователь
                // У MessageBox.Show() можно указать набор кнопок и иконка, которая будет появляться на диалог. окне
                if (MessageBoxResult.Yes == MessageBox.Show("Вы уверены, что хотите удалить запись?", "Удаление записи",
                    MessageBoxButton.YesNo, MessageBoxImage.Question))
                {
                    using (HOSTELEntities DB = new HOSTELEntities())
                    {
                        // Конструкция try/catch для отслеживания ошибок в удалении записи
                        try
                        {
                            //Запись в таблице можно найти с помощью первичного ключа, используя метод Find
                            DB.People.Remove(DB.People.Find(dg.SelectedValue));
                            DB.SaveChanges();
                        }
                        catch
                        {
                            MessageBox.Show($"Ошибка при удалении \n Возможно в других таблицах есть ссылки на эту запись");
                            return;
                        }
                    }
                    UpdateGrid();
                }
            }
            else
            {
                MessageBox.Show("Не выбрано поле!");
            }
        }

        private void BtnClosePage_Click(object sender, RoutedEventArgs e)
        {
            // Обратиться к MainWindow, к TabControl, и удалить вкладку, которая сейчас активна
            OwnerPage.pages.Items.Remove(OwnerPage.pages.SelectedItem);
        }

        // Событие, которое срабатывает при изменении выбранной записи
        private void dg_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Обновить картинку
            UpdateImage();
        }

        private void UpdateImage()
        {
            using (HOSTELEntities db = new HOSTELEntities())
            {
                // Найти в таблице People, запись, по ID, которое сейчас выбранно в таблице, и взять оттуда данные картинки
                byte[] imgData = db.People?.Find(dg.SelectedValue)?.ImageData;
                // Если данные картинки содержат что-нибудь
                if (imgData != null)
                {
                    // MemoryStream - используется для чтения или записи массива байт (Картинка - набор байт)
                    // Сразу инициализируем массивом байт будущей картинки
                    MemoryStream stream = new MemoryStream(imgData);
                    // img - картинка, которую мы будем передавать на форму
                    BitmapImage img = new BitmapImage();
                    // Инициализация картинки
                    img.BeginInit();
                    img.StreamSource = stream;
                    img.EndInit();
                    // Указываем источник для картинки на форме, созданный на форме картинку из БД
                    pict.Source = img;
                }
                else
                {
                    // Если картинки в БД нет, то вывести лицо со знаком вопроса.
                    if (pict.Source.ToString() != "pack://application:,,,/Images/unknownImage.png")
                    {
                        BitmapImage unknwn = new BitmapImage(new Uri("/Images/unknownImage.png", UriKind.Relative));
                        pict.Source = unknwn;
                    }
                }
            }
        }
    }
}
