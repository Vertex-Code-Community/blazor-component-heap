export function initColumnResizer(handleEl, rootEl, dotNetRef) {
    if (!handleEl || !rootEl) return;

    let startX = 0;
    let startWidth = 0;
    let thEl = rootEl.closest('th');

    if (!thEl) return;

    const tableEl = thEl.closest('table');
    if (!tableEl) return;

    const headerRow = thEl.parentElement;
    const colIndex = Array.prototype.indexOf.call(headerRow.children, thEl);

    let moved = false;
    const threshold = 3;
    const minW = 100;
    let headerWidths = [];

    const setCellWidth = (el, w) => {
        el.style.width = w + 'px';
        el.style.minWidth = w + 'px';
        el.style.maxWidth = w + 'px';
    };

    const snapshotHeaderWidths = () => {
        headerWidths = [];
        const headerCells = headerRow.children;
        for (let i = 0; i < headerCells.length; i++) {
            headerWidths.push(headerCells[i].offsetWidth);
        }
    };

    const applyAllWidths = () => {
        for (let i = 0; i < headerRow.children.length; i++) {
            setCellWidth(headerRow.children[i], headerWidths[i]);
        }

        try {
            const bodyRows = tableEl.querySelectorAll('tbody tr');
            bodyRows.forEach((tr) => {
                const tds = tr.children;
                for (let i = 0; i < tds.length && i < headerWidths.length; i++) {
                    setCellWidth(tds[i], headerWidths[i]);
                }
            });
        } catch (_) { }
    };

    const clearAllWidths = () => {
        try {
            for (let i = 0; i < headerRow.children.length; i++) {
                const el = headerRow.children[i];
                el.style.width = '';
                el.style.minWidth = '';
                el.style.maxWidth = '';
            }

            const bodyRows = tableEl.querySelectorAll('tbody tr');
            bodyRows.forEach((tr) => {
                const tds = tr.children;
                for (let i = 0; i < tds.length; i++) {
                    const el = tds[i];
                    el.style.width = '';
                    el.style.minWidth = '';
                    el.style.maxWidth = '';
                }
            });

            tableEl.style.width = '';
            tableEl.style.minWidth = '';
        } catch (_) { }
    };

    const markActiveColumn = (active) => {
        try {
            const th = headerRow.children[colIndex];
            if (active) th.classList.add('bch-col-active'); else th.classList.remove('bch-col-active');
            const bodyRows = tableEl.querySelectorAll('tbody tr');
            bodyRows.forEach((tr) => {
                const tds = tr.children;
                if (tds && tds[colIndex]) {
                    if (active) tds[colIndex].classList.add('bch-col-active'); else tds[colIndex].classList.remove('bch-col-active');
                }
            });
        } catch (_) { }
    };

    const onMouseMove = (e) => {
        const dx = e.clientX - startX;
        if (!moved) {
            if (Math.abs(dx) < threshold) {
                return;
            }
            moved = true;
            snapshotHeaderWidths();
            applyAllWidths();
            try {
                const baseSum = headerWidths.reduce((a, b) => a + b, 0);
                tableEl.style.width = baseSum + 'px';
                tableEl.style.minWidth = baseSum + 'px';
            } catch (_) { }
            try { tableEl.classList.add('bch-col-resizing'); } catch (_) { }
            markActiveColumn(true);
            document.body.classList.add('bch-col-resize-noselect');
        }
        const originalTarget = headerWidths[colIndex];
        const desired = Math.max(minW, startWidth + dx);
        let delta = desired - originalTarget;

        if (delta === 0) return;

        if (delta > 0) {
            headerWidths[colIndex] = originalTarget + delta;
        } else {
            headerWidths[colIndex] = originalTarget + delta;
        }

        applyAllWidths();
        try {
            const sum = headerWidths.reduce((a, b) => a + b, 0);
            tableEl.style.width = sum + 'px';
            tableEl.style.minWidth = sum + 'px';
        } catch (_) { }
    };

    const onMouseUp = () => {
        document.removeEventListener('mousemove', onMouseMove);
        document.removeEventListener('mouseup', onMouseUp);
        if (moved) {
            document.body.classList.remove('bch-col-resize-noselect');
            try { tableEl.classList.remove('bch-col-resizing'); } catch (_) { }
            markActiveColumn(false);
        }
        if (!moved) {
            clearAllWidths();
            return;
        }
        const widthCss = (headerWidths[colIndex] || thEl.offsetWidth) + 'px';
        if (dotNetRef) {
            dotNetRef.invokeMethodAsync('OnColumnResizeCompleted', widthCss);
        }
    };

    handleEl.addEventListener('mousedown', (e) => {
        e.preventDefault();
        startX = e.clientX;
        startWidth = thEl.offsetWidth;
        moved = false;
        snapshotHeaderWidths();
        applyAllWidths();
        try {
            const baseSum = headerWidths.reduce((a, b) => a + b, 0);
            tableEl.style.width = baseSum + 'px';
            tableEl.style.minWidth = baseSum + 'px';
        } catch (_) { }
        document.addEventListener('mousemove', onMouseMove);
        document.addEventListener('mouseup', onMouseUp);
    });

    handleEl.addEventListener('click', (e) => {
        e.preventDefault();
        e.stopPropagation();
    });
}
