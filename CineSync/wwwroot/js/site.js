var resizeTimeout;

function addResizeListener(dotNetObjRef) {
    // Define 'resizeHandler' inside 'addResizeListener' to capture 'dotNetObjRef' in closure
    var resizeHandler = function() {
        console.log('Resize detected');
        clearTimeout(resizeTimeout);
        resizeTimeout = setTimeout(function() {
            console.log('Invoking Blazor method');
            dotNetObjRef.invokeMethodAsync('UpdateDisplayMode').catch(error => {
                console.error('Error invoking Blazor method:', error);
            });
        }, 50);
    };

    // Attach the event listener with the handler that has closure over 'dotNetObjRef'
    window.addEventListener('resize', resizeHandler);

    // Return the handler so it can be removed later
    return resizeHandler;
}

// To manage removal, store the handler when adding the listener
var currentResizeHandler;

function removeResizeListener() {
    if (currentResizeHandler) {
        window.removeEventListener('resize', currentResizeHandler);
        currentResizeHandler = null;  // Clear the handler reference
    }
}

