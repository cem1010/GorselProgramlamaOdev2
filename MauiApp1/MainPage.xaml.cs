using System.Collections.ObjectModel;
using System.Text.Json;
using MauiApp1.Model;

namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {
        private ObservableCollection<Person> _people = new();
        private ObservableCollection<Person> _allPeople = new();
        private readonly string _filePath;

        public MainPage()
        {
            InitializeComponent();
            _filePath = Path.Combine(FileSystem.Current.AppDataDirectory, "people.json");
            LoadPeople();
            personList.ItemsSource = _people;
        }

        private void LoadPeople() 
        {
            try
            {
                if (File.Exists(_filePath))
                {
                    var jsonData = File.ReadAllText(_filePath);
                    var loadedPeople = JsonSerializer.Deserialize<ObservableCollection<Person>>(jsonData);
                    if (loadedPeople != null)
                    {
                        _people = loadedPeople;
                        _allPeople = new ObservableCollection<Person>(_people);
                    }
                }
                else
                {
                    _people = new ObservableCollection<Person>
                    {
                        new Person
                        {
                            FirstName = "Ali",
                            LastName = "Demir",
                            PhoneNumber = "05555555555",
                            Email = "ali.demir@mail.com"
                        }
                    };
                    _allPeople = new ObservableCollection<Person>(_people);
                }
            }
            catch (Exception ex)
            {
                _people = new ObservableCollection<Person>();
                _allPeople = new ObservableCollection<Person>();
                DisplayAlert("Hata", "Veriler yüklenirken hata oluştu: " + ex.Message, "Tamam");
            }
        }
        //arama 
        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.NewTextValue))
            {
                _people = new ObservableCollection<Person>(_allPeople);
            }
            else
            {
                var searchText = e.NewTextValue.ToLower();
                var filteredList = _allPeople.Where(p =>
                    p.FirstName?.ToLower().Contains(searchText) == true ||
                    p.LastName?.ToLower().Contains(searchText) == true ||
                    p.PhoneNumber?.Contains(searchText) == true).ToList();

                _people = new ObservableCollection<Person>(filteredList);
            }

            if (personList != null)
            {
                personList.ItemsSource = _people;
            }
        }

        private async void Add_Click(object sender, EventArgs e)
        {
            var page = new PersonPage(new Person());
            page.OnPersonAdd = AddPerson;
            await Navigation.PushModalAsync(page);
        }
        //silme tuşu 
        private async void Delete_Click(object sender, EventArgs e)
        {
            if (sender is MenuItem menuItem && menuItem.CommandParameter is string id)
            {
                var result = await DisplayAlert("Silme Onayı",
                    "Kişi silinsin?",
                    "Evet", "Hayır");

                if (result)
                    DeletePerson(id);
            }
        }

        private async void Edit_Click(object sender, EventArgs e)
        {
            if (sender is MenuItem menuItem && menuItem.CommandParameter is string id)
            {
                var person = _people.FirstOrDefault(p => p.Id == id);
                if (person != null)
                {
                    var page = new PersonPage(person);
                    page.OnPersonEdit = EditPerson;
                    page.OnImageAdd = async (p) => await HandleImageUpload(p);
                    await Navigation.PushModalAsync(page);
                }
            }
        }
        //resim ekliyodu ama artık yok
        private async void AddImage_Click(object sender, EventArgs e)
        {
            if (sender is MenuItem menuItem && menuItem.CommandParameter is string id)
            {
                var person = _people.FirstOrDefault(p => p.Id == id);
                if (person != null)
                {
                    await HandleImageUpload(person);
                }
            }
        }

        private void AddPerson(Person person)
        {
            if (person != null)
            {
                _people.Add(person);
                _allPeople.Add(person);
            }
        }

        private void DeletePerson(string id)
        {
            var person = _people.FirstOrDefault(p => p.Id == id);
            if (person != null)
            {
                _people.Remove(person);
                _allPeople.Remove(person);
            }
        }

        private void EditPerson(string id, Person updatedPerson)
        {
            var index = _people.ToList().FindIndex(p => p.Id == id);
            if (index != -1 && updatedPerson != null)
            {
                _people[index] = updatedPerson;

                var allIndex = _allPeople.ToList().FindIndex(p => p.Id == id);
                if (allIndex != -1)
                {
                    _allPeople[allIndex] = updatedPerson;
                }
            }
        }

        private async Task HandleImageUpload(Person person)
        {
            try
            {
                var source = await DisplayActionSheet(
                    "Resim Kaynağı",
                    "İptal",
                    null,
                    "Galeri",
                    "Kamera");

                if (source == "Galeri")
                {
                    var result = await MediaPicker.Default.PickPhotoAsync();
                    if (result != null)
                    {
                        person.ProfileImage = result.FullPath;
                    }
                }
                else if (source == "Kamera" && MediaPicker.Default.IsCaptureSupported)
                {
                    var result = await MediaPicker.Default.CapturePhotoAsync();
                    if (result != null)
                    {
                        person.ProfileImage = result.FullPath;
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata",
                    "Resim ekelenemdi: " + ex.Message,
                    "Tamam");
            }
        }

        private void SavePeople(object sender, EventArgs e)
        {
            try
            {
                var jsonData = JsonSerializer.Serialize(_allPeople);
                File.WriteAllText(_filePath, jsonData);
                DisplayAlert("Başarılı", "Veriler kaydedildi!", "Tamam");
            }
            catch (Exception ex)
            {
                DisplayAlert("Hata", "Veriler kaydedilemedi " + ex.Message, "Tamam");
            }
        }
    }
}