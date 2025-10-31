// Keyboard Shortcuts Module
let dotNetHelper = null;
let shortcuts = new Map();

export function initialize(dotNetReference) {
    dotNetHelper = dotNetReference;
    
    // Register keyboard event listener
    document.addEventListener('keydown', handleKeyDown);
}

function handleKeyDown(event) {
    // Don't trigger shortcuts when typing in input fields
    if (event.target.tagName === 'INPUT' || 
        event.target.tagName === 'TEXTAREA' || 
        event.target.isContentEditable) {
        return;
    }

    const key = buildKeyString(event);
    
    if (dotNetHelper) {
        dotNetHelper.invokeMethodAsync('HandleShortcut', key);
    }
}

function buildKeyString(event) {
    const parts = [];
    
    if (event.ctrlKey || event.metaKey) parts.push('ctrl');
    if (event.shiftKey) parts.push('shift');
    if (event.altKey) parts.push('alt');
    
    const keyName = event.key.toLowerCase();
    
    // Add the actual key
    if (!['control', 'shift', 'alt', 'meta'].includes(keyName)) {
        parts.push(keyName);
    }
    
    return parts.join('+');
}

export function dispose() {
    document.removeEventListener('keydown', handleKeyDown);
    dotNetHelper = null;
}
