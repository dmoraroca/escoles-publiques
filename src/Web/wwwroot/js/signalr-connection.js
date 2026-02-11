/**
 * signalr-connection.js
 * Manages SignalR connection for real-time updates on school events (create, update, etc.).
 */
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/schoolHub")
    .withAutomaticReconnect()
    .build();

function t(key, fallback) {
    return window.i18n ? window.i18n.t('signalr-connection.js', key, fallback) : (fallback || key);
}

function reloadCurrentPage() {
    console.log(t('AutoReload', '🔄 Recàrrega automàtica per canvis detectats...'));
    setTimeout(() => location.reload(), 1000);
}

connection.on("SchoolCreated", function (school) {
    console.log(t('SchoolCreated', '✅ Nova escola creada:'), school.name);
    
    if (window.location.pathname.includes('/Schools')) {
        reloadCurrentPage();
    }
});

connection.on("SchoolUpdated", function (school) {
    console.log(t('SchoolUpdated', '🔄 Escola actualitzada:'), school.name);
    
    if (window.location.pathname.includes('/Schools')) {
        reloadCurrentPage();
    }
});

connection.on("SchoolDeleted", function (data) {
    console.log(t('SchoolDeleted', '🗑️ Escola esborrada amb ID:'), data.id);
    
    if (window.location.pathname.includes('/Schools')) {
        reloadCurrentPage();
    }
});

connection.start()
    .then(() => console.log(t('SignalRConnected', '✅ SignalR connectat correctament')))
    .catch(err => console.error(t('SignalRConnectError', '❌ Error connectant SignalR:'), err));

connection.onreconnecting(() => console.log(t('SignalRReconnecting', '🔄 Reconnectant SignalR...')));
connection.onreconnected(() => console.log(t('SignalRReconnected', '✅ SignalR reconnectat')));
connection.onclose(() => console.log(t('SignalRClosed', '❌ Connexió SignalR tancada')));
