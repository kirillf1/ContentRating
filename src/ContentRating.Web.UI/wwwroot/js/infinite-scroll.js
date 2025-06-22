window.infiniteScroll = window.infiniteScroll || {};

window.infiniteScroll.registerIntersection = function (element, dotNetHelper) {
    if (!element || !(element instanceof Element)) return;

    const observer = new IntersectionObserver(entries => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                // Просим .NET загрузить ещё элементы
                dotNetHelper.invokeMethodAsync('LoadMoreContent');
            }
        });
    }, {
        root: null,
        rootMargin: '0px',
        threshold: 0.1
    });

    observer.observe(element);
    element._infiniteScrollObserver = observer;
};

window.infiniteScroll.unregisterIntersection = function (element) {
    if (element && element._infiniteScrollObserver) {
        element._infiniteScrollObserver.disconnect();
        delete element._infiniteScrollObserver;
    }
}; 