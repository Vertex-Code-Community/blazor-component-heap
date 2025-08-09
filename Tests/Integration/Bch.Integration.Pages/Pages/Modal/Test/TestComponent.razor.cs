using Microsoft.AspNetCore.Components;

namespace Bch.Integration.Pages.Pages.Modal.Test;

public partial class TestComponent
{
    [CascadingParameter(Name = "TesModalParameter")] public required string TestText { get; set; }
}