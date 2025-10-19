using Microsoft.Win32;
using System;
using System.Collections.Generic;
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

namespace AbdeevMarksStudents
{
    /// <summary>
    /// Логика взаимодействия для AddEditPage.xaml
    /// </summary>
    public partial class AddEditPage : Page
    {
        private STUDENTS _currentStudent = new STUDENTS();
        public AddEditPage(STUDENTS selectedStudent)
        {
            InitializeComponent();
            if (selectedStudent != null)
            {
                _currentStudent = selectedStudent;
            }

            DataContext = _currentStudent;
            var currentGroups = AbdeevMarksStudentsEntities.GetContext().GROUP.ToList();
            GroupsComboBox.ItemsSource = currentGroups;


        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (GroupsComboBox.SelectedIndex == -1) errors.AppendLine("Выберите группу!");

            if (string.IsNullOrWhiteSpace(_currentStudent.STUDENT_SURNAME))
            {
                errors.AppendLine("Введите фамилию!");
            }
            else if (_currentStudent.STUDENT_SURNAME.Any(char.IsDigit))
                errors.AppendLine("Фамилия не должна содержать цифр!");
            if (string.IsNullOrWhiteSpace(_currentStudent.STUDENT_NAME))
            {
                errors.AppendLine("Введите имя!");
            }
            else if (_currentStudent.STUDENT_NAME.Any(char.IsDigit))
                errors.AppendLine("Имя не должно содержать цифр!");
            if (!string.IsNullOrWhiteSpace(_currentStudent.STUDENT_PATRONYMIC) && _currentStudent.STUDENT_PATRONYMIC.Any(char.IsDigit))
            {
                errors.AppendLine("Отчество не должно содержать цифр!");
            }
            if (string.IsNullOrWhiteSpace(_currentStudent.STUDENT_ADDRESS))
            {
                errors.AppendLine("Введите адрес!");
            }
            if (string.IsNullOrWhiteSpace(_currentStudent.STUDENT_GENDER))
            {
                errors.AppendLine("Введите пол!");

            }
            else if (_currentStudent.STUDENT_GENDER != "м" && _currentStudent.STUDENT_GENDER != "ж")
                errors.AppendLine("Введите пол верно! Допустимые значения: 'м' или 'ж'.");

            if (_currentStudent.STUDENT_BIRTHDAY == null)
            {
                errors.AppendLine("Укажите дату рождения!");
            }
            else
            {
                if (_currentStudent.STUDENT_BIRTHDAY > DateTime.Now) errors.AppendLine("Дата рождения не может быть в будущем!");
                else
                {
                    int age = DateTime.Now.Year - _currentStudent.STUDENT_BIRTHDAY.Year;
                    if (_currentStudent.STUDENT_BIRTHDAY.Date > DateTime.Now.AddYears(-age)) age--; // корректировка, если день рождения ещё не наступил

                    if (age < 10)
                    {
                        errors.AppendLine("Возраст студента не может быть меньше 10 лет!");
                    }
                    else if (age > 100)
                    {
                        errors.AppendLine("Возраст студента не может быть больше 100 лет!");
                    }
                }
            }
            if (string.IsNullOrWhiteSpace(_currentStudent.STUDENT_PHONE))
            {
                errors.AppendLine("Введите номер телефона!");
            }
            else
            {
                string ph = _currentStudent.STUDENT_PHONE.Replace(" ", "");
                if (ph.Length != 11 || !ph.All(char.IsDigit) || !ph.StartsWith("79"))
                {
                    errors.AppendLine("Введите номер в формате 79XXXXXXXXX");
                }
            }


            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            _currentStudent.GROUP_ID = GroupsComboBox.SelectedIndex + 1;

            if (_currentStudent.STUDENT_ID == 0)
                AbdeevMarksStudentsEntities.GetContext().STUDENTS.Add(_currentStudent);

            try
            {
                AbdeevMarksStudentsEntities.GetContext().SaveChanges();
                MessageBox.Show("Информация сохранена!");
                Manager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ChangePictureBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            myOpenFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg;*.bmp)|*.png;*.jpeg;*.jpg;*.bmp|All files (*.*)|*.*";

            if (myOpenFileDialog.ShowDialog() == true)
            {
                string originalPath = myOpenFileDialog.FileName;
                string fileName = System.IO.Path.GetFileName(originalPath); // Только имя файла с расширением
                string destinationFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "res");

                // Убедиться, что папка существует
                if (!Directory.Exists(destinationFolder))
                {
                    Directory.CreateDirectory(destinationFolder);
                }

                string destinationPath = System.IO.Path.Combine(destinationFolder, fileName);

                // Копировать файл (с перезаписью, если уже существует)
                File.Copy(originalPath, destinationPath, overwrite: true);

                // Сохранить только имя файла в модели
                _currentStudent.STUDENT_PHOTO = fileName;

                // Обновить изображение в UI из локальной папки
                LogoImage.Source = new BitmapImage(new Uri(destinationPath));
            }
        }
    }
}
