using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using People.Model;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace People.Desktop.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly HttpClient _httpClient;
    
    [Reactive] public Guid? Id { get; set; }
    [Reactive] public string? LastName { get; set; }
    [Reactive] public string? FirstName { get; set; }
    
    public ObservableCollection<Person> People { get; set; } = [];
    [Reactive] public Person? SelectedPerson { get; set; }

    public ReactiveCommand<Unit, Unit> LoadPeopleCommand { get; }

    public MainWindowViewModel()
    {
        _httpClient = App.HttpClient;

        this.WhenAnyValue(vm => vm.SelectedPerson)
            .Subscribe(_ =>
            {
                Id = SelectedPerson?.Id;
                LastName = SelectedPerson?.LastName;
                FirstName = SelectedPerson?.FirstName;
            });

        LoadPeopleCommand = ReactiveCommand.CreateFromTask(LoadPeopleAsync);
    }

    private async Task LoadPeopleAsync(CancellationToken cancellationToken = default)
    {
        var url = new Uri("http://localhost:5064/people");
        var people = await _httpClient.GetFromJsonAsync<IEnumerable<Person>>(url, cancellationToken);
        
        People.Clear();
        foreach (var person in people)
        {
            People.Add(person);
        }
    }
}