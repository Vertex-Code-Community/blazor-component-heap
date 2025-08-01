Blazor.registerCustomEventType('extmousedown', {
    browserEventName: 'mousedown',
    createEventArgs: event => {

        const x = event.clientX - event.target.offsetLeft;
        const y = event.clientY - event.target.offsetTop;
        const pathCoordinates = getPathCoordinates(event);

        return {
            x: x,
            y: y,
            deltaX: event.deltaX,
            deltaY: event.deltaY,
            pageX: event.pageX,
            pageY: event.pageY,
            clientWidth: event.target.clientWidth,
            clientHeight: event.target.clientHeight,
            pathCoordinates: pathCoordinates
        };
    }
});

Blazor.registerCustomEventType('exttouchstart', {
    browserEventName: 'touchstart',
    createEventArgs: event => {

        const touches = Object.entries(event.touches).map((value, key) => {
            const touch = value[1];

            return {
                x: touch.clientX - event.target.offsetLeft,
                y: touch.clientY - event.target.offsetTop,
                clientWidth: event.target.clientWidth,
                clientHeight: event.target.clientHeight,
                clientX: touch.clientX,
                clientY: touch.clientY,
                pageX: touch.pageX,
                pageY: touch.pageY
            }
        });

        let element = document.elementFromPoint(touches[0].pageX, touches[0].pageY);
        const path = [];

        while (element && element !== document.body) {
            path.push(element);
            element = element.parentElement;
        }

        const pathCoordinates = getPathCoordinates({
            pageX: touches[0].pageX,
            pageY: touches[0].pageY,
            path: path
        });

        return {
            touches: touches,
            pathCoordinates: pathCoordinates
        };
    }
});