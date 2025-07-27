window.addBeforeUnloadHandler = function (dotNetRef) {
    window.addEventListener('beforeunload', function (e) {
        // Call the C# method synchronously
        dotNetRef.invokeMethodAsync('OnBeforeUnload');
        
        // Standard way to show a confirmation dialog
        e.preventDefault();
        e.returnValue = '';
    });
}; 