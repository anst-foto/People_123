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
    private readonly string _baseUrl;

    [Reactive] public Guid? Id { get; set; }
    [Reactive] public string? LastName { get; set; }
    [Reactive] public string? FirstName { get; set; }

    public ObservableCollection<Person> People { get; set; } = [];
    [Reactive] public Person? SelectedPerson { get; set; }

    public ReactiveCommand<Unit, Unit> LoadPeopleCommand { get; }
    public ReactiveCommand<Unit, Unit> SavePersonCommand { get; }
    public ReactiveCommand<Unit, Unit> DeletePersonCommand { get; }
    public ReactiveCommand<Unit, Unit> ClearPersonCommand { get; }

    public MainWindowViewModel()
    {
        _httpClient = App.HttpClient;
        _baseUrl = "http://localhost:5064/people";

        this.WhenAnyValue(vm => vm.SelectedPerson)
            .Subscribe(_ =>
            {
                Id = SelectedPerson?.Id;
                LastName = SelectedPerson?.LastName;
                FirstName = SelectedPerson?.FirstName;
            });

        LoadPeopleCommand = ReactiveCommand.CreateFromTask(LoadPeopleAsync);
        SavePersonCommand = ReactiveCommand.CreateFromTask(SavePersonAsync);
        DeletePersonCommand = ReactiveCommand.CreateFromTask(DeletePersonAsync);
        ClearPersonCommand = ReactiveCommand.Create(ClearPerson);
    }

    private void ClearPerson()
    {
        SelectedPerson = null;
        Id = null;
        LastName = null;
        FirstName = null;
    }

    private async Task DeletePersonAsync(CancellationToken cancellationToken = default)
    {
        var url = new Uri($"{_baseUrl}/{Id}");
        await _httpClient.DeleteAsync(url, cancellationToken);
        
        ClearPerson();
        await LoadPeopleAsync(cancellationToken);
    }

    private async Task SavePersonAsync(CancellationToken cancellationToken = default)
    {
        if (Id.HasValue)
        {
            var url = new Uri($"{_baseUrl}/{Id}");
            var person = new Person()
            {
                Id = Id.Value,
                LastName = LastName,
                FirstName = FirstName
            };
            await _httpClient.PutAsJsonAsync(url, person, cancellationToken);
        }
        else
        {
            var url = new Uri($"{_baseUrl}");
            var person = new Person()
            {
                Id = Guid.NewGuid(),
                LastName = LastName,
                FirstName = FirstName
            };
            await _httpClient.PostAsJsonAsync(url, person, cancellationToken);
        }
        
        ClearPerson();
        await LoadPeopleAsync(cancellationToken);
    }

    private async Task LoadPeopleAsync(CancellationToken cancellationToken = default)
    {
        var url = new Uri(_baseUrl);
        var people = await _httpClient.GetFromJsonAsync<IEnumerable<Person>>(url, cancellationToken);

        People.Clear();
        foreach (var person in people)
        {
            People.Add(person);
        }
    }
}