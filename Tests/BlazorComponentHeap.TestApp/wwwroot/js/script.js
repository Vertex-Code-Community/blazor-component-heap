function navigateToWithoutSaving(url) {
    window.history.replaceState({}, '', url);
}