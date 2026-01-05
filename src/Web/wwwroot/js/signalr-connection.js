const connection = new signalR.HubConnectionBuilder()
    .withUrl("/schoolHub")
    .withAutomaticReconnect()
    .build();

function reloadCurrentPage() {
    console.log('🔄 Recàrrega automàtica per canvis detectats...');
    setTimeout(() => location.reload(), 1000);
}

connection.on("SchoolCreated", function (school) {
    console.log('✅ Nova escola creada:', school.name);
    
    if (window.location.pathname.includes('/Schools')) {
        reloadCurrentPage();
    }
});

connection.on("SchoolUpdated", function (school) {
    console.log('🔄 Escola actualitzada:', school.name);
    
    if (window.location.pathname.includes('/Schools')) {
        reloadCurrentPage();
    }
});

connection.on("SchoolDeleted", function (data) {
    console.log('🗑️ Escola esborrada amb ID:', data.id);
    
    if (window.location.pathname.includes('/Schools')) {
        reloadCurrentPage();
    }
});

connection.start()
    .then(() => console.log('✅ SignalR connectat correctament'))
    .catch(err => console.error('❌ Error connectant SignalR:', err));

connection.onreconnecting(() => console.log('🔄 Reconnectant SignalR...'));
connection.onreconnected(() => console.log('✅ SignalR reconnectat'));
connection.onclose(() => console.log('❌ Connexió SignalR tancada'));
