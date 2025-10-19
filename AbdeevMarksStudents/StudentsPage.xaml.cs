using System;
using System.Collections.Generic;
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

namespace AbdeevMarksStudents
{
    /// <summary>
    /// Логика взаимодействия для StudentsPage.xaml
    /// </summary>
    public partial class StudentsPage : Page
    {
        public StudentsPage()
        {
            InitializeComponent();
            var currentStudents = AbdeevMarksStudentsEntities.GetContext().STUDENTS.ToList();
            StudentsListView.ItemsSource = currentStudents;
            SortComboBox.SelectedIndex = 0;
            FilterComboBox.SelectedIndex = 0;
            UpdateStudents();
        }

        private void SearchTB_SelectionChanged(object sender, RoutedEventArgs e)
        {
            UpdateStudents();
        }

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateStudents();
        }

        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateStudents();
        }

        private void UpdateStudents()
        {
            int CountRecords = AbdeevMarksStudentsEntities.GetContext().STUDENTS.ToList().Count;
            TBAllRecords.Text = " из " + CountRecords.ToString();

            var currentStudents = AbdeevMarksStudentsEntities.GetContext().STUDENTS.ToList();
            if (SortComboBox.SelectedIndex == 1)
            {
                currentStudents = currentStudents.OrderBy(r => r.FIO_STUDENT).ToList();
            }
            if (SortComboBox.SelectedIndex == 2)
            {
                currentStudents = currentStudents.OrderByDescending(r => r.FIO_STUDENT).ToList();
            }
            if (SortComboBox.SelectedIndex == 3)
            {
                currentStudents = currentStudents.OrderBy(r => r.STUDENT_BIRTHDAY).ToList();
            }
            if (SortComboBox.SelectedIndex == 4)
            {
                currentStudents = currentStudents.OrderByDescending(r => r.STUDENT_BIRTHDAY).ToList();
            }


            if (FilterComboBox.SelectedIndex == 1)
            {
                currentStudents = currentStudents.Where(p => p.STUDENT_GENDER == "м").ToList();
            }
            if (FilterComboBox.SelectedIndex == 2)
            {
                currentStudents = currentStudents.Where(p => p.STUDENT_GENDER == "ж").ToList();
            }
            if (FilterComboBox.SelectedIndex == 3)
                currentStudents = currentStudents.Where(s => s.STATEMENT_SESSION.Any() && s.STATEMENT_SESSION.Average(m => m.STATEMENT_SESSION_MARK) > 3).ToList();
            if (FilterComboBox.SelectedIndex == 4)
                currentStudents = currentStudents.Where(s =>
                    s.STATEMENT_SESSION.Any() && s.STATEMENT_SESSION.Average(m => m.STATEMENT_SESSION_MARK) > 4).ToList();
            if (FilterComboBox.SelectedIndex == 5)
                currentStudents = currentStudents.Where(s => s.STATEMENT_SESSION.Any() && s.STATEMENT_SESSION.Average(m => m.STATEMENT_SESSION_MARK) == 5).ToList();

            currentStudents = currentStudents.Where(p => p.FIO_STUDENT.ToLower().Contains(SearchTB.Text.ToLower())).ToList();

            StudentsListView.ItemsSource = currentStudents;

            TBCount.Text = currentStudents.Count.ToString();
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage((sender as Button).DataContext as STUDENTS));
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage(null));
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                AbdeevMarksStudentsEntities.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                StudentsListView.ItemsSource = AbdeevMarksStudentsEntities.GetContext().STUDENTS.ToList();
                UpdateStudents();
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var currentStudents = (sender as Button).DataContext as STUDENTS;

            var currentStudentSession = AbdeevMarksStudentsEntities.GetContext().STATEMENT_SESSION.ToList().Where(p => p.STUDENT_ID == currentStudents.STUDENT_ID).ToList();
            var currentStudentMarks = AbdeevMarksStudentsEntities.GetContext().STATEMENT_MARKS.ToList().Where(p => p.STUDENT_ID == currentStudents.STUDENT_ID).ToList();

            if (currentStudentSession.Count != 0 || currentStudentMarks.Count != 0)
                MessageBox.Show("Невозможно выполнить удаление студента, так как существуют записи в ведомостях оценок");
            else
            {
                if (MessageBox.Show("Вы точно хотите выполнить удаление?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        AbdeevMarksStudentsEntities.GetContext().STUDENTS.Remove(currentStudents);
                        AbdeevMarksStudentsEntities.GetContext().SaveChanges();
                        StudentsListView.ItemsSource = AbdeevMarksStudentsEntities.GetContext().STUDENTS.ToList();
                        UpdateStudents();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }

            }
        }
    }
}
