using Bch.Components.Table;
using Bch.Components.Table.Models;
using Bch.Integration.Pages.Models;

namespace Bch.Integration.Pages.Pages.Table;

public partial class TablePage
{
    // [Inject] private IJSRuntime JsRuntime { get; set; } = null!;
    // [Inject] private FlexibleNavigationManager NavigationManager { get; set; } = null!;
    
    private readonly List<UserModel> _users = new();
    private readonly List<UserModel> _filteredUsers1 = new();
    private readonly List<UserModel> _filteredUsers2 = new();
    private readonly List<UserModel> _filteredUsers3 = new();
    private readonly List<string> _countries = new List<string> { "UA", "PL", "DE" };
    private readonly List<string> _sex = new List<string> { "Male", "Female" };
    private readonly List<uint> _sizes = new() { 5, 10, 20, 50, 100 };
    
    private int _currentPage = 1;
    private int _currentPage2 = 1;
    private int _currentPage3 = 1;

    private BchTable<UserModel>? _tableRef1;
    private BchTable<UserModel>? _tableRef2;
    private BchTable<UserModel>? _tableRef3;

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
        
        _filteredUsers1.AddRange(_users);
        _filteredUsers2.AddRange(_users);
        _filteredUsers3.AddRange(_users);
    }

    private static void ApplyFilter(List<UserModel> source, List<UserModel> destination, TableFilterParameters p)
    {
        destination.Clear();
        
        switch (p.PropertyName)
        {
            case "Name":
            {
                var filter = p.Filters.First();
                destination.AddRange(source.Where(x => x.Name.StartsWith(filter)));
                break;
            }

            case "Age":
            {
                var filter = p.Filters.First();
                destination.AddRange(source.Where(x => x.Age.ToString().StartsWith(filter)));
                break;
            }
            
            case "Sex":
            {
                var filter = p.Filters.First();
                destination.AddRange(source.Where(x => x.Sex == filter));
                break;
            }
            
            case "Country":
            {
                destination.AddRange(
                    p.Filters.Count == 0 ? source : source.Where(x => p.Filters.Contains(x.Country)));

                break;
            }
            
            case "Birthday":
            {
                destination.AddRange(source.Where(x => x.Birthday == p.Date));
                break;
            }
            
            case "Report":
            {
                destination.AddRange(source
                    .Where(x => p.DateRange.Start <= x.ReportDate && p.DateRange.End >= x.ReportDate));
                
                break;
            }
        }
    }

    private static void ApplySort(List<UserModel> source, List<UserModel> destination, TableSortParameters p)
    {
        destination.Clear();
        
        switch (p.PropertyName)
        {
            case "Name":
            {
                destination.AddRange(
                    p.OrderByAsc ? source.OrderBy(x => x.Name) : source.OrderByDescending(x => x.Name));
                break;
            }

            case "Age":
            {
                destination.AddRange(
                    p.OrderByAsc ? source.OrderBy(x => x.Age) : source.OrderByDescending(x => x.Age));
                break;
            }
            
            case "Sex":
            {
                destination.AddRange(
                    p.OrderByAsc ? source.OrderBy(x => x.Sex) : source.OrderByDescending(x => x.Sex));
                break;
            }
            
            case "Country":
            {
                destination.AddRange(
                    p.OrderByAsc ? source.OrderBy(x => x.Country) : source.OrderByDescending(x => x.Country));

                break;
            }
        }
    }

    private void OnFilterData1(TableFilterParameters p)
    {
        ApplyFilter(_users, _filteredUsers1, p);
        _tableRef1?.ReloadTable();
    }

    private void OnFilterData2(TableFilterParameters p)
    {
        ApplyFilter(_users, _filteredUsers2, p);
        _tableRef2?.ReloadTable();
    }

    private void OnFilterData3(TableFilterParameters p)
    {
        ApplyFilter(_users, _filteredUsers3, p);
        _tableRef3?.ReloadTable();
    }

    private void OnSortData1(TableSortParameters p)
    {
        ApplySort(_users, _filteredUsers1, p);
        _tableRef1?.ReloadTable();
    }

    private void OnSortData2(TableSortParameters p)
    {
        ApplySort(_users, _filteredUsers2, p);
        _tableRef2?.ReloadTable();
    }

    private void OnSortData3(TableSortParameters p)
    {
        ApplySort(_users, _filteredUsers3, p);
        _tableRef3?.ReloadTable();
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
        
        _tableRef1?.ReloadTable();
        _tableRef2?.ReloadTable();
        _tableRef3?.ReloadTable();
    }
    
    // private async Task NavigateToAsync(string url)
    // {
    //     await JsRuntime.InvokeVoidAsync("navigateToWithoutSaving", url);
    // }
}
