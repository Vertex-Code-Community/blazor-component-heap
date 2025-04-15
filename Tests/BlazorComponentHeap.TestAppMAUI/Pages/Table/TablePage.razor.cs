using BlazorComponentHeap.Shared.Enums.Table;
using BlazorComponentHeap.Shared.Models.Table;
using BlazorComponentHeap.TestAppMAUI.TestModels;

namespace BlazorComponentHeap.TestAppMAUI.Pages.Table;

public partial class TablePage
{
    private List<ColumnInfo> _columns = new();
    private List<UserModel> _users = new();
    private List<string> _selectData = new();

    private int _currentPage = 1;

    protected override void OnInitialized()
    {
        _columns.AddRange(
            new List<ColumnInfo>
            {
            new ColumnInfo
            {
                ColumnName = "First Name",
                PropertyName = "FirstName",
                Width = 25,
                IsPx = false,
                FilterType = ColumnFilterType.MultiSelect,
                MinWidth = 200,
                IsSorted = true
            },
            new ColumnInfo
            {
                ColumnName = "Last Name",
                PropertyName = "LastName",
                Width = 25,
                IsPx = false,
                FilterType = ColumnFilterType.NumberSearch,
                MinWidth = 200,
                IsSorted = true
            },
            new ColumnInfo
            {
                ColumnName = "Email",
                PropertyName = "Email",
                Width = 25,
                IsPx = false,
                FilterType = ColumnFilterType.Select,
                MinWidth = 200,
                IsSorted = true
            },
            new ColumnInfo
            {
                ColumnName = "BirthDay",
                PropertyName = "BirthDay",
                Width = 25,
                IsPx = false,
                FilterType = ColumnFilterType.Date,
                MinWidth = 200,
                IsSorted = true
            },
            new ColumnInfo
            {
                ColumnName = "Period",
                PropertyName = "StartDate",
                Width = 25,
                IsPx = false,
                FilterType = ColumnFilterType.DateRange,
                MinWidth = 200,
                IsSorted = true
            },
            new ColumnInfo
            {
                ColumnName = "Actions",
                PropertyName = "Actions",
                Width = 25,
                IsPx = false,
                FilterType = ColumnFilterType.TextSearch,
                MinWidth = 200,
                IsSorted = true,
                Buttons = new List<ButtonConfig>
                {
                    new ButtonConfig
                    {
                        Name = "Update",
                        ImgUrl = ""
                    },
                    new ButtonConfig
                    {
                        Name = "Delete",
                        ImgUrl = ""
                    }
                }
            },
            }
       );

        for (int i = 0; i < 100; i++)
        {
            _users.Add(new UserModel
            {
                FirstName = $"FirstName {i}",
                LastName = $"LastName {i}",
                Email = $"email{i}@gmail.com",
                PhoneNumber = "111111111"
            });
        }

        _selectData.AddRange(
            new List<string> { "test1", "test2", "test3" }
        );
    }

    private void FilterData(TableFilterParameters value)
    {
        Console.WriteLine(value.PropertyName);

        value.Filters.ForEach(item =>
        {
            Console.WriteLine(item);
        });

    }

    private void SortedData(TableSortParameters data)
    {
        Console.WriteLine(data.OrderByAsc);
        Console.WriteLine(data.PropertyName);
    }

    private void OnPagination(int numberPage)
    {
        Console.WriteLine(numberPage);
    }

    private void OnScrollPagination()
    {
        Console.WriteLine("bottom");
    }
}
