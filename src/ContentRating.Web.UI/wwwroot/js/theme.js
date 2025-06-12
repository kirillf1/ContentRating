window.themeHelpers = {
    getSystemPreference: function() {
        return window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches;
    },
    
    matchMedia: function(query) {
        return window.matchMedia && window.matchMedia(query).matches;
    }
}; 