function bchGetBoundingClientRectById(id, param) {
    const element = document.getElementById(id);
    if (!element) return null;

    const rect = element.getBoundingClientRect();

    return {
        width: rect.width,
        height: rect.height,
        bottom: rect.bottom,
        left: rect.left,
        right: rect.right,
        top: rect.top,
        x: rect.x,
        y: rect.y,
        offsetTop: element.offsetTop,
        offsetLeft: element.offsetLeft,
        clientWidth: element.clientWidth,
        clientHeight: element.clientHeight,
        offsetWidth: element.offsetWidth,
        offsetHeight: element.offsetHeight
    };
}

function bchScrollElementTo(id, x, y, behavior) {
    const element = document.getElementById(id);

    if (!element) {
        return;
    }

    element.scrollTo({
        left: x,
        top: y,
        behavior: behavior // only 'auto' or 'smooth'
    });
}

function bchSetLeftTopToElement(id, x, y) {
    const element = document.getElementById(id);
    if (!element) return;

    element.style.left = x;
    element.style.top = y;
}

function bchFocusElement(elementId) {
    const element = document.getElementById(elementId);
    if (element === document.activeElement) return;
    element.focus();
}