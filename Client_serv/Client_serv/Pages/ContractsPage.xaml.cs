using Client_serv.Dialogs;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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

namespace Client_serv.Pages
{
    /// <summary>
    /// Логика взаимодействия для ContractsPage.xaml
    /// </summary>
    public partial class ContractsPage : Page
    {
        MainWindow OwnerPage;

        public ContractsPage()
        {
            InitializeComponent();
            dg.MouseDoubleClick += Dg_MouseDoubleClick;
        }

        private void Dg_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ContractsDialog dialog = new ContractsDialog(mode.Update, this, (int)dg.SelectedValue);
            dialog.ShowDialog();
        }

        public ContractsPage(MainWindow _OwnerPage) : this()
        {
            UpdateGrid();
            OwnerPage = _OwnerPage;
        }


        public void UpdateGrid(int selectID)
        {
            HelperClass.SaveSortDataGrid(dg);

            if (selectID == -1) selectID = (int)(dg?.SelectedValue ?? -1);
            using (HOSTELEntities h = new HOSTELEntities())
            {
                // Подгрузка таблиц ( После этого h.Таблица.Local возвращает значения из таблицы на момент вызова Load() )
                h.People.Load();
                h.Rooms.Load();
                h.Contracts.Load();
                h.Buildings.Load();
                // var - неявный "тип", т.е неизвестный тип данных. Тип становится известным только после присвоения любого значения. 
                // Присваивать значения нужно во время объявления переменной, иначе выдаст ошибку
                var ContractsDB = from i in h.Contracts.Local
                                  select new
                                  {
                                      id = i.ContractID,
                                      Surname = i.People.Name,
                                      DocDate = i.DocDate,
                                      PlanBegDate = i.PlanBegDate,
                                      PlanEndDate = i.PlanEndDate,
                                      Priority = i.Priority,
                                      Signed = i.Signed,
                                      ActualEndDate = i.ActualEndDate,
                                      Room = i.Rooms.Num+" Комната "+i.Rooms.Buildings.Name,
                                      AppDate = i.AppDate
                                };
                dg.SelectedValuePath = "id";
                dg.ItemsSource = ContractsDB;
            }
            dg.SelectedValue = selectID;
            if (dg.SelectedIndex > -1)
            {
                dg.ScrollIntoView(dg.SelectedItem);
            }
            else if (dg.SelectedIndex == -1 && dg.Items.Count > 0)
            {
                dg.SelectedIndex = 0;
            }
            HelperClass.LoadSortDataGrid(dg);
        }

        public void UpdateGrid()
        {
            UpdateGrid(-1);
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            ContractsDialog dialog = new ContractsDialog(mode.Add, this);
            dialog.ShowDialog();
        }

        private void copy_Click(object sender, RoutedEventArgs e)
        {
            if (dg.SelectedIndex > -1)
            {
                ContractsDialog dialog = new ContractsDialog(mode.Copy, this, (int)dg.SelectedValue);
                dialog.ShowDialog();
            }
            else
            {
                MessageBox.Show("Не выбрано поле!");
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (dg.SelectedIndex > -1)
            {
                ContractsDialog dialog = new ContractsDialog(mode.Update, this, (int)dg.SelectedValue);
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
                if (MessageBoxResult.Yes == MessageBox.Show("Вы уверены, что хотите удалить запись?", "Удаление записи",
                    MessageBoxButton.YesNo, MessageBoxImage.Question))
                {
                    using (HOSTELEntities DB = new HOSTELEntities())
                    {
                        try
                        {
                            DB.Contracts.Remove(DB.Contracts.Find(dg.SelectedValue));
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
            OwnerPage.pages.Items.Remove(OwnerPage.pages.SelectedItem);
        }
    }
}
