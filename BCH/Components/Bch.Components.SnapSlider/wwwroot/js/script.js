
const bchSnapSliderMap = {};

function bchRegisterSnapFeedbackRef(dotnetRef, scrollerId, leftShift) {
    bchSnapSliderMap[scrollerId] = {
        index: 3,
        dotnetRef: dotnetRef,
        leftShift: leftShift,
        triggered: false
    }
}

function bchReleaseSnapFeedbackRef(scrollerId) {
    delete bchSnapSliderMap[scrollerId];
}

function bchOnSnapSliderScroll(event) {
    const scrollerId = event.target.id;
    if (!scrollerId) return;

    const state = bchSnapSliderMap[scrollerId];
    if (!state) return;

    const firstChildElement = event.target.firstElementChild;
    const childWidth = firstChildElement.clientWidth;

    const scrollLeft = event.target.scrollLeft - firstChildElement.offsetLeft + childWidth;
    const index = (scrollLeft / childWidth);

    let difference = index - state.index;

    const differenceAbs = Math.abs(difference);

    // console.log('bchOnSnapSliderScroll __', scrollLeft, childWidth);
    // console.log('bchOnSnapSliderScroll', index, state.index);

    if (differenceAbs >= 0.998) {
        if (state.triggered) return;

        state.triggered = true;
        const direction = Math.sign(difference);
        // console.log('trigger', direction)

        //state.index += direction;
        state.dotnetRef.invokeMethodAsync('OnNextCalledFromScrollListenerAsync', direction);
    } else {
        state.triggered = false;
    }
}

function bchSnapSliderScrollTo(scrollerId, scrollToRight) {
    const scroller = document.getElementById(scrollerId);
    if (!scroller) return;

    const secondElement = scroller.children[1];
    const fourthElement = scroller.children[3];
    if (!secondElement || !fourthElement) return;

    scroller.scrollTo({
        left: scrollToRight ? fourthElement.offsetLeft : secondElement.offsetLeft,
        top: 0,
        behavior: 'smooth'
    });
}