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
  
    const onMouseMove = (e) => {
      const dx = e.clientX - startX;
      const newWidth = Math.max(60, startWidth + dx);
      thEl.style.width = newWidth + 'px';
      thEl.style.minWidth = newWidth + 'px';
      thEl.style.maxWidth = newWidth + 'px';
      try {
        const bodyRows = tableEl.querySelectorAll('tbody tr');
        bodyRows.forEach((tr) => {
          const tds = tr.children;
          if (tds && tds[colIndex]) {
            tds[colIndex].style.width = newWidth + 'px';
            tds[colIndex].style.minWidth = newWidth + 'px';
            tds[colIndex].style.maxWidth = newWidth + 'px';
          }
        });
      } catch (_) {
        // no-op
      }
    };
  
    const onMouseUp = () => {
      document.removeEventListener('mousemove', onMouseMove);
      document.removeEventListener('mouseup', onMouseUp);
      const widthCss = thEl.style.width || (thEl.offsetWidth + 'px');
      if (dotNetRef) {
        dotNetRef.invokeMethodAsync('OnColumnResizeCompleted', widthCss);
      }
    };
  
    handleEl.addEventListener('mousedown', (e) => {
      e.preventDefault();
      startX = e.clientX;
      startWidth = thEl.offsetWidth;
    try {
      const headerCells = headerRow.children;
      for (let i = 0; i < headerCells.length; i++) {
        const cell = headerCells[i];
        const w = cell.offsetWidth;
        cell.style.width = w + 'px';
        cell.style.minWidth = w + 'px';
        cell.style.maxWidth = w + 'px';
      }

      const bodyRows = tableEl.querySelectorAll('tbody tr');
      bodyRows.forEach((tr) => {
        const tds = tr.children;
        for (let i = 0; i < tds.length; i++) {
          const td = tds[i];
          const w = td.offsetWidth;
          td.style.width = w + 'px';
          td.style.minWidth = w + 'px';
          td.style.maxWidth = w + 'px';
        }
      });
    } catch (_) {
      // no-op
    }
      document.addEventListener('mousemove', onMouseMove);
      document.addEventListener('mouseup', onMouseUp);
    });
}
  