// window.addEventListener("resize", () => {
//     DotNet.invokeMethodAsync("BlazorComponentHeap.Core", 'OnBrowserResizeAsync').then(data => data);
// });
//
// window.addEventListener('scroll',function(event) {
//     const pathCoordinates = getPathCoordinates(event);
//    
//     DotNet.invokeMethodAsync("BlazorComponentHeap.Core", 'OnBrowserGlobalScrollAsync', {
//         pathCoordinates: pathCoordinates
//     });
// }, true);