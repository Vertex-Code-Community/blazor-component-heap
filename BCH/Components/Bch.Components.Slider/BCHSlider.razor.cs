using System.Globalization;
using Microsoft.AspNetCore.Components;

namespace Bch.Components.Slider;

public partial class BchSlider<TItem> : ComponentBase where TItem : class
{
    [Parameter] public List<TItem> Items { get; set; } = new();
    [Parameter] public bool ButtonsOnTopOfContent { get; set; } = false;
    [Parameter] public required RenderFragment<TItem> ItemTemplate { get; set; }
    [Parameter] public int RenderItemCount { get; set; } = 1;
    [Parameter] public bool CircularScroll { get; set; } = false;
    [Parameter] public bool ButtonsAboveContent { get; set; } = false;
    [Parameter] public bool ShowCircleButtons { get; set; } = false;
    [Parameter] public bool CircleButtonsAboveContent { get; set; } = false;

    private int _scrollIndex = 0;
    private NumberFormatInfo _numberFormatWithDot = new () { NumberDecimalSeparator = "." };

    protected override void OnInitialized()
    {
        RenderItemCount = Math.Clamp(RenderItemCount, 1, Items.Count);
    }

    protected override void OnParametersSet()
    {
        var prev = RenderItemCount;
        RenderItemCount = Math.Clamp(RenderItemCount, 1, Items.Count);

        if (prev != RenderItemCount) StateHasChanged();
    }

    private void OnNextClicked(int k)
    {
        _scrollIndex += k;

        if (!CircularScroll)
        {
            _scrollIndex = Math.Clamp(_scrollIndex, -Items.Count + RenderItemCount, 0);
        }

        StateHasChanged();
    }

    private void OnCircleClicked(int i)
    {
        var index = (-_scrollIndex) % Items.Count;
        index = index < 0 ? (Items.Count + index) : index;
        _scrollIndex += (index - i);

        StateHasChanged();
    }

    private int CirclesCount => Items.Count - (!CircularScroll ? RenderItemCount - 1 : 0);
}
