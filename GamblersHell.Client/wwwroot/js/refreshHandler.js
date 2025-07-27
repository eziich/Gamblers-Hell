window.refreshHandler = {
    registerRefreshHandler: function (dotNetHelper) {
        window.addEventListener('beforeunload', function (e) {
            // Prevent the default behavior
            e.preventDefault();
            
            // Call the C# method
            dotNetHelper.invokeMethodAsync('HandlePageRefresh')
                .then(() => {
                    // After the C# method completes, allow the page to unload
                    delete e['returnValue'];
                })
                .catch(error => {
                    console.error('Error in HandlePageRefresh:', error);
                });
            
            // Return a message to show in the confirmation dialog
            return 'Are you sure you want to leave?';
        });
    }
}; 