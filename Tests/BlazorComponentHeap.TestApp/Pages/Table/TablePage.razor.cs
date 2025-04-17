using BlazorComponentHeap.Components.Table;
using BlazorComponentHeap.Core.Models.Table;
using BlazorComponentHeap.TestApp.Routing.Services;
using BlazorComponentHeap.TestApp.TestModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorComponentHeap.TestApp.Pages.Table;

public partial class TablePage
{
    // [Inject] private IJSRuntime JsRuntime { get; set; } = null!;
    // [Inject] private FlexibleNavigationManager NavigationManager { get; set; } = null!;
    
    private readonly List<UserModel> _users = new();
    private readonly List<UserModel> _filteredUsers = new();
    private readonly List<string> _countries = new List<string> { "UA", "PL", "DE" };
    private readonly List<string> _sex = new List<string> { "Male", "Female" };
    private readonly List<uint> _sizes = new() { 5, 10, 20, 50, 100 };
    
    private int _currentPage = 1;

    private BCHTable<UserModel>? _tableRef;

    protected override void OnInitialized()
    {
        for (var i = 0; i < 100; i++)
        {
            _users.Add(new UserModel
            {
                Name = $"Name {i}",
                Age = i,
                Country = (i % 3) switch { 0 => "UA", 1 => "PL", _ => "DE" },
                Sex = i % 2 == 0 ? "Male" : "Female",
                Birthday = new DateTime(1997, i % 10 + 1, i % 27 + 1),
                ReportDate = new DateTime(2023, 09, i % 2 == 0 ? 1 : 10)
            });
        }
        
        _filteredUsers.AddRange(_users);
    }

    private void OnFilterData(TableFilterParameters p)
    {
        _filteredUsers.Clear();
        
        switch (p.PropertyName)
        {
            case "Name":
            {
                var filter = p.Filters.First();
                _filteredUsers.AddRange(_users.Where(x => x.Name.StartsWith(filter)));
                break;
            }

            case "Age":
            {
                var filter = p.Filters.First();
                _filteredUsers.AddRange(_users.Where(x => x.Age.ToString().StartsWith(filter)));
                break;
            }
            
            case "Sex":
            {
                var filter = p.Filters.First();
                _filteredUsers.AddRange(_users.Where(x => x.Sex == filter));
                break;
            }
            
            case "Country":
            {
                _filteredUsers.AddRange(
                    p.Filters.Count == 0 ? _users : _users.Where(x => p.Filters.Contains(x.Country)));

                break;
            }
            
            case "Birthday":
            {
                _filteredUsers.AddRange(_users.Where(x => x.Birthday == p.Date));
                break;
            }
            
            case "Report":
            {
                _filteredUsers.AddRange(_users
                    .Where(x => p.DateRange.Start <= x.ReportDate && p.DateRange.End >= x.ReportDate));
                
                break;
            }
        }
        
        _tableRef?.ReloadTable();
    }

    private void OnSortData(TableSortParameters p)
    {
        _filteredUsers.Clear();
        
        switch (p.PropertyName)
        {
            case "Name":
            {
                _filteredUsers.AddRange(
                    p.OrderByAsc ? _users.OrderBy(x => x.Name) : _users.OrderByDescending(x => x.Name));
                break;
            }

            case "Age":
            {
                _filteredUsers.AddRange(
                    p.OrderByAsc ? _users.OrderBy(x => x.Age) : _users.OrderByDescending(x => x.Age));
                break;
            }
            
            case "Sex":
            {
                _filteredUsers.AddRange(
                    p.OrderByAsc ? _users.OrderBy(x => x.Sex) : _users.OrderByDescending(x => x.Sex));
                break;
            }
            
            case "Country":
            {
                _filteredUsers.AddRange(
                    p.OrderByAsc ? _users.OrderBy(x => x.Country) : _users.OrderByDescending(x => x.Country));

                break;
            }
        }
        
        _tableRef?.ReloadTable();
    }

    // private void OnPagination(int numberPage)
    // {
    //     Console.WriteLine(numberPage);
    // }
    //
    // private void OnScrollPagination()
    // {
    //     Console.WriteLine("bottom");
    // }

    private void OnReloadData()
    {
        // for (var i = 100; i < 200; i++)
        // {
        //     _users.Add(new UserModel
        //     {
        //         Name = $"Name {i}",
        //         Age = i,
        //         Country = i % 2 == 0 ? "UA" : "PL",
        //         Birthday = new DateTime(1997, (i + 1) % 12, (i + 1) % 27),
        //         ReportStartDate = new DateTime(2023, 09, i % 2 == 0 ? 1 : 10),
        //         ReportEndDate = new DateTime(2023, 09, i % 2 == 0 ? 7 : 20)
        //     });
        // }
        
        _tableRef?.ReloadTable();
    }
    
    // private async Task NavigateToAsync(string url)
    // {
    //     await JsRuntime.InvokeVoidAsync("navigateToWithoutSaving", url);
    // }
}
