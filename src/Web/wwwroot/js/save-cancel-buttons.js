// save-cancel-buttons.js
// Minimal wiring for save/cancel partial: do not create DOM, only control behavior.
(function(document){
    function initComponent(container){
        if(!container) return;
        var raw = container.getAttribute('data-options') || '{}';
        var opts = {};
        try { opts = JSON.parse(raw); } catch(e){ console.warn('save-cancel-buttons: invalid data-options', e); }

        var saveBtn = container.querySelector('.save-btn');
        var cancelBtn = container.querySelector('.cancel-btn');
        var privacy = null;

        if(opts.privacySelector){
            privacy = document.querySelector(opts.privacySelector);
        }
        if(!privacy){
            privacy = container.querySelector('input[type=checkbox]');
        }

        function updateSaveState(){
            if(!saveBtn) return;
            var enabled = true;
            if(opts.disabledByDefault && privacy){
                enabled = !!privacy.checked;
            }
            saveBtn.disabled = !enabled;
            saveBtn.setAttribute('aria-disabled', (!enabled).toString());
        }

        if(privacy){
            privacy.addEventListener('change', updateSaveState);
            // initial
            updateSaveState();
        }

        if(cancelBtn){
            var action = opts.cancelAction || cancelBtn.getAttribute('data-cancel-action');
            cancelBtn.addEventListener('click', function(e){
                e.preventDefault();
                // Prefer explicit action
                if(action === 'reload') return location.reload();
                if(opts.cancelHref) return location.href = opts.cancelHref;

                // Try to cleanly reset the form and UI when inside a details/edit form
                var form = container.closest('form');
                if(form){
                    try {
                        form.reset();
                    } catch (ex) { /* ignore */ }

                    // remove validation markers
                    form.querySelectorAll('.is-invalid').forEach(function(el){ el.classList.remove('is-invalid'); });

                    // hide error alert if present
                    var errorAlert = form.querySelector('#errorAlert');
                    if(errorAlert){
                        errorAlert.classList.add('alert-hidden');
                        errorAlert.classList.remove('show');
                        errorAlert.style.display = 'none';
                    }

                    // restore readonly/disabled state for visible controls (exclude hidden inputs)
                    form.querySelectorAll('input:not([type=hidden]), select, textarea').forEach(function(el){
                        try { el.setAttribute('readonly','readonly'); } catch(e){}
                        try { el.setAttribute('disabled','disabled'); } catch(e){}
                    });

                    // toggle favorite display/edit areas if present
                    var favDisplay = document.getElementById('favoriteDisplay');
                    var favEdit = document.getElementById('favoriteEdit');
                    if(favDisplay) favDisplay.style.display = 'flex';
                    if(favEdit) favEdit.style.display = 'none';

                    // hide this component wrapper
                    container.style.display = 'none';

                    // restore header buttons if present
                    var header = document.getElementById('headerButtons');
                    if (header) header.style.display = '';

                    // ensure save button disabled state updated
                    try { updateSaveState(); } catch(e){}

                    return;
                }

                // fallback
                location.reload();
            });
        }
    }

    function initAll(){
        document.querySelectorAll('.save-cancel-component').forEach(initComponent);
    }

    if(document.readyState !== 'loading') initAll();
    else document.addEventListener('DOMContentLoaded', initAll);
})(document);
