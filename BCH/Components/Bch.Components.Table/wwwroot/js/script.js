Blazor.registerCustomEventType('bchtablescroll', {
    browserEventName: 'scroll',
    createEventArgs: event => {

        return {
            clientHeight: event.target.clientHeight,
            scrollHeight: event.target.scrollHeight,
            scrollTop: event.target.scrollTop,
            clientWidth: event.target.clientWidth
        };
    }
});