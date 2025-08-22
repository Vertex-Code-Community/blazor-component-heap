Blazor.registerCustomEventType('imageloaded', {
    browserEventName: 'load',
    createEventArgs: event => {
        console.log('load event');
        const imageWidth = event.target.naturalWidth;
        const imageHeight = event.target.naturalHeight;

        const rect = event.target.getBoundingClientRect();

        return {
            imageWidth: imageWidth,
            imageHeight: imageHeight,
            width: rect.width,
            height: rect.height,
        };
    }
});