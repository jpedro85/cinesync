function addResizeListener(dotNetObjRef) {
    window.addEventListener('resize', () => {
        dotNetObjRef.invokeMethodAsync('UpdateDisplayMode');
    });
}

function removeResizeListener(dotNetObjRef) {
    window.removeEventListener('resize', () => {
        dotNetObjRef.invokeMethodAsync('UpdateDisplayMode');
    });
}
