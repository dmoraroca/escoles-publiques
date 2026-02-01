/**
 * schools-details.js
 * Lightweight initializer for Schools details page.
 * All button rendering is done server-side via the `_SaveCancelButtons` partial.
 * This file only exposes a safe `schoolsDetails.init()` hook for future helpers.
 */

window.schoolsDetails = {
    init: function() {
        // intentionally empty; wiring handled elsewhere
    }
};

document.addEventListener('DOMContentLoaded', function() {
    if (window.schoolsDetails && typeof window.schoolsDetails.init === 'function') {
        window.schoolsDetails.init();
    }
});
                    
