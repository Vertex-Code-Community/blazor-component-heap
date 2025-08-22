
if (!Event.prototype.hasOwnProperty('path')) {
    Object.defineProperty(Event.prototype, 'path', {
        get() { return this.composedPath(); }
    });
}

// for Safari and Firefox
if (!("path" in MouseEvent.prototype)) {
    Object.defineProperty(MouseEvent.prototype, "path", {
        get: function () {

            const path = [];
            let currentElem = this.target;
            
            while (currentElem) {
                path.push(currentElem);
                currentElem = currentElem.parentElement;
            }
            if (path.indexOf(window) === -1 && path.indexOf(document) === -1)
                path.push(document);
            if (path.indexOf(window) === -1)
                path.push(window);
            return path;
        }
    });
}

function getPathCoordinates(event) {
    const pageX = event.clientX | 0;
    const pageY = event.clientY | 0;
    
    return event.path.map(element => {
        if (element.getBoundingClientRect) {
            const viewportOffset = element.getBoundingClientRect();
            
            return {
                x: pageX - viewportOffset.left,
                y: pageY - viewportOffset.top,
                clientWidth: element.clientWidth,
                clientHeight: element.clientHeight,
                scrollTop: element.scrollTop,
                classList: element.classList.value,
                id: element.id
            };
        }
    }).filter(x => x);
}

function getPathCoordinatesByPos(x, y) {
    let element = document.elementFromPoint(x, y);
    const path = [];

    while (element && element !== document.body) {
        path.push(element);
        element = element.parentElement;
    }

    return getPathCoordinates({
        pageX: x,
        pageY: y,
        path: path
    });
}

const bchListeners = {};

function bchAddDocumentListener(key, eventName, dotnetReference, methodName,
                                subscribingContextRef,
                                preventDefault = false,
                                stopPropagation = false,
                                passive = true) {

    const callback = function (event) {
        
        if (preventDefault || event.target?.closest(`[${eventName}-prevent-default]`)) event.preventDefault();
        if (stopPropagation || event.target?.closest(`[${eventName}-stop-propagation]`)) event.stopPropagation();

        // const preventDefaultAttr = event.target && event.target.hasAttribute(`${eventName}-prevent-default`);
        // const stopPropagationAttr = event.target && event.target.hasAttribute(`${eventName}-stop-propagation`);
        //
        // if (preventDefault || preventDefaultAttr) event.preventDefault();
        // if (stopPropagation || stopPropagationAttr) event.stopPropagation();

        let response = {};

        switch (eventName) {
            case "touchstart":
            case "touchmove":
            {
                const touches = Object.entries(event.touches).map((value, key) => {
                    const touch = value[1];
                    const pathCoordinates = getPathCoordinatesByPos(touch.pageX, touch.pageY);

                    return {
                        clientX: touch.clientX,
                        clientY: touch.clientY,
                        pageX: touch.pageX,
                        pageY: touch.pageY,
                        pathCoordinates: pathCoordinates
                    }
                });

                response = {
                    touches: touches
                };
                break;
            }
            case "touchend":
            {
                const touches = Object.entries(event.changedTouches).map((value, key) => {
                    const touch = value[1];
                    const pathCoordinates = getPathCoordinatesByPos(touch.pageX, touch.pageY);

                    return {
                        clientX: touch.clientX,
                        clientY: touch.clientY,
                        pageX: touch.pageX,
                        pageY: touch.pageY,
                        pathCoordinates: pathCoordinates
                    }
                });

                response = {
                    touches: touches
                };
                break;
            }
            case "mousewheel":
            {
                const x = event.pageX - event.target.offsetLeft;
                const y = event.pageY - event.target.offsetTop;
                const pathCoordinates = getPathCoordinates(event);

                response = {
                    x: x,
                    y: y,
                    deltaX: event.deltaX,
                    deltaY: event.deltaY,
                    pathCoordinates: pathCoordinates
                };

                break;
            }
            case "scroll": {
                const pathCoordinates = getPathCoordinates(event);

                response = {
                    pathCoordinates: pathCoordinates
                };
                break;
            }
            default:
                const pathCoordinates = getPathCoordinates(event);

                response = {
                    offsetX: event.offsetX,
                    offsetY: event.offsetY,
                    pageX: event.pageX,
                    pageY: event.pageY,
                    screenX: event.screenX,
                    screenY: event.screenY,
                    clientX: event.clientX,
                    clientY: event.clientY,
                    pathCoordinates: pathCoordinates
                };
                break;
        }

        dotnetReference.invokeMethodAsync(methodName, response);

        if (eventName === 'mousewheel') return false;
    };
    const subscribingContext = subscribingContextRef === 1 ? document : window;
    
    bchListeners[key + eventName] = {
        callback: callback,
        subscribingContext: subscribingContext
    };

    subscribingContext.addEventListener(eventName, callback, { passive: passive });
}

function bchRemoveDocumentListener(key, eventName) {
    const subscriptionRecord = bchListeners[key + eventName];
    
    if (subscriptionRecord) {
        const subscribingContext = subscriptionRecord.subscribingContext;
        subscribingContext.removeEventListener(eventName, subscriptionRecord.callback);
        
        delete bchListeners[key + eventName];
    }
}