using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PeopleDatabase1
{
    public partial class MainWindow : Window
    {
        private PeopleDbContext db;
        private ObservableCollection<Person> people;
        private bool sortByAge = false;

        public MainWindow()
        {
            InitializeComponent();

            db = new PeopleDbContext();

            db.Database.EnsureCreated();

            db.People.Load();

            people = db.People.Local.ToObservableCollection();

            if (people.Count == 0)
            {
                var p1 = new Person
                {
                    FirstName = "John",
                    LastName = "Smith",
                    Age = 20
                };

                var p2 = new Person
                {
                    FirstName = "Mary",
                    LastName = "Jones",
                    Age = 22
                };

                db.People.Add(p1);
                db.People.Add(p2);

                db.SaveChanges();

                db.People.Load();

                people = db.People.Local.ToObservableCollection();
            }

            RefreshGrid();
        }

        private void RefreshGrid()
        {
            var query = db.People.AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                string search = SearchBox.Text;

                query = query.Where(p =>
                    p.FirstName.Contains(search) ||
                    p.LastName.Contains(search));
            }

            if (sortByAge)
            {
                PeopleGrid.ItemsSource =
                    query.OrderBy(p => p.Age).ToList();
            }
            else
            {
                PeopleGrid.ItemsSource =
                    query.OrderBy(p => p.LastName).ToList();
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(AgeBox.Text, out int age))
            {
                Person p = new Person
                {
                    FirstName = FirstNameBox.Text,
                    LastName = LastNameBox.Text,
                    Age = age
                };

                db.People.Add(p);
                db.SaveChanges();

                RefreshGrid();

                FirstNameBox.Clear();
                LastNameBox.Clear();
                AgeBox.Clear();
            }
            else
            {
                MessageBox.Show("Please enter a valid age.");
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (PeopleGrid.SelectedItem is Person p)
            {
                db.People.Remove(p);
                db.SaveChanges();

                RefreshGrid();
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshGrid();
        }

        private void SortAge_Click(object sender, RoutedEventArgs e)
        {
            sortByAge = true;
            RefreshGrid();
        }

        private void SortName_Click(object sender, RoutedEventArgs e)
        {
            sortByAge = false;
            RefreshGrid();
        }
    }
}