using Microsoft.Win32;
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
using System.Windows.Shapes;

namespace Client_serv.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для PeopleDialog.xaml
    /// </summary>
    public partial class PeopleDialog : Window
    {

        Ipage CurPage;
        mode CurMode;
        int ID;
        public PeopleDialog()
        {
            InitializeComponent();
        }

        public PeopleDialog(mode dialogMode, Ipage page, int fieldID = -1) : this()
        {
            CurMode = dialogMode;
            ID = fieldID;
            CurPage = page;
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
            if (TbPhoneNumber.Text.Trim() == "")
            {
                MessageBox.Show("поле 'Номер телефона' должно быть заполнено");
                return false;
            }
            return true;
        }

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
                            goto addRoom;

                        case mode.Copy:
                        addRoom:
                            FillObject(r);
                            db.People.Add(r);
                            break;

                        case mode.Update:
                            r = db.People.Find(ID);
                            FillObject(r);
                            break;
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Ошибка при сохранении \n {e.Message}");
                return false;
            }
            CurPage.updateGrid();
            return true;
        }

        private void FillFields()
        {
            using (HOSTELEntities db = new HOSTELEntities())
            {
                db.Sexes.Load();
                cbSexes.ItemsSource = from i in db.Sexes.Local select new { i.SexID, i.Sex };
                cbSexes.DisplayMemberPath = "Sex";
                cbSexes.SelectedValuePath = "SexID";
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
                        People p = db.People.Find(ID);
                        TbName.Text = p.Name;
                        dpBirthDate.SelectedDate = p.BirthDate;
                        cbSexes.SelectedValue = p.SexID;
                        TbPsp.Text = p.PspNum;
                        TbPhoneNumber.Text = p.PhoneNum;
                        TbEmail.Text = p.Email;
                        // Подгрузка картинки
                        if (p.ImageData != null)
                        {
                            MemoryStream stream = new MemoryStream(p.ImageData);
                            BitmapImage img = new BitmapImage();
                            img.BeginInit();
                            img.StreamSource = stream;
                            img.EndInit();
                            Picture.Source = img;
                        }
                        break;
                }
            }
        }

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
                if(Picture.Source.ToString() != "pack://application:,,,/Images/unknownImage.png")
                {
                    MemoryStream memStream = new MemoryStream();
                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(Picture.Source as BitmapImage));
                    encoder.Save(memStream);
                    p.ImageData=memStream.ToArray();
                }
            }
        }

        private void selectImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if(ofd.ShowDialog()?? false)
            {
                Picture.Source = new BitmapImage(new Uri(ofd.FileName,UriKind.Absolute));
            }
        }
    }
}
