Blazor.registerCustomEventType('bchpanzoomscroll', {
    browserEventName: 'wheel',
    createEventArgs: event => {
        // Prevent outer scroll by default
        event.preventDefault();
        event.stopPropagation();
        
        return {
            x: event.offsetX ?? 0,
            y: event.offsetY ?? 0,
            deltaX: event.deltaX ?? 0,
            deltaY: event.deltaY ?? 0,
        };
    },
    // ensure Blazor sets passive: false
    preventDefault: true,
    stopPropagation: true
});

Blazor.registerCustomEventType('bchpanzoomtouchstart', {
    browserEventName: 'touchstart',
    createEventArgs: event => {
        event.preventDefault();
        event.stopPropagation();
        const touches = Array.from(event.touches).map(t => ({
            x: t.pageX - event.target.getBoundingClientRect().left,
            y: t.pageY - event.target.getBoundingClientRect().top,
            screenX: t.screenX,
            screenY: t.screenY,
            clientWidth: event.target.clientWidth,
            clientHeight: event.target.clientHeight,
            clientX: t.clientX,
            clientY: t.clientY,
            pageX: t.pageX,
            pageY: t.pageY,
            pathCoordinates: []
        }));
        return { touches };
    },
    preventDefault: true,
    stopPropagation: true
});

Blazor.registerCustomEventType('bchpanzoomtouchmove', {
    browserEventName: 'touchmove',
    createEventArgs: event => {
        event.preventDefault();
        event.stopPropagation();
        const touches = Array.from(event.touches).map(t => ({
            x: t.pageX - event.target.getBoundingClientRect().left,
            y: t.pageY - event.target.getBoundingClientRect().top,
            screenX: t.screenX,
            screenY: t.screenY,
            clientWidth: event.target.clientWidth,
            clientHeight: event.target.clientHeight,
            clientX: t.clientX,
            clientY: t.clientY,
            pageX: t.pageX,
            pageY: t.pageY,
            pathCoordinates: []
        }));
        return { touches };
    },
    preventDefault: true,
    stopPropagation: true
});

Blazor.registerCustomEventType('bchpanzoomtouchend', {
    browserEventName: 'touchend',
    createEventArgs: event => {
        const touches = Array.from(event.changedTouches).map(t => ({
            x: t.pageX - event.target.getBoundingClientRect().left,
            y: t.pageY - event.target.getBoundingClientRect().top,
            screenX: t.screenX,
            screenY: t.screenY,
            clientWidth: event.target.clientWidth,
            clientHeight: event.target.clientHeight,
            clientX: t.clientX,
            clientY: t.clientY,
            pageX: t.pageX,
            pageY: t.pageY,
            pathCoordinates: []
        }));
        return { touches };
    },
    preventDefault: true,
    stopPropagation: true
});

