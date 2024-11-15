using MauiApp1.Model;

namespace MauiApp1
{
    public partial class PersonPage : ContentPage
    {
        private Person _person;

        // Nullable olarak tanýmlandý
        public Action<Person>? OnPersonAdd { get; set; }
        public Action<string, Person>? OnPersonEdit { get; set; }
        public Action<Person>? OnImageAdd { get; set; }

        public PersonPage(Person person)
        {
            InitializeComponent();
            _person = person;
            BindingContext = _person;
        }

        private async void SelectImage_Clicked(object sender, EventArgs e)
        {
            if (OnImageAdd != null)
                OnImageAdd(_person);
        }

        private async void Save_Clicked(object sender, EventArgs e)
        {
            if (OnPersonAdd != null)
                OnPersonAdd(_person);
            else if (OnPersonEdit != null)
                OnPersonEdit(_person.Id, _person);

            await Navigation.PopModalAsync();
        }

        private async void Cancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}