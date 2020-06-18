using Client_serv.Pages;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Client_serv.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для PeopleDialog.xaml
    /// </summary>
    public partial class PeopleDialog : Window
    {
        PeoplePage CurPage;
        // Перечисление хранит какое действие выбрал пользователь
        mode CurMode;
        // ID хранит id записи с которой работаем
        int ID;
        public PeopleDialog()
        {
            InitializeComponent();
        }

        public PeopleDialog(mode dialogMode, PeoplePage page, int fieldID = -1) : this()
        {
            CurMode = dialogMode;
            ID = fieldID;
            CurPage = page;
            // Метод для заполнение полей диалогового окна (если нужно)
            FillFields();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {

            if (Check() && Save())
            {
                Close();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // Проверки на правильность введённых данных
        bool Check()
        {
            if (TbName.Text.Trim() == "")
            {
                MessageBox.Show("Человек не может быть без ФИО! Заполните поле 'ФИО'");
                return false;
            }
            if (cbSexes.SelectedIndex == -1)
            {
                MessageBox.Show("Человек не может быть без пола! Выберите корпус");
                return false;
            }
             Regex r = new Regex(@"[\d-()+]+");
            var m = r.Matches(TbPhoneNumber.Text);
            if (m.Count!=1 || m[0].Length!=TbPhoneNumber.Text.Length)
            {
                MessageBox.Show("Поле 'Номер телефона' должно быть заполнено только цифрами,\n также допустимы символы -,+,(,)");
                return false;
            }
             r = new Regex(@"^([a-z0-9_-]+\.)*[a-z0-9_-]+@[a-z0-9_-]+(\.[a-z0-9_-]+)*\.[a-z]{2,6}$");
             m = r.Matches(TbEmail.Text);
            if (m.Count != 1 || m[0].Length != TbEmail.Text.Length)
            {
                MessageBox.Show("Поле 'почта' заполнена неправильно");
                return false;
            }
            return true;
        }

        // Сохранение изменений в базу данных
        bool Save()
        {
            try
            {
                using (HOSTELEntities db = new HOSTELEntities())
                {
                    People r = new People();
                    switch (CurMode)
                    {
                        case mode.Add:
                            // Переход в ветку Copy
                            goto addRoom;

                        case mode.Copy:
                        addRoom:
                            // Заполняет объект r значениями из полей диалог. окна
                            FillObject(r);
                            // Добавляем объект в таблицу People
                            db.People.Add(r);
                            break;

                        case mode.Update:
                            // Находим запись таблицы People по id
                            r = db.People.Find(ID);
                            // Заполняем её значениями из полей диалог. окна
                            FillObject(r);
                            break;
                    }
                    // Сохраняем изменения
                    db.SaveChanges();
                    // Обновляем страницу, которая вызвала это диалог. окно
                    CurPage.UpdateGrid(r.PeopleID);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Ошибка при сохранении \n {e.Message}");
                return false;
            }
            return true;
        }

        private void FillFields()
        {
            // Открывается соединени
            using (HOSTELEntities db = new HOSTELEntities())
            {
                // Подгружаем таблицу Sexes, чтобы она хранилась локально
                db.Sexes.Load();
                // Источником для комбобокса указываем таблицу Sexes
                cbSexes.ItemsSource = db.Sexes.Local;
                // Элемент комбобокса будет отображать значение поля Sex
                cbSexes.DisplayMemberPath = "Sex";
                // Элемент комбобокса будет хранить значение поля SexID
                cbSexes.SelectedValuePath = "SexID";
                if (cbSexes.Items.Count > 0) cbSexes.SelectedIndex = 0;
                // Установка заголовка в зависимости от действия, которое выбрал пользователь (Удаление, изменение, добавление)
                switch (CurMode)
                {
                    case mode.Add:
                        Title = "Добавление";
                        break;

                    case mode.Copy:
                        Title = "Добавление на основе существующего";
                        goto fill;

                    case mode.Update:
                        Title = "Изменение";
                    fill:
                        // Находим запись с ID, который передали в диалоговое окно при создании
                        People p = db.People.Find(ID);
                        // Далее заполняем значениями текстбоксы, dateTimePicker, и выбираем значение у комбобокса
                        TbName.Text = p.Name;
                        dpBirthDate.SelectedDate = p.BirthDate;
                        cbSexes.SelectedValue = p.SexID;
                        TbPsp.Text = p.PspNum;
                        TbPhoneNumber.Text = p.PhoneNum;
                        TbEmail.Text = p.Email;
                        // Подгрузка картинки
                        if (p.ImageData != null)
                        {
                            // MemoryStream - используется для чтения или записи массива байт (Картинка - набор байт)
                            // Сразу инициализируем массивом байт будущей картинки
                            MemoryStream stream = new MemoryStream(p.ImageData);
                            // img - картинка, которую мы будем передавать на форму
                            BitmapImage img = new BitmapImage();
                            // Инициализация картинки
                            img.BeginInit();
                            img.StreamSource = stream;
                            img.EndInit();
                            // Указываем источник для картинки на форме, созданный на форме картинку из БД
                            Picture.Source = img;
                        }
                        break;
                }
            }
        }
        // Этот метод заполняет объект значениями из полей диалогового окна
        private void FillObject(People p)
        {
            using(HOSTELEntities db = new HOSTELEntities())
            {
                p.Name = TbName.Text;
                p.BirthDate = dpBirthDate.SelectedDate;
                p.SexID =(int?)cbSexes.SelectedValue;
                p.PspNum = TbPsp.Text;
                p.PhoneNum = TbPhoneNumber.Text;
                p.Email = TbEmail.Text;
                // Картинка => массив байтов
                if(Picture.Source.ToString() != "pack://application:,,,/Images/unknownImage.png")
                {
                    MemoryStream memStream = new MemoryStream();
                    // JpegBitmapEncoder - кодирует изображения формата jpeg  
                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    // Добавлем к кадрам, которые нужно закодировать нашу картинку с формы, предварительно приведя к типу BitMapFrame
                    encoder.Frames.Add(BitmapFrame.Create(Picture.Source as BitmapImage));
                    // Кодирует изображение и сохраняет в поток memStream
                    encoder.Save(memStream);
                    // Поток преобразуем в массив байт, и передаём в объект, полученный через параметры
                    p.ImageData=memStream.ToArray();
                }
            }
        }

        // обработчик клика кнопки 'Выбрать изображение'
        private void selectImage_Click(object sender, RoutedEventArgs e)
        {
            // Открыватся окно для выбора файла
            OpenFileDialog ofd = new OpenFileDialog();
            // Устанавливаем какие расширения файлов можно выбирать - "Название файлов | *.расширение"
            ofd.Filter = "Jpeg files |*.jpg";
            // Устанавливаем заголовок
            ofd.Title = "Выберите изображение";
            // Если пользователь нажал кнопку ок, то возвращается true, иначе false
            if(ofd.ShowDialog()?? false)
            {
                // Uri указатель на файл, в параметрах передаётся путь до файла и тип указателя абсолютный
                // т.е передаётся весь путь до файла. Есть ещё тип Relative, тогда нужно путь указывать относительно расположения приложения
                Uri u = new Uri(ofd.FileName, UriKind.Absolute);
                // Создаём в памяти BitmapImage и передаём туда указатель на картинку, которую выбрал польз-ль и делаем её источником для объекта Image в диалог. окне
                Picture.Source = new BitmapImage(u);
            }
        }
    }
}
